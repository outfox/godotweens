// Syntax colors for code blocks, taken from the site palette (theme.css) so they match the hand-highlighted
// code in the hero and demos: blue keywords, mint types, amber functions and numbers.
import { ExpressiveCodeTheme } from '@astrojs/starlight/expressive-code';

const theme = (name, type, c) =>
	new ExpressiveCodeTheme({
		name,
		type,
		colors: { 'editor.background': c.bg, 'editor.foreground': c.fg },
		tokenColors: [
			{ scope: ['comment', 'punctuation.definition.comment'], settings: { foreground: c.comment, fontStyle: 'italic' } },
			{ scope: ['punctuation', 'keyword.operator', 'meta.brace'], settings: { foreground: c.punctuation } },
			{
				scope: ['keyword', 'storage.modifier', 'storage.type', 'keyword.operator.expression', 'keyword.operator.new', 'constant.language'],
				settings: { foreground: c.keyword },
			},
			{
				scope: ['entity.name.type', 'entity.name.class', 'entity.other.inherited-class', 'support.type', 'support.class', 'storage.type.cs'],
				settings: { foreground: c.type },
			},
			{ scope: ['entity.name.function', 'support.function', 'meta.function-call'], settings: { foreground: c.fn } },
			{ scope: ['constant.numeric'], settings: { foreground: c.number } },
			{ scope: ['string', 'punctuation.definition.string', 'constant.character'], settings: { foreground: c.string } },
			{ scope: ['variable', 'entity.name.variable', 'variable.other'], settings: { foreground: c.variable } },
			{ scope: ['markup.inserted'], settings: { foreground: c.type } },
			{ scope: ['markup.deleted'], settings: { foreground: c.string } },
		],
	});

export const tweensDark = theme('tweens-dark', 'dark', {
	bg: '#111b27',
	fg: '#c9d6e2',
	comment: '#8a9db2',
	punctuation: '#a9bccd',
	keyword: '#a9bfff',
	type: '#79deb4',
	fn: '#f2bc74',
	number: '#f2bc74',
	string: '#ff8fa3',
	variable: '#eef4fa',
});

export const tweensLight = theme('tweens-light', 'light', {
	bg: '#e9eff6',
	fg: '#243446',
	comment: '#52667b',
	punctuation: '#4d6076',
	keyword: '#3552c7',
	type: '#0d7353',
	fn: '#95550a',
	number: '#95550a',
	string: '#a8304f',
	variable: '#0e1620',
});
