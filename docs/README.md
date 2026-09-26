# tweens.gd public documentation

Astro/Starlight documentation for the C# library and planned GDScript addon.
Content lives in `src/content/docs/`; sidebar order is explicit in
`astro.config.mjs`. Internal working documents stay outside this site.

## Local development

From `docs/`, with a Node version supported by the locked Astro dependencies:

```powershell
npm ci
npm run dev -- --background
```

Manage the server with `npm run astro -- dev status`, `npm run astro -- dev logs`,
and `npm run astro -- dev stop`.

```powershell
npm run build
npm run check:links
npm run preview -- --background
```

The build emits `dist/`, including Pagefind search. The link check inspects
generated HTML targets and anchors; it also rejects internal-document links and
starter-template text. Preview the production build when checking search.

## Content conventions

- The sidebar in `astro.config.mjs` is a learning path: start here, write reusable tweens,
  shape the motion, beyond nodes, then reference. Prev/next links follow it.
- Each page opens with a one-paragraph lede stating its key idea; the theme sets it apart.
  Pitfalls go in `:::caution` asides so they stand out from the main flow.
- Annotated examples use Expressive Code line-marker labels (`{"1":3-7}`) inside `<Moves>`,
  whose numbered notes match the labels.
- Shared concepts live in `concepts/` and describe current C# behavior; the landing page and
  `gdscript/` identify the addon as planned.
- C# guides and reference live in `csharp/`, with explicit prerequisites for snippets.
- GDScript installation and code examples wait for a tested public API.
- Keep release procedures, coverage reports, and implementation notes private.
- Keep library versions, adapter names, and examples aligned with the source.
- Check C# code fences with `pwsh ./scripts/Check-Examples.ps1` from this directory.
  It compiles snippets against the library using explicit context for fragments;
  it does not execute native examples or certify their rendered output.

The public URL is `https://tweens.gd`, set as Astro `site` for canonical URLs and
the sitemap. Links assume that domain-root deployment. statichost.eu builds and
deploys the site automatically from a repository webhook; no workflow is needed here.
