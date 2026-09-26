// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Reflection;
using Godot;
using tweens.gd;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace testbed.Tests.Integration;

[Collection<HeadlessCollection>]
public class ExpandedAdapterTests(HeadlessFixture godot)
{
    // Independent property contracts: expected values are read through Godot's public properties,
    // never through the adapter's getter/setter or production interpolation functions.
    public static IEnumerable<object[]> Cases()
    {
        foreach (var row in AdapterCatalogTests.Cases())
            yield return [row[0], row[1], row[2], 0.2d, 0.6d];
        yield return [typeof(Skew2DTween), "Skew", "", 0.2d, 0.6d];
        yield return [typeof(GlobalSkew2DTween), "GlobalSkew", "", 0.2d, 0.6d];
        yield return [typeof(GlobalScale2DTween), "GlobalScale", "", 0.2d, 0.6d];
        yield return [typeof(GlobalScale2DXTween), "GlobalScale", "X", 0.2d, 0.6d];
        yield return [typeof(GlobalScale2DYTween), "GlobalScale", "Y", 0.2d, 0.6d];
        yield return [typeof(ControlPivotOffsetTween), "PivotOffset", "", 0.2d, 0.6d];
        yield return [typeof(ControlPivotOffsetXTween), "PivotOffset", "X", 0.2d, 0.6d];
        yield return [typeof(ControlPivotOffsetYTween), "PivotOffset", "Y", 0.2d, 0.6d];
        yield return [typeof(ControlPivotOffsetRatioTween), "PivotOffsetRatio", "", 0.2d, 0.6d];
        yield return [typeof(ControlPivotOffsetRatioXTween), "PivotOffsetRatio", "X", 0.2d, 0.6d];
        yield return [typeof(ControlPivotOffsetRatioYTween), "PivotOffsetRatio", "Y", 0.2d, 0.6d];
        yield return [typeof(ControlCustomMinimumSizeTween), "CustomMinimumSize", "", 0.2d, 0.6d];
        yield return [typeof(ControlCustomMinimumSizeXTween), "CustomMinimumSize", "X", 0.2d, 0.6d];
        yield return [typeof(ControlCustomMinimumSizeYTween), "CustomMinimumSize", "Y", 0.2d, 0.6d];
        yield return [typeof(ControlCustomMaximumSizeTween), "CustomMaximumSize", "", 0.2d, 0.6d];
        yield return [typeof(ControlCustomMaximumSizeXTween), "CustomMaximumSize", "X", 0.2d, 0.6d];
        yield return [typeof(ControlCustomMaximumSizeYTween), "CustomMaximumSize", "Y", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformPositionTween), "OffsetTransformPosition", "", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformPositionXTween), "OffsetTransformPosition", "X", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformPositionYTween), "OffsetTransformPosition", "Y", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformPositionRatioTween), "OffsetTransformPositionRatio", "", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformPositionRatioXTween), "OffsetTransformPositionRatio", "X", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformPositionRatioYTween), "OffsetTransformPositionRatio", "Y", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformScaleTween), "OffsetTransformScale", "", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformScaleXTween), "OffsetTransformScale", "X", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformScaleYTween), "OffsetTransformScale", "Y", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformPivotTween), "OffsetTransformPivot", "", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformPivotXTween), "OffsetTransformPivot", "X", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformPivotYTween), "OffsetTransformPivot", "Y", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformPivotRatioTween), "OffsetTransformPivotRatio", "", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformPivotRatioXTween), "OffsetTransformPivotRatio", "X", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformPivotRatioYTween), "OffsetTransformPivotRatio", "Y", 0.2d, 0.6d];
        yield return [typeof(ControlSizeFlagsStretchRatioTween), "SizeFlagsStretchRatio", "", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTransformRotationTween), "OffsetTransformRotation", "", 0.2d, 0.6d];
        yield return [typeof(ControlAnchorLeftTween), "AnchorLeft", "", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetLeftTween), "OffsetLeft", "", 0.2d, 0.6d];
        yield return [typeof(ControlAnchorTopTween), "AnchorTop", "", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetTopTween), "OffsetTop", "", 0.2d, 0.6d];
        yield return [typeof(ControlAnchorRightTween), "AnchorRight", "", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetRightTween), "OffsetRight", "", 0.2d, 0.6d];
        yield return [typeof(ControlAnchorBottomTween), "AnchorBottom", "", 0.2d, 0.6d];
        yield return [typeof(ControlOffsetBottomTween), "OffsetBottom", "", 0.2d, 0.6d];
        yield return [typeof(Camera2DZoomTween), "Zoom", "", 0.2d, 0.6d];
        yield return [typeof(Camera2DZoomXTween), "Zoom", "X", 0.2d, 0.6d];
        yield return [typeof(Camera2DZoomYTween), "Zoom", "Y", 0.2d, 0.6d];
        yield return [typeof(Camera2DOffsetTween), "Offset", "", 0.2d, 0.6d];
        yield return [typeof(Camera2DOffsetXTween), "Offset", "X", 0.2d, 0.6d];
        yield return [typeof(Camera2DOffsetYTween), "Offset", "Y", 0.2d, 0.6d];
        yield return [typeof(Camera3DFovTween), "Fov", "", 20d, 60d];
        yield return [typeof(Camera3DSizeTween), "Size", "", 0.2d, 0.6d];
        yield return [typeof(Camera3DHOffsetTween), "HOffset", "", 0.2d, 0.6d];
        yield return [typeof(Camera3DVOffsetTween), "VOffset", "", 0.2d, 0.6d];
        yield return [typeof(Camera3DNearTween), "Near", "", 0.2d, 0.6d];
        yield return [typeof(Camera3DFarTween), "Far", "", 0.2d, 0.6d];
        yield return [typeof(Camera3DFrustumOffsetTween), "FrustumOffset", "", 0.2d, 0.6d];
        yield return [typeof(Camera3DFrustumOffsetXTween), "FrustumOffset", "X", 0.2d, 0.6d];
        yield return [typeof(Camera3DFrustumOffsetYTween), "FrustumOffset", "Y", 0.2d, 0.6d];
        yield return [typeof(SpriteBase3DModulateTween), "Modulate", "", 0.2d, 0.6d];
        yield return [typeof(SpriteBase3DModulateAlphaTween), "Modulate", "A", 0.2d, 0.6d];
        yield return [typeof(SpriteBase3DOffsetTween), "Offset", "", 0.2d, 0.6d];
        yield return [typeof(SpriteBase3DOffsetXTween), "Offset", "X", 0.2d, 0.6d];
        yield return [typeof(SpriteBase3DOffsetYTween), "Offset", "Y", 0.2d, 0.6d];
        yield return [typeof(SpriteBase3DPixelSizeTween), "PixelSize", "", 0.2d, 0.6d];
        yield return [typeof(Label3DModulateTween), "Modulate", "", 0.2d, 0.6d];
        yield return [typeof(Label3DModulateAlphaTween), "Modulate", "A", 0.2d, 0.6d];
        yield return [typeof(Label3DOutlineModulateTween), "OutlineModulate", "", 0.2d, 0.6d];
        yield return [typeof(Label3DOutlineModulateAlphaTween), "OutlineModulate", "A", 0.2d, 0.6d];
        yield return [typeof(Label3DOffsetTween), "Offset", "", 0.2d, 0.6d];
        yield return [typeof(Label3DOffsetXTween), "Offset", "X", 0.2d, 0.6d];
        yield return [typeof(Label3DOffsetYTween), "Offset", "Y", 0.2d, 0.6d];
        yield return [typeof(Label3DPixelSizeTween), "PixelSize", "", 0.2d, 0.6d];
        yield return [typeof(GeometryInstance3DTransparencyTween), "Transparency", "", 0.2d, 0.6d];
        yield return [typeof(PathFollow2DProgressTween), "Progress", "", 0.2d, 0.6d];
        yield return [typeof(PathFollow2DProgressRatioTween), "ProgressRatio", "", 0.2d, 0.6d];
        yield return [typeof(PathFollow2DHOffsetTween), "HOffset", "", 0.2d, 0.6d];
        yield return [typeof(PathFollow2DVOffsetTween), "VOffset", "", 0.2d, 0.6d];
        yield return [typeof(PathFollow3DProgressTween), "Progress", "", 0.2d, 0.6d];
        yield return [typeof(PathFollow3DProgressRatioTween), "ProgressRatio", "", 0.2d, 0.6d];
        yield return [typeof(PathFollow3DHOffsetTween), "HOffset", "", 0.2d, 0.6d];
        yield return [typeof(PathFollow3DVOffsetTween), "VOffset", "", 0.2d, 0.6d];
        yield return [typeof(ColorRectColorTween), "Color", "", 0.2d, 0.6d];
        yield return [typeof(ColorRectColorAlphaTween), "Color", "A", 0.2d, 0.6d];
        yield return [typeof(LabelVisibleRatioTween), "VisibleRatio", "", 0.2d, 0.6d];
        yield return [typeof(LabelVisibleCharactersTween), "VisibleCharacters", "", 2d, 6d];
        yield return [typeof(RichTextLabelVisibleRatioTween), "VisibleRatio", "", 0.2d, 0.6d];
        yield return [typeof(RichTextLabelVisibleCharactersTween), "VisibleCharacters", "", 2d, 6d];
        yield return [typeof(TextureProgressBarTintUnderTween), "TintUnder", "", 0.2d, 0.6d];
        yield return [typeof(TextureProgressBarTintUnderAlphaTween), "TintUnder", "A", 0.2d, 0.6d];
        yield return [typeof(TextureProgressBarTintOverTween), "TintOver", "", 0.2d, 0.6d];
        yield return [typeof(TextureProgressBarTintOverAlphaTween), "TintOver", "A", 0.2d, 0.6d];
        yield return [typeof(TextureProgressBarTintProgressTween), "TintProgress", "", 0.2d, 0.6d];
        yield return [typeof(TextureProgressBarTintProgressAlphaTween), "TintProgress", "A", 0.2d, 0.6d];
        yield return [typeof(TextureProgressBarRadialInitialAngleTween), "RadialInitialAngle", "", 20d, 60d];
        yield return [typeof(TextureProgressBarRadialFillDegreesTween), "RadialFillDegrees", "", 20d, 60d];
        yield return [typeof(TextureProgressBarRadialCenterOffsetTween), "RadialCenterOffset", "", 0.2d, 0.6d];
        yield return [typeof(TextureProgressBarRadialCenterOffsetXTween), "RadialCenterOffset", "X", 0.2d, 0.6d];
        yield return [typeof(TextureProgressBarRadialCenterOffsetYTween), "RadialCenterOffset", "Y", 0.2d, 0.6d];
        yield return [typeof(TextureProgressBarTextureProgressOffsetTween), "TextureProgressOffset", "", 0.2d, 0.6d];
        yield return [typeof(TextureProgressBarTextureProgressOffsetXTween), "TextureProgressOffset", "X", 0.2d, 0.6d];
        yield return [typeof(TextureProgressBarTextureProgressOffsetYTween), "TextureProgressOffset", "Y", 0.2d, 0.6d];
        yield return [typeof(CanvasLayerOffsetTween), "Offset", "", 0.2d, 0.6d];
        yield return [typeof(CanvasLayerOffsetXTween), "Offset", "X", 0.2d, 0.6d];
        yield return [typeof(CanvasLayerOffsetYTween), "Offset", "Y", 0.2d, 0.6d];
        yield return [typeof(CanvasLayerScaleTween), "Scale", "", 0.2d, 0.6d];
        yield return [typeof(CanvasLayerScaleXTween), "Scale", "X", 0.2d, 0.6d];
        yield return [typeof(CanvasLayerScaleYTween), "Scale", "Y", 0.2d, 0.6d];
        yield return [typeof(CanvasLayerRotationTween), "Rotation", "", 0.2d, 0.6d];
        yield return [typeof(CanvasModulateColorTween), "Color", "", 0.2d, 0.6d];
        yield return [typeof(CanvasModulateColorAlphaTween), "Color", "A", 0.2d, 0.6d];
        yield return [typeof(Parallax2DScrollOffsetTween), "ScrollOffset", "", 0.2d, 0.6d];
        yield return [typeof(Parallax2DScrollOffsetXTween), "ScrollOffset", "X", 0.2d, 0.6d];
        yield return [typeof(Parallax2DScrollOffsetYTween), "ScrollOffset", "Y", 0.2d, 0.6d];
        yield return [typeof(Parallax2DScrollScaleTween), "ScrollScale", "", 0.2d, 0.6d];
        yield return [typeof(Parallax2DScrollScaleXTween), "ScrollScale", "X", 0.2d, 0.6d];
        yield return [typeof(Parallax2DScrollScaleYTween), "ScrollScale", "Y", 0.2d, 0.6d];
        yield return [typeof(Parallax2DAutoscrollTween), "Autoscroll", "", 0.2d, 0.6d];
        yield return [typeof(Parallax2DAutoscrollXTween), "Autoscroll", "X", 0.2d, 0.6d];
        yield return [typeof(Parallax2DAutoscrollYTween), "Autoscroll", "Y", 0.2d, 0.6d];
        yield return [typeof(Sprite2DOffsetTween), "Offset", "", 0.2d, 0.6d];
        yield return [typeof(Sprite2DOffsetXTween), "Offset", "X", 0.2d, 0.6d];
        yield return [typeof(Sprite2DOffsetYTween), "Offset", "Y", 0.2d, 0.6d];
        yield return [typeof(Sprite2DRegionRectTween), "RegionRect", "", 0.2d, 0.6d];
        yield return [typeof(Line2DWidthTween), "Width", "", 0.2d, 0.6d];
        yield return [typeof(Line2DDefaultColorTween), "DefaultColor", "", 0.2d, 0.6d];
        yield return [typeof(Line2DDefaultColorAlphaTween), "DefaultColor", "A", 0.2d, 0.6d];
        yield return [typeof(Polygon2DColorTween), "Color", "", 0.2d, 0.6d];
        yield return [typeof(Polygon2DColorAlphaTween), "Color", "A", 0.2d, 0.6d];
        yield return [typeof(Polygon2DOffsetTween), "Offset", "", 0.2d, 0.6d];
        yield return [typeof(Polygon2DOffsetXTween), "Offset", "X", 0.2d, 0.6d];
        yield return [typeof(Polygon2DOffsetYTween), "Offset", "Y", 0.2d, 0.6d];
        yield return [typeof(Polygon2DTextureOffsetTween), "TextureOffset", "", 0.2d, 0.6d];
        yield return [typeof(Polygon2DTextureOffsetXTween), "TextureOffset", "X", 0.2d, 0.6d];
        yield return [typeof(Polygon2DTextureOffsetYTween), "TextureOffset", "Y", 0.2d, 0.6d];
        yield return [typeof(Polygon2DTextureScaleTween), "TextureScale", "", 0.2d, 0.6d];
        yield return [typeof(Polygon2DTextureScaleXTween), "TextureScale", "X", 0.2d, 0.6d];
        yield return [typeof(Polygon2DTextureScaleYTween), "TextureScale", "Y", 0.2d, 0.6d];
        yield return [typeof(Polygon2DTextureRotationTween), "TextureRotation", "", 0.2d, 0.6d];
        yield return [typeof(PointLight2DTextureScaleTween), "TextureScale", "", 0.2d, 0.6d];
        yield return [typeof(PointLight2DHeightTween), "Height", "", 0.2d, 0.6d];
        yield return [typeof(PointLight2DOffsetTween), "Offset", "", 0.2d, 0.6d];
        yield return [typeof(PointLight2DOffsetXTween), "Offset", "X", 0.2d, 0.6d];
        yield return [typeof(PointLight2DOffsetYTween), "Offset", "Y", 0.2d, 0.6d];
        yield return [typeof(Light2DShadowColorTween), "ShadowColor", "", 0.2d, 0.6d];
        yield return [typeof(Light2DShadowColorAlphaTween), "ShadowColor", "A", 0.2d, 0.6d];
        yield return [typeof(Light3DLightTemperatureTween), "LightTemperature", "", 2000d, 6000d];
        yield return [typeof(Light3DLightIndirectEnergyTween), "LightIndirectEnergy", "", 0.2d, 0.6d];
        yield return [typeof(Light3DLightVolumetricFogEnergyTween), "LightVolumetricFogEnergy", "", 0.2d, 0.6d];
        yield return [typeof(Light3DShadowOpacityTween), "ShadowOpacity", "", 0.2d, 0.6d];
        yield return [typeof(OmniLight3DOmniAttenuationTween), "OmniAttenuation", "", 0.2d, 0.6d];
        yield return [typeof(SpotLight3DSpotAttenuationTween), "SpotAttenuation", "", 0.2d, 0.6d];
        yield return [typeof(SpotLight3DSpotAngleAttenuationTween), "SpotAngleAttenuation", "", 0.2d, 0.6d];
        yield return [typeof(AudioStreamPlayer2DPanningStrengthTween), "PanningStrength", "", 0.2d, 0.6d];
        yield return [typeof(AudioStreamPlayer2DMaxDistanceTween), "MaxDistance", "", 0.2d, 0.6d];
        yield return [typeof(AudioStreamPlayer3DPanningStrengthTween), "PanningStrength", "", 0.2d, 0.6d];
        yield return [typeof(AudioStreamPlayer3DMaxDistanceTween), "MaxDistance", "", 0.2d, 0.6d];
        yield return [typeof(AudioStreamPlayer2DAttenuationTween), "Attenuation", "", 0.2d, 0.6d];
        yield return [typeof(AudioStreamPlayer3DUnitSizeTween), "UnitSize", "", 0.2d, 0.6d];
        yield return [typeof(AudioStreamPlayer3DEmissionAngleDegreesTween), "EmissionAngleDegrees", "", 0.2d, 0.6d];
        yield return [typeof(AudioStreamPlayer3DEmissionAngleFilterAttenuationDbTween), "EmissionAngleFilterAttenuationDb", "", 0.2d, 0.6d];
        yield return [typeof(AudioStreamPlayer3DAttenuationFilterCutoffHzTween), "AttenuationFilterCutoffHz", "", 1000d, 5000d];
        yield return [typeof(AudioStreamPlayer3DAttenuationFilterDbTween), "AttenuationFilterDb", "", 0.2d, 0.6d];
        yield return [typeof(AnimatedSprite2DSpeedScaleTween), "SpeedScale", "", 0.2d, 0.6d];
        yield return [typeof(AnimatedSprite3DSpeedScaleTween), "SpeedScale", "", 0.2d, 0.6d];
        yield return [typeof(AnimationPlayerSpeedScaleTween), "SpeedScale", "", 0.2d, 0.6d];
        yield return [typeof(GpuParticles2DSpeedScaleTween), "SpeedScale", "", 0.2d, 0.6d];
        yield return [typeof(GpuParticles2DLifetimeTween), "Lifetime", "", 0.2d, 0.6d];
        yield return [typeof(GpuParticles2DExplosivenessTween), "Explosiveness", "", 0.2d, 0.6d];
        yield return [typeof(GpuParticles2DRandomnessTween), "Randomness", "", 0.2d, 0.6d];
        yield return [typeof(GpuParticles2DAmountRatioTween), "AmountRatio", "", 0.2d, 0.6d];
        yield return [typeof(GpuParticles3DSpeedScaleTween), "SpeedScale", "", 0.2d, 0.6d];
        yield return [typeof(GpuParticles3DLifetimeTween), "Lifetime", "", 0.2d, 0.6d];
        yield return [typeof(GpuParticles3DExplosivenessTween), "Explosiveness", "", 0.2d, 0.6d];
        yield return [typeof(GpuParticles3DRandomnessTween), "Randomness", "", 0.2d, 0.6d];
        yield return [typeof(GpuParticles3DAmountRatioTween), "AmountRatio", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DSpeedScaleTween), "SpeedScale", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DLifetimeTween), "Lifetime", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DExplosivenessTween), "Explosiveness", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DRandomnessTween), "Randomness", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DDirectionTween), "Direction", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DDirectionXTween), "Direction", "X", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DDirectionYTween), "Direction", "Y", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DGravityTween), "Gravity", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DGravityXTween), "Gravity", "X", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DGravityYTween), "Gravity", "Y", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DSpreadTween), "Spread", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DEmissionSphereRadiusTween), "EmissionSphereRadius", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DEmissionRectExtentsTween), "EmissionRectExtents", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DEmissionRectExtentsXTween), "EmissionRectExtents", "X", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DEmissionRectExtentsYTween), "EmissionRectExtents", "Y", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DColorTween), "Color", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles2DColorAlphaTween), "Color", "A", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DSpeedScaleTween), "SpeedScale", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DLifetimeTween), "Lifetime", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DExplosivenessTween), "Explosiveness", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DRandomnessTween), "Randomness", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DDirectionTween), "Direction", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DDirectionXTween), "Direction", "X", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DDirectionYTween), "Direction", "Y", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DDirectionZTween), "Direction", "Z", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DGravityTween), "Gravity", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DGravityXTween), "Gravity", "X", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DGravityYTween), "Gravity", "Y", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DGravityZTween), "Gravity", "Z", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DSpreadTween), "Spread", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DEmissionSphereRadiusTween), "EmissionSphereRadius", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DEmissionBoxExtentsTween), "EmissionBoxExtents", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DEmissionBoxExtentsXTween), "EmissionBoxExtents", "X", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DEmissionBoxExtentsYTween), "EmissionBoxExtents", "Y", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DEmissionBoxExtentsZTween), "EmissionBoxExtents", "Z", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DColorTween), "Color", "", 0.2d, 0.6d];
        yield return [typeof(CpuParticles3DColorAlphaTween), "Color", "A", 0.2d, 0.6d];
        yield return [typeof(SpringArm3DSpringLengthTween), "SpringLength", "", 0.2d, 0.6d];
        yield return [typeof(DecalModulateTween), "Modulate", "", 0.2d, 0.6d];
        yield return [typeof(DecalModulateAlphaTween), "Modulate", "A", 0.2d, 0.6d];
        yield return [typeof(DecalSizeTween), "Size", "", 0.2d, 0.6d];
        yield return [typeof(DecalSizeXTween), "Size", "X", 0.2d, 0.6d];
        yield return [typeof(DecalSizeYTween), "Size", "Y", 0.2d, 0.6d];
        yield return [typeof(DecalSizeZTween), "Size", "Z", 0.2d, 0.6d];
        yield return [typeof(DecalEmissionEnergyTween), "EmissionEnergy", "", 0.2d, 0.6d];
        yield return [typeof(FogVolumeSizeTween), "Size", "", 0.2d, 0.6d];
        yield return [typeof(FogVolumeSizeXTween), "Size", "X", 0.2d, 0.6d];
        yield return [typeof(FogVolumeSizeYTween), "Size", "Y", 0.2d, 0.6d];
        yield return [typeof(FogVolumeSizeZTween), "Size", "Z", 0.2d, 0.6d];
        yield return [typeof(ScrollContainerScrollHorizontalTween), "ScrollHorizontal", "", 2d, 6d];
        yield return [typeof(ScrollContainerScrollVerticalTween), "ScrollVertical", "", 2d, 6d];
        yield return [typeof(Sprite2DFrameTween), "Frame", "", 2d, 6d];
        yield return [typeof(AnimatedSprite2DFrameTween), "Frame", "", 2d, 6d];
        yield return [typeof(AnimatedSprite3DFrameTween), "Frame", "", 2d, 6d];
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void AdapterAndExtensionHonorPropertyContract(Type adapter, string property, string component, double low, double high)
    {
        var args = adapter.BaseType!.GetGenericArguments();
        var method = typeof(ExpandedAdapterTests).GetMethod(nameof(Verify), BindingFlags.Instance | BindingFlags.NonPublic)!;
        try { method.MakeGenericMethod(args).Invoke(this, [adapter, property, component, low, high]); }
        catch (TargetInvocationException e) when (e.InnerException is not null)
        { System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw(); }
    }

    private void Verify<TNode, TValue>(Type adapter, string property, string component, double low, double high)
        where TNode : Node where TValue : struct
    {
        using var scene = new TestScene(godot, typeof(TNode));
        var node = (TNode)scene.Target;
        var prop = node.GetType().GetProperty(property)!;
        var initial = Sample(prop.PropertyType, low);
        var target = (TValue)Sample(typeof(TValue), high);
        prop.SetValue(node, initial);
        Close(initial, prop.GetValue(node)!);
        using var scheduler = new TweenScheduler();
        var definition = (TweenDefinition<TNode, TValue>)Activator.CreateInstance(adapter)!;
        definition.Duration = 1;
        definition.To = target;
        var handle = scheduler.Add(node, definition);
        scheduler.Update(0.5);
        var midpoint = Expected(initial, target, component, 0.5);
        Close(midpoint, prop.GetValue(node)!);
        scheduler.Update(0.5);
        Close(Expected(initial, target, component, 1), prop.GetValue(node)!);
        Assert.Equal(TweenState.Completed, handle.State);
        Assert.Equal(Reason.Completed, handle.CompletionReason);

        // Non-retaining completion restores the captured value, while cancellation retains its sample.
        prop.SetValue(node, initial);
        definition.Fill = FillMode.None;
        var restoring = scheduler.Add(node, definition);
        scheduler.Update(0.5);
        Close(midpoint, prop.GetValue(node)!);
        scheduler.Update(0.5);
        Close(initial, prop.GetValue(node)!);
        Assert.Equal(TweenState.Completed, restoring.State);
        var cancelled = scheduler.Add(node, definition);
        scheduler.Update(0.5);
        cancelled.Cancel();
        scheduler.Update(1);
        Close(midpoint, prop.GetValue(node)!);
        Assert.Equal(Reason.Cancelled, cancelled.CompletionReason);

        // Explicit From with omitted To returns to the value captured at addition.
        prop.SetValue(node, initial);
        definition.From = target;
        definition.To = null;
        definition.Fill = FillMode.RetainFinalValue;
        var returning = scheduler.Add(node, definition);
        scheduler.Update(0.5);
        Close(midpoint, prop.GetValue(node)!);
        scheduler.Update(0.5);
        Close(initial, prop.GetValue(node)!);
        Assert.Equal(TweenState.Completed, returning.State);

        // Exercise every convenience overload through the real automatic scheduler.
        var extension = Assert.Single(typeof(TweenExtensions).GetMethods(), m =>
            m.GetParameters().Length == 4 && m.GetParameters()[3].ParameterType == typeof(Action<>).MakeGenericType(adapter));
        var automatic = (TweenInstance)extension.Invoke(null, [node, target, 1d, null])!;
        TweenRuntime.GetRunner(node).Scheduler.Update(0.5);
        Close(midpoint, prop.GetValue(node)!);
        TweenRuntime.GetRunner(node).Scheduler.Update(0.5);
        Close(Expected(initial, target, component, 1), prop.GetValue(node)!);
        Assert.Equal(TweenState.Completed, automatic.State);
    }

    internal static object Sample(Type type, double v)
    {
        if (type == typeof(float)) return (float)v;
        if (type == typeof(double)) return v;
        if (type == typeof(int)) return (int)v;
        if (type == typeof(Vector2)) return new Vector2((float)v, (float)v + 0.07f);
        if (type == typeof(Vector3)) return new Vector3((float)v, (float)v + 0.07f, (float)v + 0.13f);
        if (type == typeof(Color)) return new Color((float)v, (float)v + 0.03f, (float)v + 0.07f, (float)v + 0.11f);
        if (type == typeof(Quaternion)) return new Quaternion(Vector3.Up, (float)v);
        if (type == typeof(Rect2)) return new Rect2((float)v, (float)v + 0.1f, (float)v + 0.2f, (float)v + 0.3f);
        throw new NotSupportedException(type.Name);
    }

    internal static object Expected(object initial, object target, string component, double weight)
    {
        if (component.Length > 0)
        {
            var field = initial.GetType().GetField(component)!;
            var result = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(initial);
            field.SetValue(result, (float)Mix(Convert.ToDouble(field.GetValue(initial)), Convert.ToDouble(target), weight));
            return result;
        }
        return initial switch
        {
            float x => (float)Mix(x, (float)target, weight),
            double x => Mix(x, (double)target, weight),
            int x => (int)Math.Round(Mix(x, (int)target, weight), MidpointRounding.AwayFromZero),
            Vector2 x => x * (float)(1 - weight) + (Vector2)target * (float)weight,
            Vector3 x => x * (float)(1 - weight) + (Vector3)target * (float)weight,
            Color x => x * (float)(1 - weight) + (Color)target * (float)weight,
            Quaternion x => new Quaternion(Vector3.Up, (float)Mix(x.GetAngle(), ((Quaternion)target).GetAngle(), weight)),
            Rect2 x => new Rect2((Vector2)Expected(x.Position, ((Rect2)target).Position, "", weight),
                (Vector2)Expected(x.Size, ((Rect2)target).Size, "", weight)),
            _ => throw new NotSupportedException(initial.GetType().Name),
        };
    }

    private static double Mix(double a, double b, double t) => a * (1 - t) + b * t;
    internal static void Close(object expected, object actual)
    {
        var equal = expected switch
        {
            float x => Math.Abs(x - (float)actual) < 0.0001,
            double x => Math.Abs(x - (double)actual) < 0.0001,
            int x => x == (int)actual,
            Vector2 x => x.DistanceTo((Vector2)actual) < 0.0001,
            Vector3 x => x.DistanceTo((Vector3)actual) < 0.0001,
            Color x => x.IsEqualApprox((Color)actual),
            Quaternion x => Math.Abs(x.Dot((Quaternion)actual)) > 0.99999f,
            Rect2 x => x.IsEqualApprox((Rect2)actual),
            _ => false,
        };
        Assert.True(equal, $"Expected {expected}, got {actual}.");
    }

    internal sealed class TestScene : IDisposable
    {
        private readonly Node root = new();
        private readonly List<Resource> resources = [];
        public Node Target { get; }
        public TestScene(HeadlessFixture godot, Type type)
        {
            var concrete = type == typeof(CanvasItem) ? typeof(Node2D)
                : type == typeof(Godot.Range) ? typeof(ProgressBar)
                : type == typeof(SpriteBase3D) ? typeof(Sprite3D)
                : type == typeof(GeometryInstance3D) ? typeof(MeshInstance3D)
                : type == typeof(Light2D) ? typeof(PointLight2D)
                : type == typeof(Light3D) ? typeof(OmniLight3D) : type;
            Target = (Node)Activator.CreateInstance(concrete)!;
            if (Target is Control control) { control.Size = new Vector2(100, 200); control.OffsetTransformEnabled = true; }
            if (Target is Godot.Range range) range.Step = 0;
            if (Target is Label label) label.Text = new string('a', 100);
            if (Target is RichTextLabel rich) rich.Text = new string('a', 100);
            if (Target is Sprite2D sprite) { sprite.Hframes = 10; sprite.RegionEnabled = true; }
            if (Target is AnimatedSprite2D or AnimatedSprite3D)
            {
                var frames = new SpriteFrames(); resources.Add(frames);
                for (var i = 0; i < 10; i++) frames.AddFrame("default", null!);
                if (Target is AnimatedSprite2D a) a.SpriteFrames = frames;
                if (Target is AnimatedSprite3D b) b.SpriteFrames = frames;
            }
            if (Target is GpuParticles2D gpu2) gpu2.Emitting = false;
            if (Target is GpuParticles3D gpu3) gpu3.Emitting = false;
            if (Target is CpuParticles2D cpu2) cpu2.Emitting = false;
            if (Target is CpuParticles3D cpu3) cpu3.Emitting = false;
            if (Target is PathFollow2D follow2)
            {
                var curve = new Curve2D(); resources.Add(curve);
                curve.AddPoint(Vector2.Zero); curve.AddPoint(new Vector2(100, 0));
                var path = new Path2D { Curve = curve }; root.AddChild(path); path.AddChild(follow2); follow2.Loop = false;
            }
            else if (Target is PathFollow3D follow3)
            {
                var curve = new Curve3D(); resources.Add(curve);
                curve.AddPoint(Vector3.Zero); curve.AddPoint(new Vector3(100, 0, 0));
                var path = new Path3D { Curve = curve }; root.AddChild(path); path.AddChild(follow3); follow3.Loop = false;
            }
            else root.AddChild(Target);
            godot.Tree.Root.AddChild(root);
            if (Target is ScrollContainer scroll)
            {
                scroll.GetHScrollBar().MaxValue = 1000; scroll.GetHScrollBar().Page = 100;
                scroll.GetVScrollBar().MaxValue = 1000; scroll.GetVScrollBar().Page = 100;
            }
        }
        public void Dispose()
        {
            root.Free();
            foreach (var resource in resources) resource.Dispose();
        }
    }
}
