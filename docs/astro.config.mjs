// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';

// https://astro.build/config
export default defineConfig({
	integrations: [
		starlight({
			title: 'tweens.gd',
			description: 'Reusable tweens for Godot. C# guides, playback concepts, and API reference.',
			social: [{ icon: 'github', label: 'GitHub', href: 'https://github.com/outfox/godotweens' }],
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
