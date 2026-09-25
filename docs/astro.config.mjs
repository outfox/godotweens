// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';

// https://astro.build/config
export default defineConfig({
	site: 'https://tweens.gd',
	integrations: [
		starlight({
			title: 'tweens.gd',
			description: 'Reusable tweens for Godot. C# guides, playback concepts, and API reference.',
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
				themes: ['github-dark-default', 'github-light'],
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
			social: [{ icon: 'github', label: 'GitHub', href: 'https://github.com/outfox/tweens.gd' }],
			sidebar: [
				{ label: 'Overview', slug: '' },
				{ label: 'Compatibility & availability', slug: 'compatibility' },
				{
					label: 'Concepts',
					items: [
						{ label: 'Definitions & playback', slug: 'concepts/definitions' },
						{ label: 'Timing & loops', slug: 'concepts/timing' },
						{ label: 'Lifetime & ownership', slug: 'concepts/lifetime' },
						{ label: 'Easing', slug: 'concepts/easing' },
					],
				},
				{
					label: 'C#',
					items: [
						{ label: 'Installation', slug: 'csharp/installation' },
						{ label: 'Quickstart', slug: 'csharp/quickstart' },
						{ label: 'Playback & async', slug: 'csharp/playback' },
						{ label: 'Sequences', slug: 'csharp/sequences' },
						{ label: 'Custom tweens', slug: 'csharp/custom-tweens' },
						{ label: 'Gallery', slug: 'csharp/gallery' },
					],
				},
				{
					label: 'C# reference',
					items: [
						{ label: 'Core API', slug: 'csharp/api' },
						{ label: 'Node & value catalog', slug: 'csharp/nodes' },
						{ label: 'Materials', slug: 'csharp/materials' },
						{ label: 'Shader uniforms', slug: 'csharp/shaders' },
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
