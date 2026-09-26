// Easing math ported from csharp/src/Easing/Easing.cs (adapted from unity-tweens, MIT).
// The docs animate with the same curves the library ships.

const A = 1.70158;
const B = A * 1.525;
const C = A + 1;
const D = (2 * Math.PI) / 3;
const E = (2 * Math.PI) / 4.5;
const F = 7.5625;
const G = 2.75;

const bounceOut = (t: number) => {
	if (t < 1 / G) return F * t * t;
	if (t < 2 / G) return F * (t -= 1.5 / G) * t + 0.75;
	if (t < 2.5 / G) return F * (t -= 2.25 / G) * t + 0.9375;
	return F * (t -= 2.625 / G) * t + 0.984375;
};

export const EASES = {
	Linear: (t: number) => t,
	SineIn: (t: number) => 1 - Math.cos((t * Math.PI) / 2),
	SineOut: (t: number) => Math.sin((t * Math.PI) / 2),
	SineInOut: (t: number) => -(Math.cos(Math.PI * t) - 1) / 2,
	QuadIn: (t: number) => t * t,
	QuadOut: (t: number) => 1 - (1 - t) * (1 - t),
	QuadInOut: (t: number) => (t < 0.5 ? 2 * t * t : 1 - Math.pow(-2 * t + 2, 2) / 2),
	CubicIn: (t: number) => t * t * t,
	CubicOut: (t: number) => 1 - Math.pow(1 - t, 3),
	CubicInOut: (t: number) => (t < 0.5 ? 4 * t * t * t : 1 - Math.pow(-2 * t + 2, 3) / 2),
	QuartIn: (t: number) => t ** 4,
	QuartOut: (t: number) => 1 - Math.pow(1 - t, 4),
	QuartInOut: (t: number) => (t < 0.5 ? 8 * t ** 4 : 1 - Math.pow(-2 * t + 2, 4) / 2),
	QuintIn: (t: number) => t ** 5,
	QuintOut: (t: number) => 1 - Math.pow(1 - t, 5),
	QuintInOut: (t: number) => (t < 0.5 ? 16 * t ** 5 : 1 - Math.pow(-2 * t + 2, 5) / 2),
	ExpoIn: (t: number) => (t === 0 ? 0 : Math.pow(2, 10 * t - 10)),
	ExpoOut: (t: number) => (t === 1 ? 1 : 1 - Math.pow(2, -10 * t)),
	ExpoInOut: (t: number) =>
		t === 0 ? 0 : t === 1 ? 1 : t < 0.5 ? Math.pow(2, 20 * t - 10) / 2 : (2 - Math.pow(2, -20 * t + 10)) / 2,
	CircIn: (t: number) => 1 - Math.sqrt(1 - t * t),
	CircOut: (t: number) => Math.sqrt(1 - (t - 1) ** 2),
	CircInOut: (t: number) =>
		t < 0.5 ? (1 - Math.sqrt(1 - (2 * t) ** 2)) / 2 : (Math.sqrt(1 - (-2 * t + 2) ** 2) + 1) / 2,
	BackIn: (t: number) => C * t ** 3 - A * t * t,
	BackOut: (t: number) => 1 + C * (t - 1) ** 3 + A * (t - 1) ** 2,
	BackInOut: (t: number) =>
		t < 0.5
			? ((2 * t) ** 2 * ((B + 1) * 2 * t - B)) / 2
			: ((2 * t - 2) ** 2 * ((B + 1) * (t * 2 - 2) + B) + 2) / 2,
	ElasticIn: (t: number) =>
		t === 0 ? 0 : t === 1 ? 1 : -Math.pow(2, 10 * t - 10) * Math.sin((t * 10 - 10.75) * D),
	ElasticOut: (t: number) =>
		t === 0 ? 0 : t === 1 ? 1 : Math.pow(2, -10 * t) * Math.sin((t * 10 - 0.75) * D) + 1,
	ElasticInOut: (t: number) =>
		t === 0
			? 0
			: t === 1
				? 1
				: t < 0.5
					? -(Math.pow(2, 20 * t - 10) * Math.sin((20 * t - 11.125) * E)) / 2
					: (Math.pow(2, -20 * t + 10) * Math.sin((20 * t - 11.125) * E)) / 2 + 1,
	BounceIn: (t: number) => 1 - bounceOut(1 - t),
	BounceOut: bounceOut,
	BounceInOut: (t: number) => (t < 0.5 ? (1 - bounceOut(1 - 2 * t)) / 2 : (1 + bounceOut(2 * t - 1)) / 2),
} as const;

