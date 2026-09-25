---
title: C# node and value catalog
description: Typed definitions, convenience methods, units, and Godot property constraints.
---

The node/value catalog provides 306 built-in definitions (298 node/property definitions and 8 callback value definitions). Each has a typed convenience extension. Adapters use Godot properties directly; no runtime reflection or string property paths are used.

## Usage

```csharp
// sprite: Sprite2D, camera: Camera2D, label: Label; all inside the tree.
var movement = sprite.TweenPosition(new Vector2(300, 120), 0.5,
    options => options.Ease = EaseType.CubicOut);
var fade = sprite.TweenModulateAlpha(0, 0.2);
var zoom = camera.TweenZoom(new Vector2(2, 2), 0.4);
var reveal = label.TweenVisibleRatio(1, 1.5, options => options.From = 0);
await Task.WhenAll(movement.End, fade.End);
```

Signatures are `target.TweenProperty(to, duration, configure = null)`. The optional typed configure callback runs synchronously before starting playback and may override From, To, Duration or any other definition setting, including callbacks. Configuration errors propagate before playback is added. Keep using `target.Tween(new Definition { ... })` for reusable definitions. Both forms return the existing typed handle with pause/cancel/completion support and the same owner lifetime.

Base-class extensions apply to derived nodes: Node2D/Node3D provide transforms, CanvasItem provides 2D modulation, Control provides layout, SpriteBase3D provides 3D sprite appearance, GeometryInstance3D provides transparency, and Range provides Value. Specific overloads take precedence over generic Node callback-value helpers (e.g. a node with a color property uses its property-specific TweenColor overload). Use an explicit value definition when that distinction matters.

## Units and engine constraints

