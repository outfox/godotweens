# Node tween catalog

The node/value catalog provides 306 built-in definitions (298 node/property definitions and 8 callback value definitions). Each has a typed convenience extension. Adapters use Godot properties directly; no runtime reflection or string property paths are used.

## Usage

```csharp
var movement = sprite.TweenPosition(new Vector2(300, 120), 0.5,
    options => options.Ease = EaseType.CubicOut);
var fade = sprite.TweenModulateAlpha(0, 0.2);
var zoom = camera.TweenZoom(new Vector2(2, 2), 0.4);
var reveal = label.TweenVisibleRatio(1, 1.5, options => options.From = 0);
await Task.WhenAll(movement.Completion, fade.Completion);
```

Signatures are `target.TweenProperty(to, duration, configure = null)`. The optional typed configure callback runs synchronously before starting playback and may override From, To, Duration or any other definition setting, including callbacks. Configuration errors propagate before playback is added. Keep using `target.Tween(new Definition { ... })` for reusable definitions. Both forms return the existing typed handle with pause/cancel/completion support and the same owner lifetime.

Base-class extensions apply to derived nodes: Node2D/Node3D provide transforms, CanvasItem provides 2D modulation, Control provides layout, SpriteBase3D provides 3D sprite appearance, GeometryInstance3D provides transparency, and Range provides Value. Specific overloads take precedence over generic Node callback-value helpers (e.g. a node with a color property uses its property-specific TweenColor overload). Use an explicit value definition when that distinction matters.

## Units and engine constraints

- Rotation, skew and texture rotation use radians. Camera Fov, light/emission angles and radial progress angles use degrees. Ratios use the native property range; positions, sizes, paths and offsets use their respective Godot units.
- Particle SpeedScale and Lifetime use double precision, matching Godot's properties. Other adapter value types match their native property types.
- Integer properties (visible characters, scroll positions and sprite frames) interpolate continuously, round nearest with midpoint ties away from zero, and saturate at Int32 limits. Native constraints still apply. Configure sprite frames/animations and scrollable content before tweening them. Negative visible-character sentinel values remain native Godot behavior; use explicit From = 0 for a reveal.
- GlobalQuaternion3DTween performs shortest-path quaternion interpolation, then writes Godot's YXZ global rotation. It preserves current global position and basis scale. As with Godot GlobalRotation, it replaces shear and can change local scale under nonuniformly scaled parents. It does not preserve an arbitrary sheared transform. Singular transforms are outside the supported orientation use case.
- Node2D global scale/skew inherit Godot's parent-relative decomposition. Under a nonuniform parent, changing skew can also affect apparent scale; these are not independent transform channels. Axis setters preserve the other components of the property being written, not every derived transform component.
- Control anchors use push-opposite behavior. Minimum/maximum size constraints and containers still apply. OffsetTransform properties require OffsetTransformEnabled = true and allow visual motion/scale without rewriting container-controlled Position/Size. Ratio variants are relative to Control.Size.
- Paths require PathFollow2D/3D under a matching Path2D/3D with a nonempty curve. ProgressRatio is normalized; Progress is distance. Loop/wrap behavior remains controlled by the node.
- Camera Size and FrustumOffset depend on projection mode; light temperature depends on physical-light settings; particle emission parameters depend on configured emitter modes/materials. Setting a property does not enable rendering features or create resources.
- GeometryInstance3D.Transparency is a transparency amount (0 opaque, 1 transparent), not opacity. Renderer support and sorting limitations remain Godot's. A headless property test does not promise identical rendering across backends.
- Materials and shader parameters have their own [implemented API and validation guide](MATERIAL-TWEENS.md).

## Definitions and extensions

Each row is a supported definition and its convenience method. Names ending X/Y/Z or Alpha affect one component. All methods also take duration and an optional configure callback.