export type EaseName = keyof typeof EASES;
export const ease = (name: EaseName, t: number) => EASES[name](Math.min(1, Math.max(0, t)));

export const reducedMotion = () => matchMedia('(prefers-reduced-motion: reduce)').matches;

/** `Element.animate` for small feedback pops, skipped under reduced motion (the CSS rule cannot reach script animations). */
export const animate = (el: Element, keyframes: Keyframe[], options: KeyframeAnimationOptions) =>
	reducedMotion() ? undefined : el.animate(keyframes, options);

export const lerp = (a: number, b: number, w: number) => a + (b - a) * w;

export interface TweenHandle {
	cancel(): void;
	completion: Promise<'completed' | 'cancelled'>;
}

/** A single leg, sampled every frame. Reduced motion jumps straight to the end. */
export function tween(
	duration: number,
	onUpdate: (weight: number, progress: number) => void,
	easeName: EaseName = 'CubicOut',
	delay = 0,
): TweenHandle {
	let raf = 0;
	let settle!: (r: 'completed' | 'cancelled') => void;
	const completion = new Promise<'completed' | 'cancelled'>((r) => (settle = r));
	if (reducedMotion()) {
		onUpdate(1, 1);
		settle('completed');
		return { cancel() {}, completion };
	}
	const start = performance.now() + delay * 1000;
	const step = (now: number) => {
		const p = Math.min(1, Math.max(0, (now - start) / (duration * 1000)));
		if (now >= start) onUpdate(ease(easeName, p), p);
		if (p < 1) raf = requestAnimationFrame(step);
		else settle('completed');
	};
	raf = requestAnimationFrame(step);
	return {
		cancel() {
			cancelAnimationFrame(raf);
			settle('cancelled');
		},
		completion,
	};
}

/**
 * Runs `frame(elapsedSeconds)` while the element is on screen and the tab is visible.
 * Returns a stop function. Under reduced motion `frame` runs once at `stillTime`.
 */
export function loop(el: Element, frame: (t: number, dt: number) => void, stillTime = 0) {
	if (reducedMotion()) {
		frame(stillTime, 0);
		return () => {};
	}
	let raf = 0;
	let visible = false;
	let last = 0;
	let elapsed = stillTime;
	const tick = (now: number) => {
		const dt = last ? Math.min(0.1, (now - last) / 1000) : 0;
		last = now;
		elapsed += dt;
		frame(elapsed, dt);
		raf = requestAnimationFrame(tick);
	};
	const sync = () => {
		const run = visible && !document.hidden;
		if (run && !raf) {
			last = 0;
			raf = requestAnimationFrame(tick);
		} else if (!run && raf) {
			cancelAnimationFrame(raf);
			raf = 0;
		}
	};
	frame(elapsed, 0);
	const io = new IntersectionObserver(([entry]) => {
		visible = entry.isIntersecting;
		sync();
	});
	io.observe(el);
	document.addEventListener('visibilitychange', sync);
	return () => {
		io.disconnect();
		document.removeEventListener('visibilitychange', sync);
		cancelAnimationFrame(raf);
		raf = 0;
	};
}

/** Calls `cb` once, the first time the element scrolls into view. */
export function onceVisible(el: Element, cb: () => void, threshold = 0.25) {
	const io = new IntersectionObserver(
		(entries) => {
			if (entries.some((e) => e.isIntersecting)) {
				io.disconnect();
				cb();
			}
		},
		{ threshold },
	);
	io.observe(el);
}

/** SVG path for an ease curve inside a w×h box, y flipped, with headroom for overshoot. */
export function curvePath(name: EaseName, w: number, h: number, pad = 0, samples = 96) {
	let d = '';
	for (let i = 0; i <= samples; i++) {
		const t = i / samples;
		const x = t * w;
		const y = h - pad - EASES[name](t) * (h - 2 * pad);
		d += `${i ? 'L' : 'M'}${x.toFixed(2)} ${y.toFixed(2)}`;
	}
	return d;
}
