---
title: Node and value catalog
description: Typed definitions, convenience methods, units, and Godot property constraints.
---

The node/value catalog provides 306 built-in definitions (298 node/property definitions and 8 callback value definitions). Each has a typed convenience extension. Adapters use Godot properties directly; no runtime reflection or string property paths are used.

## Usage

The example assumes `using Godot;` and `using tweens.gd;`.

```csharp
// sprite: Sprite2D, camera: Camera2D, label: Label; all inside the tree.
var movement = sprite.TweenPosition(new Vector2(300, 120), 0.5,
    options => options.Ease = EaseType.CubicOut);
var fade = sprite.TweenModulateAlpha(0, 0.2);
var zoom = camera.TweenZoom(new Vector2(2, 2), 0.4);
var reveal = label.TweenVisibleRatio(1, 1.5, options => options.From = 0);
await Group.Of(movement, fade).End;
```

Signatures are `target.TweenProperty(to, duration, configure = null)`. The optional
typed configure callback runs synchronously before playback starts and may override
`From`, `To`, `Duration`, or any other definition setting, including callbacks.
Configuration errors propagate before playback is added. Keep using
`target.Tween(new Definition { ... })` for reusable definitions. Both forms return a
`TweenInstance<TTarget, TValue>` handle with pause, cancel, and completion support,
and the same [owner lifetime](/concepts/lifetime/).

Base-class extensions apply to derived nodes: `Node2D` and `Node3D` provide
transforms, `CanvasItem` provides 2D modulation, `Control` provides layout,
`SpriteBase3D` provides 3D sprite appearance, `GeometryInstance3D` provides
transparency, and `Range` provides `Value`. Specific overloads take precedence over
the generic `Node` callback value helpers: a node with a color property uses its
property-specific `TweenColor` overload. Use an explicit value definition when that
distinction matters.

