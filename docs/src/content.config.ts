import { defineCollection } from 'astro:content';
import { z } from 'astro/zod';
import { docsLoader } from '@astrojs/starlight/loaders';
import { docsSchema } from '@astrojs/starlight/schema';

export const collections = {
	docs: defineCollection({
		loader: docsLoader(),
		schema: docsSchema({
			extend: z.object({
				// Optional HTML for the hero; description stays plain text for page metadata.
				heroDescription: z.string().optional(),
				// Short facts under the landing hero, such as supported languages and engine versions.
				facts: z.array(z.object({ text: z.string(), link: z.string().optional() })).optional(),
			}),
		}),
	}),
};
