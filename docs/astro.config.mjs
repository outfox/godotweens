// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import { tweensDark, tweensLight } from './src/styles/code-themes.mjs';

// Wraps Markdown tables in a scroll container, so a wide table scrolls inside the content lane instead of spilling past it.
// Registered on the Sätteri processor's hast pipeline, the same way Starlight adds its own transforms.
const tableScroll = {
	name: 'tweens-table-scroll',
	hooks: {
		'astro:config:setup': ({ config }) =>
			config.markdown.processor.options.hastPlugins.push({
				name: 'tweens-table-wrap',
				element: [
					{
						filter: ['table'],
						visit: (node, ctx) =>
							ctx.wrapNode(node, { type: 'element', tagName: 'div', properties: { className: ['table-wrap'] }, children: [] }),
					},
				],
			}),
	},
};

// https://astro.build/config
export default defineConfig({
	site: 'https://tweens.gd',
	integrations: [
		tableScroll,
		starlight({
			title: 'tweens.gd',
			description: 'An alternative tweening library for Godot. C# guides, playback concepts, and API reference.',
			tableOfContents: false,
			customCss: [
				'@fontsource-variable/bricolage-grotesque/standard.css',
				'@fontsource-variable/figtree',
				'@fontsource-variable/jetbrains-mono',
				'./src/styles/theme.css',
			],
			components: {
				Head: './src/components/overrides/Head.astro',
				Hero: './src/components/overrides/Hero.astro',
				PageTitle: './src/components/overrides/PageTitle.astro',
				SiteTitle: './src/components/overrides/SiteTitle.astro',
				MarkdownContent: './src/components/overrides/MarkdownContent.astro',
			},
			expressiveCode: {
				themes: [tweensDark, tweensLight],
				styleOverrides: {
					borderRadius: '0.9rem',
					borderColor: 'var(--tw-outline)',
					codeFontFamily: 'var(--__sl-font-mono)',
					uiFontFamily: 'var(--__sl-font)',
					codeBackground: 'var(--tw-stage)',
					frames: {
						editorTabBarBackground: 'var(--tw-surface)',
						// The theme's accent stripe on the active tab gets clipped by the frame's corner radius.
						editorActiveTabIndicatorTopColor: 'transparent',
						editorActiveTabIndicatorBottomColor: 'transparent',
						terminalTitlebarBackground: 'var(--tw-surface)',
						frameBoxShadowCssValue: 'var(--tw-shadow)',
					},
				},
			},
			social: [
				{ icon: 'discord', label: 'Discord', href: 'https://discord.gg/3UXVHnmEwd' },
				{ icon: 'github', label: 'GitHub', href: 'https://github.com/outfox/tweens.gd' },
			],
			// Ordered as a learning path; Starlight's prev/next links follow it page by page.
			sidebar: [
				{
					label: 'Start here',
					items: [
						{ label: 'Overview', slug: '' },
						{ label: 'Install', slug: 'csharp/installation' },
						{ label: 'Your first tween', slug: 'csharp/quickstart' },
					],
				},
				{
					label: 'Write reusable tweens',
					items: [
						{ label: 'Definitions', slug: 'concepts/definitions' },
						{ label: 'Sequences', slug: 'csharp/sequences' },
						{ label: 'Control & completion', slug: 'csharp/playback' },
					],
				},
				{
					label: 'Shape the motion',
					items: [
						{ label: 'Easing', slug: 'concepts/easing' },
						{ label: 'Timing & loops', slug: 'concepts/timing' },
						{ label: 'Lifetime & ownership', slug: 'concepts/lifetime' },
					],
				},
				{
					label: 'Beyond nodes',
					items: [
						{ label: 'Materials', slug: 'csharp/materials' },
						{ label: 'Shader uniforms', slug: 'csharp/shaders' },
						{ label: 'Custom tweens', slug: 'csharp/custom-tweens' },
					],
				},
				{
					label: 'Reference',
					items: [
						{ label: 'Core API', slug: 'csharp/api' },
						{ label: 'Node & value catalog', slug: 'csharp/nodes' },
						{ label: 'Compatibility', slug: 'compatibility' },
					],
				},
				{
					label: 'GDScript',
					items: [{ label: 'Addon status', slug: 'gdscript', badge: { text: 'Planned', variant: 'note' } }],
				},
			],
		}),
	],
});