| Target | Definition | Extension | Value |
| --- | --- | --- | --- |
| AnimatedSprite2D | `AnimatedSprite2DFrameTween` | `TweenFrame` | int |
| AnimatedSprite2D | `AnimatedSprite2DSpeedScaleTween` | `TweenSpeedScale` | float |
| AnimatedSprite3D | `AnimatedSprite3DFrameTween` | `TweenFrame` | int |
| AnimatedSprite3D | `AnimatedSprite3DSpeedScaleTween` | `TweenSpeedScale` | float |
| AnimationPlayer | `AnimationPlayerSpeedScaleTween` | `TweenSpeedScale` | float |
| AudioStreamPlayer | `AudioPitchScaleTween` | `TweenPitchScale` | float |
| AudioStreamPlayer | `AudioVolumeDbTween` | `TweenVolumeDb` | float |
| AudioStreamPlayer | `AudioVolumeLinearTween` | `TweenVolumeLinear` | float |
| AudioStreamPlayer2D | `AudioStreamPlayer2DAttenuationTween` | `TweenAttenuation` | float |
| AudioStreamPlayer2D | `AudioStreamPlayer2DMaxDistanceTween` | `TweenMaxDistance` | float |
| AudioStreamPlayer2D | `AudioStreamPlayer2DPanningStrengthTween` | `TweenPanningStrength` | float |
| AudioStreamPlayer2D | `AudioPitchScale2DTween` | `TweenPitchScale` | float |
| AudioStreamPlayer2D | `AudioVolumeDb2DTween` | `TweenVolumeDb` | float |
| AudioStreamPlayer2D | `AudioVolumeLinear2DTween` | `TweenVolumeLinear` | float |
| AudioStreamPlayer3D | `AudioStreamPlayer3DAttenuationFilterCutoffHzTween` | `TweenAttenuationFilterCutoffHz` | float |
| AudioStreamPlayer3D | `AudioStreamPlayer3DAttenuationFilterDbTween` | `TweenAttenuationFilterDb` | float |
| AudioStreamPlayer3D | `AudioStreamPlayer3DEmissionAngleDegreesTween` | `TweenEmissionAngleDegrees` | float |
| AudioStreamPlayer3D | `AudioStreamPlayer3DEmissionAngleFilterAttenuationDbTween` | `TweenEmissionAngleFilterAttenuationDb` | float |
| AudioStreamPlayer3D | `AudioStreamPlayer3DMaxDistanceTween` | `TweenMaxDistance` | float |
| AudioStreamPlayer3D | `AudioStreamPlayer3DPanningStrengthTween` | `TweenPanningStrength` | float |
| AudioStreamPlayer3D | `AudioPitchScale3DTween` | `TweenPitchScale` | float |
| AudioStreamPlayer3D | `AudioStreamPlayer3DUnitSizeTween` | `TweenUnitSize` | float |
| AudioStreamPlayer3D | `AudioVolumeDb3DTween` | `TweenVolumeDb` | float |
| AudioStreamPlayer3D | `AudioVolumeLinear3DTween` | `TweenVolumeLinear` | float |
| Camera2D | `Camera2DOffsetTween` | `TweenOffset` | Vector2 |
| Camera2D | `Camera2DOffsetXTween` | `TweenOffsetX` | float |
| Camera2D | `Camera2DOffsetYTween` | `TweenOffsetY` | float |
| Camera2D | `Camera2DZoomTween` | `TweenZoom` | Vector2 |
| Camera2D | `Camera2DZoomXTween` | `TweenZoomX` | float |
| Camera2D | `Camera2DZoomYTween` | `TweenZoomY` | float |
| Camera3D | `Camera3DFarTween` | `TweenFar` | float |
| Camera3D | `Camera3DFovTween` | `TweenFov` | float |
| Camera3D | `Camera3DFrustumOffsetTween` | `TweenFrustumOffset` | Vector2 |
| Camera3D | `Camera3DFrustumOffsetXTween` | `TweenFrustumOffsetX` | float |
| Camera3D | `Camera3DFrustumOffsetYTween` | `TweenFrustumOffsetY` | float |
| Camera3D | `Camera3DHOffsetTween` | `TweenHOffset` | float |
| Camera3D | `Camera3DNearTween` | `TweenNear` | float |
| Camera3D | `Camera3DSizeTween` | `TweenSize` | float |
| Camera3D | `Camera3DVOffsetTween` | `TweenVOffset` | float |
| CanvasItem | `ModulateTween` | `TweenModulate` | Color |
| CanvasItem | `ModulateAlphaTween` | `TweenModulateAlpha` | float |
| CanvasItem | `SelfModulateTween` | `TweenSelfModulate` | Color |
| CanvasItem | `SelfModulateAlphaTween` | `TweenSelfModulateAlpha` | float |
| CanvasLayer | `CanvasLayerOffsetTween` | `TweenOffset` | Vector2 |
| CanvasLayer | `CanvasLayerOffsetXTween` | `TweenOffsetX` | float |
| CanvasLayer | `CanvasLayerOffsetYTween` | `TweenOffsetY` | float |
| CanvasLayer | `CanvasLayerRotationTween` | `TweenRotation` | float |
| CanvasLayer | `CanvasLayerScaleTween` | `TweenScale` | Vector2 |
| CanvasLayer | `CanvasLayerScaleXTween` | `TweenScaleX` | float |
| CanvasLayer | `CanvasLayerScaleYTween` | `TweenScaleY` | float |
| CanvasModulate | `CanvasModulateColorTween` | `TweenColor` | Color |
| CanvasModulate | `CanvasModulateColorAlphaTween` | `TweenColorAlpha` | float |
| ColorRect | `ColorRectColorTween` | `TweenColor` | Color |
| ColorRect | `ColorRectColorAlphaTween` | `TweenColorAlpha` | float |
| Control | `ControlAnchorBottomTween` | `TweenAnchorBottom` | float |
| Control | `ControlAnchorLeftTween` | `TweenAnchorLeft` | float |
| Control | `ControlAnchorMaxTween` | `TweenAnchorMax` | Vector2 |
| Control | `ControlAnchorMinTween` | `TweenAnchorMin` | Vector2 |
| Control | `ControlAnchorRightTween` | `TweenAnchorRight` | float |
| Control | `ControlAnchorTopTween` | `TweenAnchorTop` | float |
| Control | `ControlCustomMaximumSizeTween` | `TweenCustomMaximumSize` | Vector2 |
| Control | `ControlCustomMaximumSizeXTween` | `TweenCustomMaximumSizeX` | float |
| Control | `ControlCustomMaximumSizeYTween` | `TweenCustomMaximumSizeY` | float |
| Control | `ControlCustomMinimumSizeTween` | `TweenCustomMinimumSize` | Vector2 |
| Control | `ControlCustomMinimumSizeXTween` | `TweenCustomMinimumSizeX` | float |
| Control | `ControlCustomMinimumSizeYTween` | `TweenCustomMinimumSizeY` | float |
| Control | `ControlGlobalPositionTween` | `TweenGlobalPosition` | Vector2 |
| Control | `ControlGlobalPositionXTween` | `TweenGlobalPositionX` | float |
| Control | `ControlGlobalPositionYTween` | `TweenGlobalPositionY` | float |
| Control | `ControlOffsetBottomTween` | `TweenOffsetBottom` | float |
| Control | `ControlOffsetLeftTween` | `TweenOffsetLeft` | float |
| Control | `ControlOffsetRightTween` | `TweenOffsetRight` | float |
| Control | `ControlOffsetsTween` | `TweenOffsets` | Vector4 |
| Control | `ControlOffsetTopTween` | `TweenOffsetTop` | float |
| Control | `ControlOffsetTransformPivotTween` | `TweenOffsetTransformPivot` | Vector2 |
| Control | `ControlOffsetTransformPivotRatioTween` | `TweenOffsetTransformPivotRatio` | Vector2 |
| Control | `ControlOffsetTransformPivotRatioXTween` | `TweenOffsetTransformPivotRatioX` | float |
| Control | `ControlOffsetTransformPivotRatioYTween` | `TweenOffsetTransformPivotRatioY` | float |
| Control | `ControlOffsetTransformPivotXTween` | `TweenOffsetTransformPivotX` | float |
| Control | `ControlOffsetTransformPivotYTween` | `TweenOffsetTransformPivotY` | float |
| Control | `ControlOffsetTransformPositionTween` | `TweenOffsetTransformPosition` | Vector2 |
| Control | `ControlOffsetTransformPositionRatioTween` | `TweenOffsetTransformPositionRatio` | Vector2 |
| Control | `ControlOffsetTransformPositionRatioXTween` | `TweenOffsetTransformPositionRatioX` | float |
| Control | `ControlOffsetTransformPositionRatioYTween` | `TweenOffsetTransformPositionRatioY` | float |
| Control | `ControlOffsetTransformPositionXTween` | `TweenOffsetTransformPositionX` | float |
| Control | `ControlOffsetTransformPositionYTween` | `TweenOffsetTransformPositionY` | float |
| Control | `ControlOffsetTransformRotationTween` | `TweenOffsetTransformRotation` | float |
| Control | `ControlOffsetTransformScaleTween` | `TweenOffsetTransformScale` | Vector2 |
| Control | `ControlOffsetTransformScaleXTween` | `TweenOffsetTransformScaleX` | float |
| Control | `ControlOffsetTransformScaleYTween` | `TweenOffsetTransformScaleY` | float |
| Control | `ControlPivotOffsetTween` | `TweenPivotOffset` | Vector2 |
| Control | `ControlPivotOffsetRatioTween` | `TweenPivotOffsetRatio` | Vector2 |
| Control | `ControlPivotOffsetRatioXTween` | `TweenPivotOffsetRatioX` | float |
| Control | `ControlPivotOffsetRatioYTween` | `TweenPivotOffsetRatioY` | float |
| Control | `ControlPivotOffsetXTween` | `TweenPivotOffsetX` | float |
| Control | `ControlPivotOffsetYTween` | `TweenPivotOffsetY` | float |
| Control | `ControlPositionTween` | `TweenPosition` | Vector2 |
| Control | `ControlPositionXTween` | `TweenPositionX` | float |
| Control | `ControlPositionYTween` | `TweenPositionY` | float |
| Control | `ControlRotationTween` | `TweenRotation` | float |
| Control | `ControlScaleTween` | `TweenScale` | Vector2 |
| Control | `ControlScaleXTween` | `TweenScaleX` | float |
| Control | `ControlScaleYTween` | `TweenScaleY` | float |
| Control | `ControlSizeTween` | `TweenSize` | Vector2 |
| Control | `ControlSizeFlagsStretchRatioTween` | `TweenSizeFlagsStretchRatio` | float |
| Control | `ControlSizeXTween` | `TweenSizeX` | float |
| Control | `ControlSizeYTween` | `TweenSizeY` | float |
| CpuParticles2D | `CpuParticles2DColorTween` | `TweenColor` | Color |
| CpuParticles2D | `CpuParticles2DColorAlphaTween` | `TweenColorAlpha` | float |
| CpuParticles2D | `CpuParticles2DDirectionTween` | `TweenDirection` | Vector2 |
| CpuParticles2D | `CpuParticles2DDirectionXTween` | `TweenDirectionX` | float |
| CpuParticles2D | `CpuParticles2DDirectionYTween` | `TweenDirectionY` | float |
| CpuParticles2D | `CpuParticles2DEmissionRectExtentsTween` | `TweenEmissionRectExtents` | Vector2 |
| CpuParticles2D | `CpuParticles2DEmissionRectExtentsXTween` | `TweenEmissionRectExtentsX` | float |
| CpuParticles2D | `CpuParticles2DEmissionRectExtentsYTween` | `TweenEmissionRectExtentsY` | float |
| CpuParticles2D | `CpuParticles2DEmissionSphereRadiusTween` | `TweenEmissionSphereRadius` | float |
| CpuParticles2D | `CpuParticles2DExplosivenessTween` | `TweenExplosiveness` | float |
| CpuParticles2D | `CpuParticles2DGravityTween` | `TweenGravity` | Vector2 |
| CpuParticles2D | `CpuParticles2DGravityXTween` | `TweenGravityX` | float |
| CpuParticles2D | `CpuParticles2DGravityYTween` | `TweenGravityY` | float |
| CpuParticles2D | `CpuParticles2DLifetimeTween` | `TweenLifetime` | double |
| CpuParticles2D | `CpuParticles2DRandomnessTween` | `TweenRandomness` | float |
| CpuParticles2D | `CpuParticles2DSpeedScaleTween` | `TweenSpeedScale` | double |
| CpuParticles2D | `CpuParticles2DSpreadTween` | `TweenSpread` | float |
| CpuParticles3D | `CpuParticles3DColorTween` | `TweenColor` | Color |
| CpuParticles3D | `CpuParticles3DColorAlphaTween` | `TweenColorAlpha` | float |
| CpuParticles3D | `CpuParticles3DDirectionTween` | `TweenDirection` | Vector3 |
| CpuParticles3D | `CpuParticles3DDirectionXTween` | `TweenDirectionX` | float |
| CpuParticles3D | `CpuParticles3DDirectionYTween` | `TweenDirectionY` | float |
| CpuParticles3D | `CpuParticles3DDirectionZTween` | `TweenDirectionZ` | float |
| CpuParticles3D | `CpuParticles3DEmissionBoxExtentsTween` | `TweenEmissionBoxExtents` | Vector3 |
| CpuParticles3D | `CpuParticles3DEmissionBoxExtentsXTween` | `TweenEmissionBoxExtentsX` | float |
| CpuParticles3D | `CpuParticles3DEmissionBoxExtentsYTween` | `TweenEmissionBoxExtentsY` | float |
| CpuParticles3D | `CpuParticles3DEmissionBoxExtentsZTween` | `TweenEmissionBoxExtentsZ` | float |
| CpuParticles3D | `CpuParticles3DEmissionSphereRadiusTween` | `TweenEmissionSphereRadius` | float |
| CpuParticles3D | `CpuParticles3DExplosivenessTween` | `TweenExplosiveness` | float |
| CpuParticles3D | `CpuParticles3DGravityTween` | `TweenGravity` | Vector3 |
| CpuParticles3D | `CpuParticles3DGravityXTween` | `TweenGravityX` | float |
| CpuParticles3D | `CpuParticles3DGravityYTween` | `TweenGravityY` | float |
| CpuParticles3D | `CpuParticles3DGravityZTween` | `TweenGravityZ` | float |
| CpuParticles3D | `CpuParticles3DLifetimeTween` | `TweenLifetime` | double |
| CpuParticles3D | `CpuParticles3DRandomnessTween` | `TweenRandomness` | float |
| CpuParticles3D | `CpuParticles3DSpeedScaleTween` | `TweenSpeedScale` | double |
| CpuParticles3D | `CpuParticles3DSpreadTween` | `TweenSpread` | float |
| Decal | `DecalEmissionEnergyTween` | `TweenEmissionEnergy` | float |
| Decal | `DecalModulateTween` | `TweenModulate` | Color |
| Decal | `DecalModulateAlphaTween` | `TweenModulateAlpha` | float |
| Decal | `DecalSizeTween` | `TweenSize` | Vector3 |
| Decal | `DecalSizeXTween` | `TweenSizeX` | float |
| Decal | `DecalSizeYTween` | `TweenSizeY` | float |
| Decal | `DecalSizeZTween` | `TweenSizeZ` | float |
| FogVolume | `FogVolumeSizeTween` | `TweenSize` | Vector3 |
| FogVolume | `FogVolumeSizeXTween` | `TweenSizeX` | float |
| FogVolume | `FogVolumeSizeYTween` | `TweenSizeY` | float |
| FogVolume | `FogVolumeSizeZTween` | `TweenSizeZ` | float |
| GeometryInstance3D | `GeometryInstance3DTransparencyTween` | `TweenTransparency` | float |
| Godot.Range | `RangeValueTween` | `TweenValue` | double |
| GpuParticles2D | `GpuParticles2DAmountRatioTween` | `TweenAmountRatio` | float |
| GpuParticles2D | `GpuParticles2DExplosivenessTween` | `TweenExplosiveness` | float |
| GpuParticles2D | `GpuParticles2DLifetimeTween` | `TweenLifetime` | double |
| GpuParticles2D | `GpuParticles2DRandomnessTween` | `TweenRandomness` | float |
| GpuParticles2D | `GpuParticles2DSpeedScaleTween` | `TweenSpeedScale` | double |
| GpuParticles3D | `GpuParticles3DAmountRatioTween` | `TweenAmountRatio` | float |
| GpuParticles3D | `GpuParticles3DExplosivenessTween` | `TweenExplosiveness` | float |
| GpuParticles3D | `GpuParticles3DLifetimeTween` | `TweenLifetime` | double |
| GpuParticles3D | `GpuParticles3DRandomnessTween` | `TweenRandomness` | float |
| GpuParticles3D | `GpuParticles3DSpeedScaleTween` | `TweenSpeedScale` | double |
| Label | `LabelVisibleCharactersTween` | `TweenVisibleCharacters` | int |
| Label | `LabelVisibleRatioTween` | `TweenVisibleRatio` | float |
| Label3D | `Label3DModulateTween` | `TweenModulate` | Color |
| Label3D | `Label3DModulateAlphaTween` | `TweenModulateAlpha` | float |
| Label3D | `Label3DOffsetTween` | `TweenOffset` | Vector2 |
| Label3D | `Label3DOffsetXTween` | `TweenOffsetX` | float |
| Label3D | `Label3DOffsetYTween` | `TweenOffsetY` | float |
| Label3D | `Label3DOutlineModulateTween` | `TweenOutlineModulate` | Color |
| Label3D | `Label3DOutlineModulateAlphaTween` | `TweenOutlineModulateAlpha` | float |
| Label3D | `Label3DPixelSizeTween` | `TweenPixelSize` | float |
| Light2D | `LightColor2DTween` | `TweenColor` | Color |
| Light2D | `LightEnergy2DTween` | `TweenEnergy` | float |
| Light2D | `Light2DShadowColorTween` | `TweenShadowColor` | Color |
| Light2D | `Light2DShadowColorAlphaTween` | `TweenShadowColorAlpha` | float |
| Light3D | `LightColor3DTween` | `TweenLightColor` | Color |
| Light3D | `LightEnergy3DTween` | `TweenLightEnergy` | float |
| Light3D | `Light3DLightIndirectEnergyTween` | `TweenLightIndirectEnergy` | float |
| Light3D | `Light3DLightTemperatureTween` | `TweenLightTemperature` | float |
| Light3D | `Light3DLightVolumetricFogEnergyTween` | `TweenLightVolumetricFogEnergy` | float |
| Light3D | `Light3DShadowOpacityTween` | `TweenShadowOpacity` | float |
| Line2D | `Line2DDefaultColorTween` | `TweenDefaultColor` | Color |
| Line2D | `Line2DDefaultColorAlphaTween` | `TweenDefaultColorAlpha` | float |
| Line2D | `Line2DWidthTween` | `TweenWidth` | float |
| Node | `ColorTween` | `TweenColor` | Color |
| Node | `DoubleTween` | `TweenDouble` | double |
| Node | `FloatTween` | `TweenFloat` | float |
| Node | `QuaternionTween` | `TweenQuaternion` | Quaternion |
| Node | `Rect2Tween` | `TweenRect2` | Rect2 |
| Node | `Vector2Tween` | `TweenVector2` | Vector2 |
| Node | `Vector3Tween` | `TweenVector3` | Vector3 |
| Node | `Vector4Tween` | `TweenVector4` | Vector4 |
| Node2D | `GlobalPosition2DTween` | `TweenGlobalPosition` | Vector2 |
| Node2D | `GlobalPosition2DXTween` | `TweenGlobalPositionX` | float |
| Node2D | `GlobalPosition2DYTween` | `TweenGlobalPositionY` | float |
| Node2D | `GlobalRotation2DTween` | `TweenGlobalRotation` | float |
| Node2D | `GlobalScale2DTween` | `TweenGlobalScale` | Vector2 |
| Node2D | `GlobalScale2DXTween` | `TweenGlobalScaleX` | float |
| Node2D | `GlobalScale2DYTween` | `TweenGlobalScaleY` | float |
| Node2D | `GlobalSkew2DTween` | `TweenGlobalSkew` | float |
| Node2D | `Position2DTween` | `TweenPosition` | Vector2 |
| Node2D | `Position2DXTween` | `TweenPositionX` | float |
| Node2D | `Position2DYTween` | `TweenPositionY` | float |
| Node2D | `Rotation2DTween` | `TweenRotation` | float |
| Node2D | `Scale2DTween` | `TweenScale` | Vector2 |
| Node2D | `Scale2DXTween` | `TweenScaleX` | float |
| Node2D | `Scale2DYTween` | `TweenScaleY` | float |
| Node2D | `Skew2DTween` | `TweenSkew` | float |
| Node3D | `GlobalPosition3DTween` | `TweenGlobalPosition` | Vector3 |
| Node3D | `GlobalPosition3DXTween` | `TweenGlobalPositionX` | float |
| Node3D | `GlobalPosition3DYTween` | `TweenGlobalPositionY` | float |
| Node3D | `GlobalPosition3DZTween` | `TweenGlobalPositionZ` | float |
| Node3D | `GlobalQuaternion3DTween` | `TweenGlobalQuaternion` | Quaternion |
| Node3D | `GlobalRotation3DTween` | `TweenGlobalRotation` | Vector3 |
| Node3D | `GlobalRotation3DXTween` | `TweenGlobalRotationX` | float |
| Node3D | `GlobalRotation3DYTween` | `TweenGlobalRotationY` | float |
| Node3D | `GlobalRotation3DZTween` | `TweenGlobalRotationZ` | float |
| Node3D | `Position3DTween` | `TweenPosition` | Vector3 |
| Node3D | `Position3DXTween` | `TweenPositionX` | float |
| Node3D | `Position3DYTween` | `TweenPositionY` | float |
| Node3D | `Position3DZTween` | `TweenPositionZ` | float |
| Node3D | `Quaternion3DTween` | `TweenQuaternion` | Quaternion |
| Node3D | `Rotation3DTween` | `TweenRotation` | Vector3 |
| Node3D | `Rotation3DXTween` | `TweenRotationX` | float |
| Node3D | `Rotation3DYTween` | `TweenRotationY` | float |
| Node3D | `Rotation3DZTween` | `TweenRotationZ` | float |
| Node3D | `Scale3DTween` | `TweenScale` | Vector3 |
| Node3D | `Scale3DXTween` | `TweenScaleX` | float |
| Node3D | `Scale3DYTween` | `TweenScaleY` | float |
| Node3D | `Scale3DZTween` | `TweenScaleZ` | float |
| OmniLight3D | `OmniLight3DOmniAttenuationTween` | `TweenOmniAttenuation` | float |
| OmniLight3D | `OmniRangeTween` | `TweenOmniRange` | float |
| Parallax2D | `Parallax2DAutoscrollTween` | `TweenAutoscroll` | Vector2 |
| Parallax2D | `Parallax2DAutoscrollXTween` | `TweenAutoscrollX` | float |
| Parallax2D | `Parallax2DAutoscrollYTween` | `TweenAutoscrollY` | float |
| Parallax2D | `Parallax2DScrollOffsetTween` | `TweenScrollOffset` | Vector2 |
| Parallax2D | `Parallax2DScrollOffsetXTween` | `TweenScrollOffsetX` | float |
| Parallax2D | `Parallax2DScrollOffsetYTween` | `TweenScrollOffsetY` | float |
| Parallax2D | `Parallax2DScrollScaleTween` | `TweenScrollScale` | Vector2 |
| Parallax2D | `Parallax2DScrollScaleXTween` | `TweenScrollScaleX` | float |
| Parallax2D | `Parallax2DScrollScaleYTween` | `TweenScrollScaleY` | float |
| PathFollow2D | `PathFollow2DHOffsetTween` | `TweenHOffset` | float |
| PathFollow2D | `PathFollow2DProgressTween` | `TweenProgress` | float |
| PathFollow2D | `PathFollow2DProgressRatioTween` | `TweenProgressRatio` | float |
| PathFollow2D | `PathFollow2DVOffsetTween` | `TweenVOffset` | float |
| PathFollow3D | `PathFollow3DHOffsetTween` | `TweenHOffset` | float |
| PathFollow3D | `PathFollow3DProgressTween` | `TweenProgress` | float |
| PathFollow3D | `PathFollow3DProgressRatioTween` | `TweenProgressRatio` | float |
| PathFollow3D | `PathFollow3DVOffsetTween` | `TweenVOffset` | float |
| PointLight2D | `PointLight2DHeightTween` | `TweenHeight` | float |
| PointLight2D | `PointLight2DOffsetTween` | `TweenOffset` | Vector2 |
| PointLight2D | `PointLight2DOffsetXTween` | `TweenOffsetX` | float |
| PointLight2D | `PointLight2DOffsetYTween` | `TweenOffsetY` | float |
| PointLight2D | `PointLight2DTextureScaleTween` | `TweenTextureScale` | float |
| Polygon2D | `Polygon2DColorTween` | `TweenColor` | Color |
| Polygon2D | `Polygon2DColorAlphaTween` | `TweenColorAlpha` | float |
| Polygon2D | `Polygon2DOffsetTween` | `TweenOffset` | Vector2 |
| Polygon2D | `Polygon2DOffsetXTween` | `TweenOffsetX` | float |
| Polygon2D | `Polygon2DOffsetYTween` | `TweenOffsetY` | float |
| Polygon2D | `Polygon2DTextureOffsetTween` | `TweenTextureOffset` | Vector2 |
| Polygon2D | `Polygon2DTextureOffsetXTween` | `TweenTextureOffsetX` | float |
| Polygon2D | `Polygon2DTextureOffsetYTween` | `TweenTextureOffsetY` | float |
| Polygon2D | `Polygon2DTextureRotationTween` | `TweenTextureRotation` | float |
| Polygon2D | `Polygon2DTextureScaleTween` | `TweenTextureScale` | Vector2 |
| Polygon2D | `Polygon2DTextureScaleXTween` | `TweenTextureScaleX` | float |
| Polygon2D | `Polygon2DTextureScaleYTween` | `TweenTextureScaleY` | float |
| RichTextLabel | `RichTextLabelVisibleCharactersTween` | `TweenVisibleCharacters` | int |
| RichTextLabel | `RichTextLabelVisibleRatioTween` | `TweenVisibleRatio` | float |
| ScrollContainer | `ScrollContainerScrollHorizontalTween` | `TweenScrollHorizontal` | int |
| ScrollContainer | `ScrollContainerScrollVerticalTween` | `TweenScrollVertical` | int |
| SpotLight3D | `SpotAngleTween` | `TweenSpotAngle` | float |
| SpotLight3D | `SpotLight3DSpotAngleAttenuationTween` | `TweenSpotAngleAttenuation` | float |
| SpotLight3D | `SpotLight3DSpotAttenuationTween` | `TweenSpotAttenuation` | float |
| SpotLight3D | `SpotRangeTween` | `TweenSpotRange` | float |
| SpringArm3D | `SpringArm3DSpringLengthTween` | `TweenSpringLength` | float |
| Sprite2D | `Sprite2DFrameTween` | `TweenFrame` | int |
| Sprite2D | `Sprite2DOffsetTween` | `TweenOffset` | Vector2 |
| Sprite2D | `Sprite2DOffsetXTween` | `TweenOffsetX` | float |
| Sprite2D | `Sprite2DOffsetYTween` | `TweenOffsetY` | float |
| Sprite2D | `Sprite2DRegionRectTween` | `TweenRegionRect` | Rect2 |
| SpriteBase3D | `SpriteBase3DModulateTween` | `TweenModulate` | Color |
| SpriteBase3D | `SpriteBase3DModulateAlphaTween` | `TweenModulateAlpha` | float |
| SpriteBase3D | `SpriteBase3DOffsetTween` | `TweenOffset` | Vector2 |
| SpriteBase3D | `SpriteBase3DOffsetXTween` | `TweenOffsetX` | float |
| SpriteBase3D | `SpriteBase3DOffsetYTween` | `TweenOffsetY` | float |
| SpriteBase3D | `SpriteBase3DPixelSizeTween` | `TweenPixelSize` | float |
| TextureProgressBar | `TextureProgressBarRadialCenterOffsetTween` | `TweenRadialCenterOffset` | Vector2 |
| TextureProgressBar | `TextureProgressBarRadialCenterOffsetXTween` | `TweenRadialCenterOffsetX` | float |
| TextureProgressBar | `TextureProgressBarRadialCenterOffsetYTween` | `TweenRadialCenterOffsetY` | float |
| TextureProgressBar | `TextureProgressBarRadialFillDegreesTween` | `TweenRadialFillDegrees` | float |
| TextureProgressBar | `TextureProgressBarRadialInitialAngleTween` | `TweenRadialInitialAngle` | float |
| TextureProgressBar | `TextureProgressBarTextureProgressOffsetTween` | `TweenTextureProgressOffset` | Vector2 |
| TextureProgressBar | `TextureProgressBarTextureProgressOffsetXTween` | `TweenTextureProgressOffsetX` | float |
| TextureProgressBar | `TextureProgressBarTextureProgressOffsetYTween` | `TweenTextureProgressOffsetY` | float |
| TextureProgressBar | `TextureProgressBarTintOverTween` | `TweenTintOver` | Color |
| TextureProgressBar | `TextureProgressBarTintOverAlphaTween` | `TweenTintOverAlpha` | float |
| TextureProgressBar | `TextureProgressBarTintProgressTween` | `TweenTintProgress` | Color |
| TextureProgressBar | `TextureProgressBarTintProgressAlphaTween` | `TweenTintProgressAlpha` | float |
| TextureProgressBar | `TextureProgressBarTintUnderTween` | `TweenTintUnder` | Color |
| TextureProgressBar | `TextureProgressBarTintUnderAlphaTween` | `TweenTintUnderAlpha` | float |

## Verification

ExpandedAdapterTests checks node properties independently through the Godot API, including nonuniform component samples, initial capture, midpoint/end values, omitted endpoints, fill restoration, cancellation and the automatic convenience route. NodePropertyBehaviorTests covers transformed parents, quaternion scale preservation, composable components, container offset transforms, path motion, owner lifetime, engine-pumped completion, value helpers and catalog completeness. IntegerInterpolationTests covers signed midpoint rounding, range saturation and invalid weights. Special paired-anchor/offset adapters retain dedicated layout tests.

Node-expansion validation before the material additions, on 2026-09-23 with 2dog 4.7.2.87: 430 Release tests passed; the Release solution build succeeded. Coverage of handwritten library source is 98.6% of lines and 84.2% of branches (Godot-generated sources excluded). The adapter and convenience-extension source directories each have 100% line coverage. Coverage is reported separately from behavior: remaining core/runtime branches are follow-up work, and headless tests do not certify visual output on every renderer. Local reports are under `artifacts/node-tweens-tests/`.