- Rotation, skew and texture rotation use radians. Camera Fov, light/emission angles and radial progress angles use degrees. Ratios use the native property range; positions, sizes, paths and offsets use their respective Godot units.
- Particle SpeedScale and Lifetime use double precision, matching Godot's properties. Other adapter value types match their native property types.
- Integer properties (visible characters, scroll positions and sprite frames) interpolate continuously, round nearest with midpoint ties away from zero, and saturate at Int32 limits. Native constraints still apply. Configure sprite frames/animations and scrollable content before tweening them. Negative visible-character sentinel values remain native Godot behavior; use explicit From = 0 for a reveal.
- Tweens.GlobalQuaternion3D performs shortest-path quaternion interpolation, then writes Godot's YXZ global rotation. It preserves current global position and basis scale. As with Godot GlobalRotation, it replaces shear and can change local scale under nonuniformly scaled parents. It does not preserve an arbitrary sheared transform. Singular transforms are outside the supported orientation use case.
- Node2D global scale/skew inherit Godot's parent-relative decomposition. Under a nonuniform parent, changing skew can also affect apparent scale; these are not independent transform channels. Axis setters preserve the other components of the property being written, not every derived transform component.
- Control anchors use push-opposite behavior. Minimum/maximum size constraints and containers still apply. OffsetTransform properties require OffsetTransformEnabled = true and allow visual motion/scale without rewriting container-controlled Position/Size. Ratio variants are relative to Control.Size.
- Paths require PathFollow2D/3D under a matching Path2D/3D with a nonempty curve. ProgressRatio is normalized; Progress is distance. Loop/wrap behavior remains controlled by the node.
- Camera Size and FrustumOffset depend on projection mode; light temperature depends on physical-light settings; particle emission parameters depend on configured emitter modes/materials. Setting a property does not enable rendering features or create resources.
- GeometryInstance3D.Transparency is a transparency amount (0 opaque, 1 transparent), not opacity. Renderer support and sorting limitations remain Godot's. A headless property test does not promise identical rendering across backends.
- Materials and shader parameters have their own [C# material guide](/csharp/materials/).

## Definitions and extensions

Each row is a supported definition and its convenience method. Names ending X/Y/Z or Alpha affect one component. All methods also take duration and an optional configure callback.

| Target | Definition | Extension | Value |
| --- | --- | --- | --- |
| AnimatedSprite2D | `Tweens.AnimatedSprite2DFrame` | `TweenFrame` | int |
| AnimatedSprite2D | `Tweens.AnimatedSprite2DSpeedScale` | `TweenSpeedScale` | float |
| AnimatedSprite3D | `Tweens.AnimatedSprite3DFrame` | `TweenFrame` | int |
| AnimatedSprite3D | `Tweens.AnimatedSprite3DSpeedScale` | `TweenSpeedScale` | float |
| AnimationPlayer | `Tweens.AnimationPlayerSpeedScale` | `TweenSpeedScale` | float |
| AudioStreamPlayer | `Tweens.AudioPitchScale` | `TweenPitchScale` | float |
| AudioStreamPlayer | `Tweens.AudioVolumeDb` | `TweenVolumeDb` | float |
| AudioStreamPlayer | `Tweens.AudioVolumeLinear` | `TweenVolumeLinear` | float |
| AudioStreamPlayer2D | `Tweens.AudioStreamPlayer2DAttenuation` | `TweenAttenuation` | float |
| AudioStreamPlayer2D | `Tweens.AudioStreamPlayer2DMaxDistance` | `TweenMaxDistance` | float |
| AudioStreamPlayer2D | `Tweens.AudioStreamPlayer2DPanningStrength` | `TweenPanningStrength` | float |
| AudioStreamPlayer2D | `Tweens.AudioPitchScale2D` | `TweenPitchScale` | float |
| AudioStreamPlayer2D | `Tweens.AudioVolumeDb2D` | `TweenVolumeDb` | float |
| AudioStreamPlayer2D | `Tweens.AudioVolumeLinear2D` | `TweenVolumeLinear` | float |
| AudioStreamPlayer3D | `Tweens.AudioStreamPlayer3DAttenuationFilterCutoffHz` | `TweenAttenuationFilterCutoffHz` | float |
| AudioStreamPlayer3D | `Tweens.AudioStreamPlayer3DAttenuationFilterDb` | `TweenAttenuationFilterDb` | float |
| AudioStreamPlayer3D | `Tweens.AudioStreamPlayer3DEmissionAngleDegrees` | `TweenEmissionAngleDegrees` | float |
| AudioStreamPlayer3D | `Tweens.AudioStreamPlayer3DEmissionAngleFilterAttenuationDb` | `TweenEmissionAngleFilterAttenuationDb` | float |
| AudioStreamPlayer3D | `Tweens.AudioStreamPlayer3DMaxDistance` | `TweenMaxDistance` | float |
| AudioStreamPlayer3D | `Tweens.AudioStreamPlayer3DPanningStrength` | `TweenPanningStrength` | float |
| AudioStreamPlayer3D | `Tweens.AudioPitchScale3D` | `TweenPitchScale` | float |
| AudioStreamPlayer3D | `Tweens.AudioStreamPlayer3DUnitSize` | `TweenUnitSize` | float |
| AudioStreamPlayer3D | `Tweens.AudioVolumeDb3D` | `TweenVolumeDb` | float |
| AudioStreamPlayer3D | `Tweens.AudioVolumeLinear3D` | `TweenVolumeLinear` | float |
| Camera2D | `Tweens.Camera2DOffset` | `TweenOffset` | Vector2 |
| Camera2D | `Tweens.Camera2DOffsetX` | `TweenOffsetX` | float |
| Camera2D | `Tweens.Camera2DOffsetY` | `TweenOffsetY` | float |
| Camera2D | `Tweens.Camera2DZoom` | `TweenZoom` | Vector2 |
| Camera2D | `Tweens.Camera2DZoomX` | `TweenZoomX` | float |
| Camera2D | `Tweens.Camera2DZoomY` | `TweenZoomY` | float |
| Camera3D | `Tweens.Camera3DFar` | `TweenFar` | float |
| Camera3D | `Tweens.Camera3DFov` | `TweenFov` | float |
| Camera3D | `Tweens.Camera3DFrustumOffset` | `TweenFrustumOffset` | Vector2 |
| Camera3D | `Tweens.Camera3DFrustumOffsetX` | `TweenFrustumOffsetX` | float |
| Camera3D | `Tweens.Camera3DFrustumOffsetY` | `TweenFrustumOffsetY` | float |
| Camera3D | `Tweens.Camera3DHOffset` | `TweenHOffset` | float |
| Camera3D | `Tweens.Camera3DNear` | `TweenNear` | float |
| Camera3D | `Tweens.Camera3DSize` | `TweenSize` | float |
| Camera3D | `Tweens.Camera3DVOffset` | `TweenVOffset` | float |
| CanvasItem | `Tweens.Modulate` | `TweenModulate` | Color |
| CanvasItem | `Tweens.ModulateAlpha` | `TweenModulateAlpha` | float |
| CanvasItem | `Tweens.SelfModulate` | `TweenSelfModulate` | Color |
| CanvasItem | `Tweens.SelfModulateAlpha` | `TweenSelfModulateAlpha` | float |
| CanvasLayer | `Tweens.CanvasLayerOffset` | `TweenOffset` | Vector2 |
| CanvasLayer | `Tweens.CanvasLayerOffsetX` | `TweenOffsetX` | float |
| CanvasLayer | `Tweens.CanvasLayerOffsetY` | `TweenOffsetY` | float |
| CanvasLayer | `Tweens.CanvasLayerRotation` | `TweenRotation` | float |
| CanvasLayer | `Tweens.CanvasLayerScale` | `TweenScale` | Vector2 |
| CanvasLayer | `Tweens.CanvasLayerScaleX` | `TweenScaleX` | float |
| CanvasLayer | `Tweens.CanvasLayerScaleY` | `TweenScaleY` | float |
| CanvasModulate | `Tweens.CanvasModulateColor` | `TweenColor` | Color |
| CanvasModulate | `Tweens.CanvasModulateColorAlpha` | `TweenColorAlpha` | float |
| ColorRect | `Tweens.ColorRectColor` | `TweenColor` | Color |
| ColorRect | `Tweens.ColorRectColorAlpha` | `TweenColorAlpha` | float |
| Control | `Tweens.ControlAnchorBottom` | `TweenAnchorBottom` | float |
| Control | `Tweens.ControlAnchorLeft` | `TweenAnchorLeft` | float |
| Control | `Tweens.ControlAnchorMax` | `TweenAnchorMax` | Vector2 |
| Control | `Tweens.ControlAnchorMin` | `TweenAnchorMin` | Vector2 |
| Control | `Tweens.ControlAnchorRight` | `TweenAnchorRight` | float |
| Control | `Tweens.ControlAnchorTop` | `TweenAnchorTop` | float |
| Control | `Tweens.ControlCustomMaximumSize` | `TweenCustomMaximumSize` | Vector2 |
| Control | `Tweens.ControlCustomMaximumSizeX` | `TweenCustomMaximumSizeX` | float |
| Control | `Tweens.ControlCustomMaximumSizeY` | `TweenCustomMaximumSizeY` | float |
| Control | `Tweens.ControlCustomMinimumSize` | `TweenCustomMinimumSize` | Vector2 |
| Control | `Tweens.ControlCustomMinimumSizeX` | `TweenCustomMinimumSizeX` | float |
| Control | `Tweens.ControlCustomMinimumSizeY` | `TweenCustomMinimumSizeY` | float |
| Control | `Tweens.ControlGlobalPosition` | `TweenGlobalPosition` | Vector2 |
| Control | `Tweens.ControlGlobalPositionX` | `TweenGlobalPositionX` | float |
| Control | `Tweens.ControlGlobalPositionY` | `TweenGlobalPositionY` | float |
| Control | `Tweens.ControlOffsetBottom` | `TweenOffsetBottom` | float |
| Control | `Tweens.ControlOffsetLeft` | `TweenOffsetLeft` | float |
| Control | `Tweens.ControlOffsetRight` | `TweenOffsetRight` | float |
| Control | `Tweens.ControlOffsets` | `TweenOffsets` | Vector4 |
| Control | `Tweens.ControlOffsetTop` | `TweenOffsetTop` | float |
| Control | `Tweens.ControlOffsetTransformPivot` | `TweenOffsetTransformPivot` | Vector2 |
| Control | `Tweens.ControlOffsetTransformPivotRatio` | `TweenOffsetTransformPivotRatio` | Vector2 |
| Control | `Tweens.ControlOffsetTransformPivotRatioX` | `TweenOffsetTransformPivotRatioX` | float |
| Control | `Tweens.ControlOffsetTransformPivotRatioY` | `TweenOffsetTransformPivotRatioY` | float |
| Control | `Tweens.ControlOffsetTransformPivotX` | `TweenOffsetTransformPivotX` | float |
| Control | `Tweens.ControlOffsetTransformPivotY` | `TweenOffsetTransformPivotY` | float |
| Control | `Tweens.ControlOffsetTransformPosition` | `TweenOffsetTransformPosition` | Vector2 |
| Control | `Tweens.ControlOffsetTransformPositionRatio` | `TweenOffsetTransformPositionRatio` | Vector2 |
| Control | `Tweens.ControlOffsetTransformPositionRatioX` | `TweenOffsetTransformPositionRatioX` | float |
| Control | `Tweens.ControlOffsetTransformPositionRatioY` | `TweenOffsetTransformPositionRatioY` | float |
| Control | `Tweens.ControlOffsetTransformPositionX` | `TweenOffsetTransformPositionX` | float |
| Control | `Tweens.ControlOffsetTransformPositionY` | `TweenOffsetTransformPositionY` | float |
| Control | `Tweens.ControlOffsetTransformRotation` | `TweenOffsetTransformRotation` | float |
| Control | `Tweens.ControlOffsetTransformScale` | `TweenOffsetTransformScale` | Vector2 |
| Control | `Tweens.ControlOffsetTransformScaleX` | `TweenOffsetTransformScaleX` | float |
| Control | `Tweens.ControlOffsetTransformScaleY` | `TweenOffsetTransformScaleY` | float |
| Control | `Tweens.ControlPivotOffset` | `TweenPivotOffset` | Vector2 |
| Control | `Tweens.ControlPivotOffsetRatio` | `TweenPivotOffsetRatio` | Vector2 |
| Control | `Tweens.ControlPivotOffsetRatioX` | `TweenPivotOffsetRatioX` | float |
| Control | `Tweens.ControlPivotOffsetRatioY` | `TweenPivotOffsetRatioY` | float |
| Control | `Tweens.ControlPivotOffsetX` | `TweenPivotOffsetX` | float |
| Control | `Tweens.ControlPivotOffsetY` | `TweenPivotOffsetY` | float |
| Control | `Tweens.ControlPosition` | `TweenPosition` | Vector2 |
| Control | `Tweens.ControlPositionX` | `TweenPositionX` | float |
| Control | `Tweens.ControlPositionY` | `TweenPositionY` | float |
| Control | `Tweens.ControlRotation` | `TweenRotation` | float |
| Control | `Tweens.ControlScale` | `TweenScale` | Vector2 |
| Control | `Tweens.ControlScaleX` | `TweenScaleX` | float |
| Control | `Tweens.ControlScaleY` | `TweenScaleY` | float |
| Control | `Tweens.ControlSize` | `TweenSize` | Vector2 |
| Control | `Tweens.ControlSizeFlagsStretchRatio` | `TweenSizeFlagsStretchRatio` | float |
| Control | `Tweens.ControlSizeX` | `TweenSizeX` | float |
| Control | `Tweens.ControlSizeY` | `TweenSizeY` | float |
| CpuParticles2D | `Tweens.CpuParticles2DColor` | `TweenColor` | Color |
| CpuParticles2D | `Tweens.CpuParticles2DColorAlpha` | `TweenColorAlpha` | float |
| CpuParticles2D | `Tweens.CpuParticles2DDirection` | `TweenDirection` | Vector2 |
| CpuParticles2D | `Tweens.CpuParticles2DDirectionX` | `TweenDirectionX` | float |
| CpuParticles2D | `Tweens.CpuParticles2DDirectionY` | `TweenDirectionY` | float |
| CpuParticles2D | `Tweens.CpuParticles2DEmissionRectExtents` | `TweenEmissionRectExtents` | Vector2 |
| CpuParticles2D | `Tweens.CpuParticles2DEmissionRectExtentsX` | `TweenEmissionRectExtentsX` | float |
| CpuParticles2D | `Tweens.CpuParticles2DEmissionRectExtentsY` | `TweenEmissionRectExtentsY` | float |
| CpuParticles2D | `Tweens.CpuParticles2DEmissionSphereRadius` | `TweenEmissionSphereRadius` | float |
| CpuParticles2D | `Tweens.CpuParticles2DExplosiveness` | `TweenExplosiveness` | float |
| CpuParticles2D | `Tweens.CpuParticles2DGravity` | `TweenGravity` | Vector2 |
| CpuParticles2D | `Tweens.CpuParticles2DGravityX` | `TweenGravityX` | float |
| CpuParticles2D | `Tweens.CpuParticles2DGravityY` | `TweenGravityY` | float |
| CpuParticles2D | `Tweens.CpuParticles2DLifetime` | `TweenLifetime` | double |
| CpuParticles2D | `Tweens.CpuParticles2DRandomness` | `TweenRandomness` | float |
| CpuParticles2D | `Tweens.CpuParticles2DSpeedScale` | `TweenSpeedScale` | double |
| CpuParticles2D | `Tweens.CpuParticles2DSpread` | `TweenSpread` | float |
| CpuParticles3D | `Tweens.CpuParticles3DColor` | `TweenColor` | Color |
| CpuParticles3D | `Tweens.CpuParticles3DColorAlpha` | `TweenColorAlpha` | float |
| CpuParticles3D | `Tweens.CpuParticles3DDirection` | `TweenDirection` | Vector3 |
| CpuParticles3D | `Tweens.CpuParticles3DDirectionX` | `TweenDirectionX` | float |
| CpuParticles3D | `Tweens.CpuParticles3DDirectionY` | `TweenDirectionY` | float |
| CpuParticles3D | `Tweens.CpuParticles3DDirectionZ` | `TweenDirectionZ` | float |
| CpuParticles3D | `Tweens.CpuParticles3DEmissionBoxExtents` | `TweenEmissionBoxExtents` | Vector3 |
| CpuParticles3D | `Tweens.CpuParticles3DEmissionBoxExtentsX` | `TweenEmissionBoxExtentsX` | float |
| CpuParticles3D | `Tweens.CpuParticles3DEmissionBoxExtentsY` | `TweenEmissionBoxExtentsY` | float |
| CpuParticles3D | `Tweens.CpuParticles3DEmissionBoxExtentsZ` | `TweenEmissionBoxExtentsZ` | float |
| CpuParticles3D | `Tweens.CpuParticles3DEmissionSphereRadius` | `TweenEmissionSphereRadius` | float |
| CpuParticles3D | `Tweens.CpuParticles3DExplosiveness` | `TweenExplosiveness` | float |
| CpuParticles3D | `Tweens.CpuParticles3DGravity` | `TweenGravity` | Vector3 |
| CpuParticles3D | `Tweens.CpuParticles3DGravityX` | `TweenGravityX` | float |
| CpuParticles3D | `Tweens.CpuParticles3DGravityY` | `TweenGravityY` | float |
| CpuParticles3D | `Tweens.CpuParticles3DGravityZ` | `TweenGravityZ` | float |
| CpuParticles3D | `Tweens.CpuParticles3DLifetime` | `TweenLifetime` | double |
| CpuParticles3D | `Tweens.CpuParticles3DRandomness` | `TweenRandomness` | float |
| CpuParticles3D | `Tweens.CpuParticles3DSpeedScale` | `TweenSpeedScale` | double |
| CpuParticles3D | `Tweens.CpuParticles3DSpread` | `TweenSpread` | float |
| Decal | `Tweens.DecalEmissionEnergy` | `TweenEmissionEnergy` | float |
| Decal | `Tweens.DecalModulate` | `TweenModulate` | Color |
| Decal | `Tweens.DecalModulateAlpha` | `TweenModulateAlpha` | float |
| Decal | `Tweens.DecalSize` | `TweenSize` | Vector3 |
| Decal | `Tweens.DecalSizeX` | `TweenSizeX` | float |
| Decal | `Tweens.DecalSizeY` | `TweenSizeY` | float |
| Decal | `Tweens.DecalSizeZ` | `TweenSizeZ` | float |
| FogVolume | `Tweens.FogVolumeSize` | `TweenSize` | Vector3 |
| FogVolume | `Tweens.FogVolumeSizeX` | `TweenSizeX` | float |
| FogVolume | `Tweens.FogVolumeSizeY` | `TweenSizeY` | float |
| FogVolume | `Tweens.FogVolumeSizeZ` | `TweenSizeZ` | float |
| GeometryInstance3D | `Tweens.GeometryInstance3DTransparency` | `TweenTransparency` | float |
| Godot.Range | `Tweens.RangeValue` | `TweenValue` | double |
| GpuParticles2D | `Tweens.GpuParticles2DAmountRatio` | `TweenAmountRatio` | float |
| GpuParticles2D | `Tweens.GpuParticles2DExplosiveness` | `TweenExplosiveness` | float |
| GpuParticles2D | `Tweens.GpuParticles2DLifetime` | `TweenLifetime` | double |
| GpuParticles2D | `Tweens.GpuParticles2DRandomness` | `TweenRandomness` | float |
| GpuParticles2D | `Tweens.GpuParticles2DSpeedScale` | `TweenSpeedScale` | double |
| GpuParticles3D | `Tweens.GpuParticles3DAmountRatio` | `TweenAmountRatio` | float |
| GpuParticles3D | `Tweens.GpuParticles3DExplosiveness` | `TweenExplosiveness` | float |
| GpuParticles3D | `Tweens.GpuParticles3DLifetime` | `TweenLifetime` | double |
| GpuParticles3D | `Tweens.GpuParticles3DRandomness` | `TweenRandomness` | float |
| GpuParticles3D | `Tweens.GpuParticles3DSpeedScale` | `TweenSpeedScale` | double |
| Label | `Tweens.LabelVisibleCharacters` | `TweenVisibleCharacters` | int |
| Label | `Tweens.LabelVisibleRatio` | `TweenVisibleRatio` | float |
| Label3D | `Tweens.Label3DModulate` | `TweenModulate` | Color |
| Label3D | `Tweens.Label3DModulateAlpha` | `TweenModulateAlpha` | float |
| Label3D | `Tweens.Label3DOffset` | `TweenOffset` | Vector2 |
| Label3D | `Tweens.Label3DOffsetX` | `TweenOffsetX` | float |
| Label3D | `Tweens.Label3DOffsetY` | `TweenOffsetY` | float |
| Label3D | `Tweens.Label3DOutlineModulate` | `TweenOutlineModulate` | Color |
| Label3D | `Tweens.Label3DOutlineModulateAlpha` | `TweenOutlineModulateAlpha` | float |
| Label3D | `Tweens.Label3DPixelSize` | `TweenPixelSize` | float |
| Light2D | `Tweens.LightColor2D` | `TweenColor` | Color |
| Light2D | `Tweens.LightEnergy2D` | `TweenEnergy` | float |
| Light2D | `Tweens.Light2DShadowColor` | `TweenShadowColor` | Color |
| Light2D | `Tweens.Light2DShadowColorAlpha` | `TweenShadowColorAlpha` | float |
| Light3D | `Tweens.LightColor3D` | `TweenLightColor` | Color |
| Light3D | `Tweens.LightEnergy3D` | `TweenLightEnergy` | float |
| Light3D | `Tweens.Light3DLightIndirectEnergy` | `TweenLightIndirectEnergy` | float |
| Light3D | `Tweens.Light3DLightTemperature` | `TweenLightTemperature` | float |
| Light3D | `Tweens.Light3DLightVolumetricFogEnergy` | `TweenLightVolumetricFogEnergy` | float |
| Light3D | `Tweens.Light3DShadowOpacity` | `TweenShadowOpacity` | float |
| Line2D | `Tweens.Line2DDefaultColor` | `TweenDefaultColor` | Color |
| Line2D | `Tweens.Line2DDefaultColorAlpha` | `TweenDefaultColorAlpha` | float |
| Line2D | `Tweens.Line2DWidth` | `TweenWidth` | float |
| Node | `Tweens.Color` | `TweenColor` | Color |
| Node | `Tweens.Double` | `TweenDouble` | double |
| Node | `Tweens.Float` | `TweenFloat` | float |
| Node | `Tweens.Quaternion` | `TweenQuaternion` | Quaternion |
| Node | `Tweens.Rect2` | `TweenRect2` | Rect2 |
| Node | `Tweens.Vector2` | `TweenVector2` | Vector2 |
| Node | `Tweens.Vector3` | `TweenVector3` | Vector3 |
| Node | `Tweens.Vector4` | `TweenVector4` | Vector4 |
| Node2D | `Tweens.GlobalPosition2D` | `TweenGlobalPosition` | Vector2 |
| Node2D | `Tweens.GlobalPosition2DX` | `TweenGlobalPositionX` | float |
| Node2D | `Tweens.GlobalPosition2DY` | `TweenGlobalPositionY` | float |
| Node2D | `Tweens.GlobalRotation2D` | `TweenGlobalRotation` | float |
| Node2D | `Tweens.GlobalScale2D` | `TweenGlobalScale` | Vector2 |
| Node2D | `Tweens.GlobalScale2DX` | `TweenGlobalScaleX` | float |
| Node2D | `Tweens.GlobalScale2DY` | `TweenGlobalScaleY` | float |
| Node2D | `Tweens.GlobalSkew2D` | `TweenGlobalSkew` | float |
| Node2D | `Tweens.Position2D` | `TweenPosition` | Vector2 |
| Node2D | `Tweens.Position2DX` | `TweenPositionX` | float |
| Node2D | `Tweens.Position2DY` | `TweenPositionY` | float |
| Node2D | `Tweens.Rotation2D` | `TweenRotation` | float |
| Node2D | `Tweens.Scale2D` | `TweenScale` | Vector2 |
| Node2D | `Tweens.Scale2DX` | `TweenScaleX` | float |
| Node2D | `Tweens.Scale2DY` | `TweenScaleY` | float |
| Node2D | `Tweens.Skew2D` | `TweenSkew` | float |
| Node3D | `Tweens.GlobalPosition3D` | `TweenGlobalPosition` | Vector3 |
| Node3D | `Tweens.GlobalPosition3DX` | `TweenGlobalPositionX` | float |
| Node3D | `Tweens.GlobalPosition3DY` | `TweenGlobalPositionY` | float |
| Node3D | `Tweens.GlobalPosition3DZ` | `TweenGlobalPositionZ` | float |
| Node3D | `Tweens.GlobalQuaternion3D` | `TweenGlobalQuaternion` | Quaternion |
| Node3D | `Tweens.GlobalRotation3D` | `TweenGlobalRotation` | Vector3 |
| Node3D | `Tweens.GlobalRotation3DX` | `TweenGlobalRotationX` | float |
| Node3D | `Tweens.GlobalRotation3DY` | `TweenGlobalRotationY` | float |
| Node3D | `Tweens.GlobalRotation3DZ` | `TweenGlobalRotationZ` | float |
| Node3D | `Tweens.Position3D` | `TweenPosition` | Vector3 |
| Node3D | `Tweens.Position3DX` | `TweenPositionX` | float |
| Node3D | `Tweens.Position3DY` | `TweenPositionY` | float |
| Node3D | `Tweens.Position3DZ` | `TweenPositionZ` | float |
| Node3D | `Tweens.Quaternion3D` | `TweenQuaternion` | Quaternion |
| Node3D | `Tweens.Rotation3D` | `TweenRotation` | Vector3 |
| Node3D | `Tweens.Rotation3DX` | `TweenRotationX` | float |
| Node3D | `Tweens.Rotation3DY` | `TweenRotationY` | float |
| Node3D | `Tweens.Rotation3DZ` | `TweenRotationZ` | float |
| Node3D | `Tweens.Scale3D` | `TweenScale` | Vector3 |
| Node3D | `Tweens.Scale3DX` | `TweenScaleX` | float |
| Node3D | `Tweens.Scale3DY` | `TweenScaleY` | float |
| Node3D | `Tweens.Scale3DZ` | `TweenScaleZ` | float |
| OmniLight3D | `Tweens.OmniLight3DOmniAttenuation` | `TweenOmniAttenuation` | float |
| OmniLight3D | `Tweens.OmniRange` | `TweenOmniRange` | float |
| Parallax2D | `Tweens.Parallax2DAutoscroll` | `TweenAutoscroll` | Vector2 |
| Parallax2D | `Tweens.Parallax2DAutoscrollX` | `TweenAutoscrollX` | float |
| Parallax2D | `Tweens.Parallax2DAutoscrollY` | `TweenAutoscrollY` | float |
| Parallax2D | `Tweens.Parallax2DScrollOffset` | `TweenScrollOffset` | Vector2 |
| Parallax2D | `Tweens.Parallax2DScrollOffsetX` | `TweenScrollOffsetX` | float |
| Parallax2D | `Tweens.Parallax2DScrollOffsetY` | `TweenScrollOffsetY` | float |
| Parallax2D | `Tweens.Parallax2DScrollScale` | `TweenScrollScale` | Vector2 |
| Parallax2D | `Tweens.Parallax2DScrollScaleX` | `TweenScrollScaleX` | float |
| Parallax2D | `Tweens.Parallax2DScrollScaleY` | `TweenScrollScaleY` | float |
| PathFollow2D | `Tweens.PathFollow2DHOffset` | `TweenHOffset` | float |
| PathFollow2D | `Tweens.PathFollow2DProgress` | `TweenProgress` | float |
| PathFollow2D | `Tweens.PathFollow2DProgressRatio` | `TweenProgressRatio` | float |
| PathFollow2D | `Tweens.PathFollow2DVOffset` | `TweenVOffset` | float |
| PathFollow3D | `Tweens.PathFollow3DHOffset` | `TweenHOffset` | float |
| PathFollow3D | `Tweens.PathFollow3DProgress` | `TweenProgress` | float |
| PathFollow3D | `Tweens.PathFollow3DProgressRatio` | `TweenProgressRatio` | float |
| PathFollow3D | `Tweens.PathFollow3DVOffset` | `TweenVOffset` | float |
| PointLight2D | `Tweens.PointLight2DHeight` | `TweenHeight` | float |
| PointLight2D | `Tweens.PointLight2DOffset` | `TweenOffset` | Vector2 |
| PointLight2D | `Tweens.PointLight2DOffsetX` | `TweenOffsetX` | float |
| PointLight2D | `Tweens.PointLight2DOffsetY` | `TweenOffsetY` | float |
| PointLight2D | `Tweens.PointLight2DTextureScale` | `TweenTextureScale` | float |
| Polygon2D | `Tweens.Polygon2DColor` | `TweenColor` | Color |
| Polygon2D | `Tweens.Polygon2DColorAlpha` | `TweenColorAlpha` | float |
| Polygon2D | `Tweens.Polygon2DOffset` | `TweenOffset` | Vector2 |
| Polygon2D | `Tweens.Polygon2DOffsetX` | `TweenOffsetX` | float |
| Polygon2D | `Tweens.Polygon2DOffsetY` | `TweenOffsetY` | float |
| Polygon2D | `Tweens.Polygon2DTextureOffset` | `TweenTextureOffset` | Vector2 |
| Polygon2D | `Tweens.Polygon2DTextureOffsetX` | `TweenTextureOffsetX` | float |
| Polygon2D | `Tweens.Polygon2DTextureOffsetY` | `TweenTextureOffsetY` | float |
| Polygon2D | `Tweens.Polygon2DTextureRotation` | `TweenTextureRotation` | float |
| Polygon2D | `Tweens.Polygon2DTextureScale` | `TweenTextureScale` | Vector2 |
| Polygon2D | `Tweens.Polygon2DTextureScaleX` | `TweenTextureScaleX` | float |
| Polygon2D | `Tweens.Polygon2DTextureScaleY` | `TweenTextureScaleY` | float |
| RichTextLabel | `Tweens.RichTextLabelVisibleCharacters` | `TweenVisibleCharacters` | int |
| RichTextLabel | `Tweens.RichTextLabelVisibleRatio` | `TweenVisibleRatio` | float |
| ScrollContainer | `Tweens.ScrollContainerScrollHorizontal` | `TweenScrollHorizontal` | int |
| ScrollContainer | `Tweens.ScrollContainerScrollVertical` | `TweenScrollVertical` | int |
| SpotLight3D | `Tweens.SpotAngle` | `TweenSpotAngle` | float |
| SpotLight3D | `Tweens.SpotLight3DSpotAngleAttenuation` | `TweenSpotAngleAttenuation` | float |
| SpotLight3D | `Tweens.SpotLight3DSpotAttenuation` | `TweenSpotAttenuation` | float |
| SpotLight3D | `Tweens.SpotRange` | `TweenSpotRange` | float |
| SpringArm3D | `Tweens.SpringArm3DSpringLength` | `TweenSpringLength` | float |
| Sprite2D | `Tweens.Sprite2DFrame` | `TweenFrame` | int |
| Sprite2D | `Tweens.Sprite2DOffset` | `TweenOffset` | Vector2 |
| Sprite2D | `Tweens.Sprite2DOffsetX` | `TweenOffsetX` | float |
| Sprite2D | `Tweens.Sprite2DOffsetY` | `TweenOffsetY` | float |
| Sprite2D | `Tweens.Sprite2DRegionRect` | `TweenRegionRect` | Rect2 |
| SpriteBase3D | `Tweens.SpriteBase3DModulate` | `TweenModulate` | Color |
| SpriteBase3D | `Tweens.SpriteBase3DModulateAlpha` | `TweenModulateAlpha` | float |
| SpriteBase3D | `Tweens.SpriteBase3DOffset` | `TweenOffset` | Vector2 |
| SpriteBase3D | `Tweens.SpriteBase3DOffsetX` | `TweenOffsetX` | float |
| SpriteBase3D | `Tweens.SpriteBase3DOffsetY` | `TweenOffsetY` | float |
| SpriteBase3D | `Tweens.SpriteBase3DPixelSize` | `TweenPixelSize` | float |
| TextureProgressBar | `Tweens.TextureProgressBarRadialCenterOffset` | `TweenRadialCenterOffset` | Vector2 |
| TextureProgressBar | `Tweens.TextureProgressBarRadialCenterOffsetX` | `TweenRadialCenterOffsetX` | float |
| TextureProgressBar | `Tweens.TextureProgressBarRadialCenterOffsetY` | `TweenRadialCenterOffsetY` | float |
| TextureProgressBar | `Tweens.TextureProgressBarRadialFillDegrees` | `TweenRadialFillDegrees` | float |
| TextureProgressBar | `Tweens.TextureProgressBarRadialInitialAngle` | `TweenRadialInitialAngle` | float |
| TextureProgressBar | `Tweens.TextureProgressBarTextureProgressOffset` | `TweenTextureProgressOffset` | Vector2 |
| TextureProgressBar | `Tweens.TextureProgressBarTextureProgressOffsetX` | `TweenTextureProgressOffsetX` | float |
| TextureProgressBar | `Tweens.TextureProgressBarTextureProgressOffsetY` | `TweenTextureProgressOffsetY` | float |
| TextureProgressBar | `Tweens.TextureProgressBarTintOver` | `TweenTintOver` | Color |
| TextureProgressBar | `Tweens.TextureProgressBarTintOverAlpha` | `TweenTintOverAlpha` | float |
| TextureProgressBar | `Tweens.TextureProgressBarTintProgress` | `TweenTintProgress` | Color |
| TextureProgressBar | `Tweens.TextureProgressBarTintProgressAlpha` | `TweenTintProgressAlpha` | float |
| TextureProgressBar | `Tweens.TextureProgressBarTintUnder` | `TweenTintUnder` | Color |
| TextureProgressBar | `Tweens.TextureProgressBarTintUnderAlpha` | `TweenTintUnderAlpha` | float |


Import `Godot` and `tweens.gd` in C# examples. For callback value definitions,
see [custom tweens](/csharp/custom-tweens/); for lifecycle rules, see
[lifetime and ownership](/concepts/lifetime/).
