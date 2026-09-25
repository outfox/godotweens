import { readdir, readFile, stat } from 'node:fs/promises';
import { dirname, join, relative, resolve, sep } from 'node:path';
import { fileURLToPath } from 'node:url';

const output = resolve(dirname(fileURLToPath(import.meta.url)), '../dist');
const origin = 'https://docs.invalid';
const decode = (text) => text.replace(/&(?:amp|quot|apos|lt|gt|#\d+|#x[\da-f]+);/gi, (entity) => {
  const named = { '&amp;': '&', '&quot;': '"', '&apos;': "'", '&lt;': '<', '&gt;': '>' };
  if (named[entity]) return named[entity];
  return String.fromCodePoint(entity.startsWith('&#x')
    ? Number.parseInt(entity.slice(3, -1), 16) : Number(entity.slice(2, -1)));
});

async function walk(directory) {
  const entries = await readdir(directory, { withFileTypes: true });
  return (await Promise.all(entries.map((entry) => entry.isDirectory()
    ? walk(join(directory, entry.name)) : join(directory, entry.name)))).flat();
}

const pages = new Map();
for (const file of (await walk(output)).filter((file) => file.endsWith('.html'))) {
  const html = await readFile(file, 'utf8');
  const route = '/' + relative(output, file).split(sep).join('/').replace(/index\.html$/, '');
  pages.set(file, {
    html, route,
    ids: new Set([...html.matchAll(/\bid="([^"]*)"/g)].map((match) => decode(match[1]))),
  });
}
if (!pages.size) throw new Error('No built HTML found. Run npm run build first.');

const errors = [];
let links = 0;
for (const { html, route } of pages.values()) {
  if (/Welcome to Starlight|Example Guide|Example Reference|My Docs|houston\.webp/.test(html)) {
    errors.push(`${route}: starter-template content remains`);
  }
  // Astro's static HTML quotes attribute values; code fences escape their markup.
  for (const match of html.matchAll(/\b(?:href|src)="([^"]*)"/g)) {
    const attribute = decode(match[1]);
    if (/(?:docs-internal|\/plans\/|\/artifacts\/)/i.test(attribute)) {
      errors.push(`${route}: private-document link ${attribute}`);
      continue;
    }
    const target = new URL(attribute, origin + route);
    if (target.origin !== origin) continue;
    links++;
    let file = resolve(output, '.' + decodeURIComponent(target.pathname));
    if (file !== output && !file.startsWith(output + sep)) {
      errors.push(`${route}: target escapes build output: ${attribute}`);
      continue;
    }
    try {
      if ((await stat(file)).isDirectory()) file = join(file, 'index.html');
      await stat(file);
      if (target.hash && pages.has(file) && !pages.get(file).ids.has(decodeURIComponent(target.hash.slice(1)))) {
        errors.push(`${route}: missing anchor ${attribute}`);
      }
    } catch {
      errors.push(`${route}: missing target ${attribute}`);
    }
  }
}
if (errors.length) throw new Error(errors.join('\n'));
console.log(`Checked ${links} local links/assets and anchors across ${pages.size} HTML pages.`);