Units, value types, and the Godot constraints that still apply are listed under
[units and engine constraints](#units-and-engine-constraints), after the catalog.

## Definitions and extensions

- 2D: [CanvasItem](#canvasitem) · [Node2D](#node2d) · [AnimatedSprite2D](#animatedsprite2d) · [Camera2D](#camera2d) · [CanvasLayer](#canvaslayer) · [CanvasModulate](#canvasmodulate) · [CpuParticles2D](#cpuparticles2d) · [GpuParticles2D](#gpuparticles2d) · [Light2D](#light2d) · [Line2D](#line2d) · [Parallax2D](#parallax2d) · [PathFollow2D](#pathfollow2d) · [PointLight2D](#pointlight2d) · [Polygon2D](#polygon2d) · [Sprite2D](#sprite2d)
- 3D: [Node3D](#node3d) · [GeometryInstance3D](#geometryinstance3d) · [SpriteBase3D](#spritebase3d) · [AnimatedSprite3D](#animatedsprite3d) · [Camera3D](#camera3d) · [CpuParticles3D](#cpuparticles3d) · [Decal](#decal) · [FogVolume](#fogvolume) · [GpuParticles3D](#gpuparticles3d) · [Label3D](#label3d) · [Light3D](#light3d) · [OmniLight3D](#omnilight3d) · [PathFollow3D](#pathfollow3d) · [SpotLight3D](#spotlight3d) · [SpringArm3D](#springarm3d)
- UI: [Control](#control) · [Range](#range) · [ColorRect](#colorrect) · [Label](#label) · [RichTextLabel](#richtextlabel) · [ScrollContainer](#scrollcontainer) · [TextureProgressBar](#textureprogressbar)
- Animation and audio: [AnimationPlayer](#animationplayer) · [AudioStreamPlayer](#audiostreamplayer) · [AudioStreamPlayer2D](#audiostreamplayer2d) · [AudioStreamPlayer3D](#audiostreamplayer3d)
- Any node: [Callback values](#callback-values)

Each row is a supported definition and its convenience method. Names ending in X, Y,
Z, or Alpha affect one component. Each method takes the target value, a duration,
and either an optional configure callback or a `TweenOptions` value.

### CanvasItem

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.Modulate` | `TweenModulate` | Color |
| `Tweens.ModulateAlpha` | `TweenModulateAlpha` | float |
| `Tweens.SelfModulate` | `TweenSelfModulate` | Color |
| `Tweens.SelfModulateAlpha` | `TweenSelfModulateAlpha` | float |

### Node2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.GlobalPosition2D` | `TweenGlobalPosition` | Vector2 |
| `Tweens.GlobalPosition2DX` | `TweenGlobalPositionX` | float |
| `Tweens.GlobalPosition2DY` | `TweenGlobalPositionY` | float |
| `Tweens.GlobalRotation2D` | `TweenGlobalRotation` | float |
| `Tweens.GlobalScale2D` | `TweenGlobalScale` | Vector2 |
| `Tweens.GlobalScale2DX` | `TweenGlobalScaleX` | float |
| `Tweens.GlobalScale2DY` | `TweenGlobalScaleY` | float |
| `Tweens.GlobalSkew2D` | `TweenGlobalSkew` | float |
| `Tweens.Position2D` | `TweenPosition` | Vector2 |
| `Tweens.Position2DX` | `TweenPositionX` | float |
| `Tweens.Position2DY` | `TweenPositionY` | float |
| `Tweens.Rotation2D` | `TweenRotation` | float |
| `Tweens.Scale2D` | `TweenScale` | Vector2 |
| `Tweens.Scale2DX` | `TweenScaleX` | float |
| `Tweens.Scale2DY` | `TweenScaleY` | float |
| `Tweens.Skew2D` | `TweenSkew` | float |

### AnimatedSprite2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.AnimatedSprite2DFrame` | `TweenFrame` | int |
| `Tweens.AnimatedSprite2DSpeedScale` | `TweenSpeedScale` | float |

### Camera2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.Camera2DOffset` | `TweenOffset` | Vector2 |
| `Tweens.Camera2DOffsetX` | `TweenOffsetX` | float |
| `Tweens.Camera2DOffsetY` | `TweenOffsetY` | float |
| `Tweens.Camera2DZoom` | `TweenZoom` | Vector2 |
| `Tweens.Camera2DZoomX` | `TweenZoomX` | float |
| `Tweens.Camera2DZoomY` | `TweenZoomY` | float |

### CanvasLayer

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.CanvasLayerOffset` | `TweenOffset` | Vector2 |
| `Tweens.CanvasLayerOffsetX` | `TweenOffsetX` | float |
| `Tweens.CanvasLayerOffsetY` | `TweenOffsetY` | float |
| `Tweens.CanvasLayerRotation` | `TweenRotation` | float |
| `Tweens.CanvasLayerScale` | `TweenScale` | Vector2 |
| `Tweens.CanvasLayerScaleX` | `TweenScaleX` | float |
| `Tweens.CanvasLayerScaleY` | `TweenScaleY` | float |

### CanvasModulate

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.CanvasModulateColor` | `TweenColor` | Color |
| `Tweens.CanvasModulateColorAlpha` | `TweenColorAlpha` | float |

### CpuParticles2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.CpuParticles2DColor` | `TweenColor` | Color |
| `Tweens.CpuParticles2DColorAlpha` | `TweenColorAlpha` | float |
| `Tweens.CpuParticles2DDirection` | `TweenDirection` | Vector2 |
| `Tweens.CpuParticles2DDirectionX` | `TweenDirectionX` | float |
| `Tweens.CpuParticles2DDirectionY` | `TweenDirectionY` | float |
| `Tweens.CpuParticles2DEmissionRectExtents` | `TweenEmissionRectExtents` | Vector2 |
| `Tweens.CpuParticles2DEmissionRectExtentsX` | `TweenEmissionRectExtentsX` | float |
| `Tweens.CpuParticles2DEmissionRectExtentsY` | `TweenEmissionRectExtentsY` | float |
| `Tweens.CpuParticles2DEmissionSphereRadius` | `TweenEmissionSphereRadius` | float |
| `Tweens.CpuParticles2DExplosiveness` | `TweenExplosiveness` | float |
| `Tweens.CpuParticles2DGravity` | `TweenGravity` | Vector2 |
| `Tweens.CpuParticles2DGravityX` | `TweenGravityX` | float |
| `Tweens.CpuParticles2DGravityY` | `TweenGravityY` | float |
| `Tweens.CpuParticles2DLifetime` | `TweenLifetime` | double |
| `Tweens.CpuParticles2DRandomness` | `TweenRandomness` | float |
| `Tweens.CpuParticles2DSpeedScale` | `TweenSpeedScale` | double |
| `Tweens.CpuParticles2DSpread` | `TweenSpread` | float |

### GpuParticles2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.GpuParticles2DAmountRatio` | `TweenAmountRatio` | float |
| `Tweens.GpuParticles2DExplosiveness` | `TweenExplosiveness` | float |
| `Tweens.GpuParticles2DLifetime` | `TweenLifetime` | double |
| `Tweens.GpuParticles2DRandomness` | `TweenRandomness` | float |
| `Tweens.GpuParticles2DSpeedScale` | `TweenSpeedScale` | double |

### Light2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.LightColor2D` | `TweenColor` | Color |
| `Tweens.LightEnergy2D` | `TweenEnergy` | float |
| `Tweens.Light2DShadowColor` | `TweenShadowColor` | Color |
| `Tweens.Light2DShadowColorAlpha` | `TweenShadowColorAlpha` | float |

### Line2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.Line2DDefaultColor` | `TweenDefaultColor` | Color |
| `Tweens.Line2DDefaultColorAlpha` | `TweenDefaultColorAlpha` | float |
| `Tweens.Line2DWidth` | `TweenWidth` | float |

### Parallax2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.Parallax2DAutoscroll` | `TweenAutoscroll` | Vector2 |
| `Tweens.Parallax2DAutoscrollX` | `TweenAutoscrollX` | float |
| `Tweens.Parallax2DAutoscrollY` | `TweenAutoscrollY` | float |
| `Tweens.Parallax2DScrollOffset` | `TweenScrollOffset` | Vector2 |
| `Tweens.Parallax2DScrollOffsetX` | `TweenScrollOffsetX` | float |
| `Tweens.Parallax2DScrollOffsetY` | `TweenScrollOffsetY` | float |
| `Tweens.Parallax2DScrollScale` | `TweenScrollScale` | Vector2 |
| `Tweens.Parallax2DScrollScaleX` | `TweenScrollScaleX` | float |
| `Tweens.Parallax2DScrollScaleY` | `TweenScrollScaleY` | float |

### PathFollow2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.PathFollow2DHOffset` | `TweenHOffset` | float |
| `Tweens.PathFollow2DProgress` | `TweenProgress` | float |
| `Tweens.PathFollow2DProgressRatio` | `TweenProgressRatio` | float |
| `Tweens.PathFollow2DVOffset` | `TweenVOffset` | float |

### PointLight2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.PointLight2DHeight` | `TweenHeight` | float |
| `Tweens.PointLight2DOffset` | `TweenOffset` | Vector2 |
| `Tweens.PointLight2DOffsetX` | `TweenOffsetX` | float |
| `Tweens.PointLight2DOffsetY` | `TweenOffsetY` | float |
| `Tweens.PointLight2DTextureScale` | `TweenTextureScale` | float |

### Polygon2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.Polygon2DColor` | `TweenColor` | Color |
| `Tweens.Polygon2DColorAlpha` | `TweenColorAlpha` | float |
| `Tweens.Polygon2DOffset` | `TweenOffset` | Vector2 |
| `Tweens.Polygon2DOffsetX` | `TweenOffsetX` | float |
| `Tweens.Polygon2DOffsetY` | `TweenOffsetY` | float |
| `Tweens.Polygon2DTextureOffset` | `TweenTextureOffset` | Vector2 |
| `Tweens.Polygon2DTextureOffsetX` | `TweenTextureOffsetX` | float |
| `Tweens.Polygon2DTextureOffsetY` | `TweenTextureOffsetY` | float |
| `Tweens.Polygon2DTextureRotation` | `TweenTextureRotation` | float |
| `Tweens.Polygon2DTextureScale` | `TweenTextureScale` | Vector2 |
| `Tweens.Polygon2DTextureScaleX` | `TweenTextureScaleX` | float |
| `Tweens.Polygon2DTextureScaleY` | `TweenTextureScaleY` | float |

### Sprite2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.Sprite2DFrame` | `TweenFrame` | int |
| `Tweens.Sprite2DOffset` | `TweenOffset` | Vector2 |
| `Tweens.Sprite2DOffsetX` | `TweenOffsetX` | float |
| `Tweens.Sprite2DOffsetY` | `TweenOffsetY` | float |
| `Tweens.Sprite2DRegionRect` | `TweenRegionRect` | Rect2 |

### Node3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.GlobalPosition3D` | `TweenGlobalPosition` | Vector3 |
| `Tweens.GlobalPosition3DX` | `TweenGlobalPositionX` | float |
| `Tweens.GlobalPosition3DY` | `TweenGlobalPositionY` | float |
| `Tweens.GlobalPosition3DZ` | `TweenGlobalPositionZ` | float |
| `Tweens.GlobalQuaternion3D` | `TweenGlobalQuaternion` | Quaternion |
| `Tweens.GlobalRotation3D` | `TweenGlobalRotation` | Vector3 |
| `Tweens.GlobalRotation3DX` | `TweenGlobalRotationX` | float |
| `Tweens.GlobalRotation3DY` | `TweenGlobalRotationY` | float |
| `Tweens.GlobalRotation3DZ` | `TweenGlobalRotationZ` | float |
| `Tweens.Position3D` | `TweenPosition` | Vector3 |
| `Tweens.Position3DX` | `TweenPositionX` | float |
| `Tweens.Position3DY` | `TweenPositionY` | float |
| `Tweens.Position3DZ` | `TweenPositionZ` | float |
| `Tweens.Quaternion3D` | `TweenQuaternion` | Quaternion |
| `Tweens.Rotation3D` | `TweenRotation` | Vector3 |
| `Tweens.Rotation3DX` | `TweenRotationX` | float |
| `Tweens.Rotation3DY` | `TweenRotationY` | float |
| `Tweens.Rotation3DZ` | `TweenRotationZ` | float |
| `Tweens.Scale3D` | `TweenScale` | Vector3 |
| `Tweens.Scale3DX` | `TweenScaleX` | float |
| `Tweens.Scale3DY` | `TweenScaleY` | float |
| `Tweens.Scale3DZ` | `TweenScaleZ` | float |

### GeometryInstance3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.GeometryInstance3DTransparency` | `TweenTransparency` | float |

### SpriteBase3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.SpriteBase3DModulate` | `TweenModulate` | Color |
| `Tweens.SpriteBase3DModulateAlpha` | `TweenModulateAlpha` | float |
| `Tweens.SpriteBase3DOffset` | `TweenOffset` | Vector2 |
| `Tweens.SpriteBase3DOffsetX` | `TweenOffsetX` | float |
| `Tweens.SpriteBase3DOffsetY` | `TweenOffsetY` | float |
| `Tweens.SpriteBase3DPixelSize` | `TweenPixelSize` | float |

### AnimatedSprite3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.AnimatedSprite3DFrame` | `TweenFrame` | int |
| `Tweens.AnimatedSprite3DSpeedScale` | `TweenSpeedScale` | float |

### Camera3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.Camera3DFar` | `TweenFar` | float |
| `Tweens.Camera3DFov` | `TweenFov` | float |
| `Tweens.Camera3DFrustumOffset` | `TweenFrustumOffset` | Vector2 |
| `Tweens.Camera3DFrustumOffsetX` | `TweenFrustumOffsetX` | float |
| `Tweens.Camera3DFrustumOffsetY` | `TweenFrustumOffsetY` | float |
| `Tweens.Camera3DHOffset` | `TweenHOffset` | float |
| `Tweens.Camera3DNear` | `TweenNear` | float |
| `Tweens.Camera3DSize` | `TweenSize` | float |
| `Tweens.Camera3DVOffset` | `TweenVOffset` | float |

### CpuParticles3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.CpuParticles3DColor` | `TweenColor` | Color |
| `Tweens.CpuParticles3DColorAlpha` | `TweenColorAlpha` | float |
| `Tweens.CpuParticles3DDirection` | `TweenDirection` | Vector3 |
| `Tweens.CpuParticles3DDirectionX` | `TweenDirectionX` | float |
| `Tweens.CpuParticles3DDirectionY` | `TweenDirectionY` | float |
| `Tweens.CpuParticles3DDirectionZ` | `TweenDirectionZ` | float |
| `Tweens.CpuParticles3DEmissionBoxExtents` | `TweenEmissionBoxExtents` | Vector3 |
| `Tweens.CpuParticles3DEmissionBoxExtentsX` | `TweenEmissionBoxExtentsX` | float |
| `Tweens.CpuParticles3DEmissionBoxExtentsY` | `TweenEmissionBoxExtentsY` | float |
| `Tweens.CpuParticles3DEmissionBoxExtentsZ` | `TweenEmissionBoxExtentsZ` | float |
| `Tweens.CpuParticles3DEmissionSphereRadius` | `TweenEmissionSphereRadius` | float |
| `Tweens.CpuParticles3DExplosiveness` | `TweenExplosiveness` | float |
| `Tweens.CpuParticles3DGravity` | `TweenGravity` | Vector3 |
| `Tweens.CpuParticles3DGravityX` | `TweenGravityX` | float |
| `Tweens.CpuParticles3DGravityY` | `TweenGravityY` | float |
| `Tweens.CpuParticles3DGravityZ` | `TweenGravityZ` | float |
| `Tweens.CpuParticles3DLifetime` | `TweenLifetime` | double |
| `Tweens.CpuParticles3DRandomness` | `TweenRandomness` | float |
| `Tweens.CpuParticles3DSpeedScale` | `TweenSpeedScale` | double |
| `Tweens.CpuParticles3DSpread` | `TweenSpread` | float |

### Decal

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.DecalEmissionEnergy` | `TweenEmissionEnergy` | float |
| `Tweens.DecalModulate` | `TweenModulate` | Color |
| `Tweens.DecalModulateAlpha` | `TweenModulateAlpha` | float |
| `Tweens.DecalSize` | `TweenSize` | Vector3 |
| `Tweens.DecalSizeX` | `TweenSizeX` | float |
| `Tweens.DecalSizeY` | `TweenSizeY` | float |
| `Tweens.DecalSizeZ` | `TweenSizeZ` | float |

### FogVolume

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.FogVolumeSize` | `TweenSize` | Vector3 |
| `Tweens.FogVolumeSizeX` | `TweenSizeX` | float |
| `Tweens.FogVolumeSizeY` | `TweenSizeY` | float |
| `Tweens.FogVolumeSizeZ` | `TweenSizeZ` | float |

### GpuParticles3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.GpuParticles3DAmountRatio` | `TweenAmountRatio` | float |
| `Tweens.GpuParticles3DExplosiveness` | `TweenExplosiveness` | float |
| `Tweens.GpuParticles3DLifetime` | `TweenLifetime` | double |
| `Tweens.GpuParticles3DRandomness` | `TweenRandomness` | float |
| `Tweens.GpuParticles3DSpeedScale` | `TweenSpeedScale` | double |

### Label3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.Label3DModulate` | `TweenModulate` | Color |
| `Tweens.Label3DModulateAlpha` | `TweenModulateAlpha` | float |
| `Tweens.Label3DOffset` | `TweenOffset` | Vector2 |
| `Tweens.Label3DOffsetX` | `TweenOffsetX` | float |
| `Tweens.Label3DOffsetY` | `TweenOffsetY` | float |
| `Tweens.Label3DOutlineModulate` | `TweenOutlineModulate` | Color |
| `Tweens.Label3DOutlineModulateAlpha` | `TweenOutlineModulateAlpha` | float |
| `Tweens.Label3DPixelSize` | `TweenPixelSize` | float |

### Light3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.LightColor3D` | `TweenLightColor` | Color |
| `Tweens.LightEnergy3D` | `TweenLightEnergy` | float |
| `Tweens.Light3DLightIndirectEnergy` | `TweenLightIndirectEnergy` | float |
| `Tweens.Light3DLightTemperature` | `TweenLightTemperature` | float |
| `Tweens.Light3DLightVolumetricFogEnergy` | `TweenLightVolumetricFogEnergy` | float |
| `Tweens.Light3DShadowOpacity` | `TweenShadowOpacity` | float |

### OmniLight3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.OmniLight3DOmniAttenuation` | `TweenOmniAttenuation` | float |
| `Tweens.OmniRange` | `TweenOmniRange` | float |

### PathFollow3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.PathFollow3DHOffset` | `TweenHOffset` | float |
| `Tweens.PathFollow3DProgress` | `TweenProgress` | float |
| `Tweens.PathFollow3DProgressRatio` | `TweenProgressRatio` | float |
| `Tweens.PathFollow3DVOffset` | `TweenVOffset` | float |

### SpotLight3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.SpotAngle` | `TweenSpotAngle` | float |
| `Tweens.SpotLight3DSpotAngleAttenuation` | `TweenSpotAngleAttenuation` | float |
| `Tweens.SpotLight3DSpotAttenuation` | `TweenSpotAttenuation` | float |
| `Tweens.SpotRange` | `TweenSpotRange` | float |

### SpringArm3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.SpringArm3DSpringLength` | `TweenSpringLength` | float |

### Control

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.ControlAnchorBottom` | `TweenAnchorBottom` | float |
| `Tweens.ControlAnchorLeft` | `TweenAnchorLeft` | float |
| `Tweens.ControlAnchorMax` | `TweenAnchorMax` | Vector2 |
| `Tweens.ControlAnchorMin` | `TweenAnchorMin` | Vector2 |
| `Tweens.ControlAnchorRight` | `TweenAnchorRight` | float |
| `Tweens.ControlAnchorTop` | `TweenAnchorTop` | float |
| `Tweens.ControlCustomMaximumSize` | `TweenCustomMaximumSize` | Vector2 |
| `Tweens.ControlCustomMaximumSizeX` | `TweenCustomMaximumSizeX` | float |
| `Tweens.ControlCustomMaximumSizeY` | `TweenCustomMaximumSizeY` | float |
| `Tweens.ControlCustomMinimumSize` | `TweenCustomMinimumSize` | Vector2 |
| `Tweens.ControlCustomMinimumSizeX` | `TweenCustomMinimumSizeX` | float |
| `Tweens.ControlCustomMinimumSizeY` | `TweenCustomMinimumSizeY` | float |
| `Tweens.ControlGlobalPosition` | `TweenGlobalPosition` | Vector2 |
| `Tweens.ControlGlobalPositionX` | `TweenGlobalPositionX` | float |
| `Tweens.ControlGlobalPositionY` | `TweenGlobalPositionY` | float |
| `Tweens.ControlOffsetBottom` | `TweenOffsetBottom` | float |
| `Tweens.ControlOffsetLeft` | `TweenOffsetLeft` | float |
| `Tweens.ControlOffsetRight` | `TweenOffsetRight` | float |
| `Tweens.ControlOffsets` | `TweenOffsets` | Vector4 |
| `Tweens.ControlOffsetTop` | `TweenOffsetTop` | float |
| `Tweens.ControlOffsetTransformPivot` | `TweenOffsetTransformPivot` | Vector2 |
| `Tweens.ControlOffsetTransformPivotRatio` | `TweenOffsetTransformPivotRatio` | Vector2 |
| `Tweens.ControlOffsetTransformPivotRatioX` | `TweenOffsetTransformPivotRatioX` | float |
| `Tweens.ControlOffsetTransformPivotRatioY` | `TweenOffsetTransformPivotRatioY` | float |
| `Tweens.ControlOffsetTransformPivotX` | `TweenOffsetTransformPivotX` | float |
| `Tweens.ControlOffsetTransformPivotY` | `TweenOffsetTransformPivotY` | float |
| `Tweens.ControlOffsetTransformPosition` | `TweenOffsetTransformPosition` | Vector2 |
| `Tweens.ControlOffsetTransformPositionRatio` | `TweenOffsetTransformPositionRatio` | Vector2 |
| `Tweens.ControlOffsetTransformPositionRatioX` | `TweenOffsetTransformPositionRatioX` | float |
| `Tweens.ControlOffsetTransformPositionRatioY` | `TweenOffsetTransformPositionRatioY` | float |
| `Tweens.ControlOffsetTransformPositionX` | `TweenOffsetTransformPositionX` | float |
| `Tweens.ControlOffsetTransformPositionY` | `TweenOffsetTransformPositionY` | float |
| `Tweens.ControlOffsetTransformRotation` | `TweenOffsetTransformRotation` | float |
| `Tweens.ControlOffsetTransformScale` | `TweenOffsetTransformScale` | Vector2 |
| `Tweens.ControlOffsetTransformScaleX` | `TweenOffsetTransformScaleX` | float |
| `Tweens.ControlOffsetTransformScaleY` | `TweenOffsetTransformScaleY` | float |
| `Tweens.ControlPivotOffset` | `TweenPivotOffset` | Vector2 |
| `Tweens.ControlPivotOffsetRatio` | `TweenPivotOffsetRatio` | Vector2 |
| `Tweens.ControlPivotOffsetRatioX` | `TweenPivotOffsetRatioX` | float |
| `Tweens.ControlPivotOffsetRatioY` | `TweenPivotOffsetRatioY` | float |
| `Tweens.ControlPivotOffsetX` | `TweenPivotOffsetX` | float |
| `Tweens.ControlPivotOffsetY` | `TweenPivotOffsetY` | float |
| `Tweens.ControlPosition` | `TweenPosition` | Vector2 |
| `Tweens.ControlPositionX` | `TweenPositionX` | float |
| `Tweens.ControlPositionY` | `TweenPositionY` | float |
| `Tweens.ControlRotation` | `TweenRotation` | float |
| `Tweens.ControlScale` | `TweenScale` | Vector2 |
| `Tweens.ControlScaleX` | `TweenScaleX` | float |
| `Tweens.ControlScaleY` | `TweenScaleY` | float |
| `Tweens.ControlSize` | `TweenSize` | Vector2 |
| `Tweens.ControlSizeFlagsStretchRatio` | `TweenSizeFlagsStretchRatio` | float |
| `Tweens.ControlSizeX` | `TweenSizeX` | float |
| `Tweens.ControlSizeY` | `TweenSizeY` | float |

### Range

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.RangeValue` | `TweenValue` | double |

### ColorRect

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.ColorRectColor` | `TweenColor` | Color |
| `Tweens.ColorRectColorAlpha` | `TweenColorAlpha` | float |

### Label

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.LabelVisibleCharacters` | `TweenVisibleCharacters` | int |
| `Tweens.LabelVisibleRatio` | `TweenVisibleRatio` | float |

### RichTextLabel

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.RichTextLabelVisibleCharacters` | `TweenVisibleCharacters` | int |
| `Tweens.RichTextLabelVisibleRatio` | `TweenVisibleRatio` | float |

### ScrollContainer

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.ScrollContainerScrollHorizontal` | `TweenScrollHorizontal` | int |
| `Tweens.ScrollContainerScrollVertical` | `TweenScrollVertical` | int |

### TextureProgressBar

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.TextureProgressBarRadialCenterOffset` | `TweenRadialCenterOffset` | Vector2 |
| `Tweens.TextureProgressBarRadialCenterOffsetX` | `TweenRadialCenterOffsetX` | float |
| `Tweens.TextureProgressBarRadialCenterOffsetY` | `TweenRadialCenterOffsetY` | float |
| `Tweens.TextureProgressBarRadialFillDegrees` | `TweenRadialFillDegrees` | float |
| `Tweens.TextureProgressBarRadialInitialAngle` | `TweenRadialInitialAngle` | float |
| `Tweens.TextureProgressBarTextureProgressOffset` | `TweenTextureProgressOffset` | Vector2 |
| `Tweens.TextureProgressBarTextureProgressOffsetX` | `TweenTextureProgressOffsetX` | float |
| `Tweens.TextureProgressBarTextureProgressOffsetY` | `TweenTextureProgressOffsetY` | float |
| `Tweens.TextureProgressBarTintOver` | `TweenTintOver` | Color |
| `Tweens.TextureProgressBarTintOverAlpha` | `TweenTintOverAlpha` | float |
| `Tweens.TextureProgressBarTintProgress` | `TweenTintProgress` | Color |
| `Tweens.TextureProgressBarTintProgressAlpha` | `TweenTintProgressAlpha` | float |
| `Tweens.TextureProgressBarTintUnder` | `TweenTintUnder` | Color |
| `Tweens.TextureProgressBarTintUnderAlpha` | `TweenTintUnderAlpha` | float |

### AnimationPlayer

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.AnimationPlayerSpeedScale` | `TweenSpeedScale` | float |

### AudioStreamPlayer

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.AudioPitchScale` | `TweenPitchScale` | float |
| `Tweens.AudioVolumeDb` | `TweenVolumeDb` | float |
| `Tweens.AudioVolumeLinear` | `TweenVolumeLinear` | float |

### AudioStreamPlayer2D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.AudioStreamPlayer2DAttenuation` | `TweenAttenuation` | float |
| `Tweens.AudioStreamPlayer2DMaxDistance` | `TweenMaxDistance` | float |
| `Tweens.AudioStreamPlayer2DPanningStrength` | `TweenPanningStrength` | float |
| `Tweens.AudioPitchScale2D` | `TweenPitchScale` | float |
| `Tweens.AudioVolumeDb2D` | `TweenVolumeDb` | float |
| `Tweens.AudioVolumeLinear2D` | `TweenVolumeLinear` | float |

### AudioStreamPlayer3D

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.AudioStreamPlayer3DAttenuationFilterCutoffHz` | `TweenAttenuationFilterCutoffHz` | float |
| `Tweens.AudioStreamPlayer3DAttenuationFilterDb` | `TweenAttenuationFilterDb` | float |
| `Tweens.AudioStreamPlayer3DEmissionAngleDegrees` | `TweenEmissionAngleDegrees` | float |
| `Tweens.AudioStreamPlayer3DEmissionAngleFilterAttenuationDb` | `TweenEmissionAngleFilterAttenuationDb` | float |
| `Tweens.AudioStreamPlayer3DMaxDistance` | `TweenMaxDistance` | float |
| `Tweens.AudioStreamPlayer3DPanningStrength` | `TweenPanningStrength` | float |
| `Tweens.AudioPitchScale3D` | `TweenPitchScale` | float |
| `Tweens.AudioStreamPlayer3DUnitSize` | `TweenUnitSize` | float |
| `Tweens.AudioVolumeDb3D` | `TweenVolumeDb` | float |
| `Tweens.AudioVolumeLinear3D` | `TweenVolumeLinear` | float |

### Callback values

These target any `Node`, write no property, and deliver each sample to `OnUpdate`. See
[custom tweens](/csharp/custom-tweens/).

| Definition | Extension | Value |
| --- | --- | --- |
| `Tweens.Color` | `TweenColor` | Color |
| `Tweens.Double` | `TweenDouble` | double |
| `Tweens.Float` | `TweenFloat` | float |
| `Tweens.Quaternion` | `TweenQuaternion` | Quaternion |
| `Tweens.Rect2` | `TweenRect2` | Rect2 |
| `Tweens.Vector2` | `TweenVector2` | Vector2 |
| `Tweens.Vector3` | `TweenVector3` | Vector3 |
| `Tweens.Vector4` | `TweenVector4` | Vector4 |

## Units and engine constraints

- Rotation, skew, and texture rotation use radians. Camera `Fov`, light and emission
  angles, and radial progress angles use degrees. Ratios use the native property
  range; positions, sizes, paths, and offsets use the Godot units of each property.
- Particle `SpeedScale` and `Lifetime` are `double`, matching Godot's properties.
  Other value types match their native property types.
- Integer properties (`VisibleCharacters`, `ScrollHorizontal`, `ScrollVertical`, and
  sprite `Frame`) interpolate continuously, round to nearest with midpoint ties away
  from zero, and saturate at `Int32` limits. Native constraints still apply. Set up
  sprite frames and animations, and scrollable content, before tweening them. A
  negative `VisibleCharacters` value keeps its native Godot meaning; set `From = 0`
  explicitly for a reveal.
- `Tweens.GlobalQuaternion3D` interpolates along the shortest quaternion path, then
  writes Godot's YXZ global rotation. It preserves the current global position and
  basis scale. Like Godot's `GlobalRotation`, it replaces shear and can change local
  scale under a nonuniformly scaled parent. It does not preserve an arbitrary sheared
  transform. Singular transforms are not a supported orientation use case.
- `Node2D` global scale and skew follow Godot's parent-relative decomposition. Under
  a nonuniformly scaled parent, changing skew can also change apparent scale; the two
  are not independent transform channels. Axis definitions (`...X`, `...Y`) preserve
  the other components of the property they write, not every derived transform
  component.
- `Control` anchors use push-opposite behavior. Minimum and maximum size constraints
  and containers still apply. The `OffsetTransform` properties require
  `OffsetTransformEnabled = true`; they move and scale a control visually without
  rewriting its container-controlled `Position` and `Size`. The pivot and position
  `Ratio` variants are relative to `Control.Size`.
- Path definitions require a `PathFollow2D` or `PathFollow3D` under a matching
  `Path2D` or `Path3D` with a nonempty curve. `ProgressRatio` is normalized;
  `Progress` is a distance. The node still controls looping and wrapping.
- `Camera3D` `Size` and `FrustumOffset` depend on the projection mode,
  `LightTemperature` depends on physical light settings, and particle emission
  parameters depend on the configured emitter modes and materials. Setting a property
  does not enable a rendering feature or create a resource.
- `GeometryInstance3D.Transparency` is an amount of transparency (0 opaque,
  1 transparent), not opacity. Renderer support and sorting limitations are Godot's.
  A headless property test does not guarantee identical rendering across backends.
- Materials and shader parameters are covered in [materials](/csharp/materials/) and
  [shaders](/csharp/shaders/).
