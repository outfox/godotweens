// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Reflection;
using Godot;
using tweens.gd;
using twodog.Testing;
using twodog.Testing.Xunit;
using static testbed.Tests.Integration.ExpandedAdapterTests;

namespace testbed.Tests.Integration;

[Collection<HeadlessCollection>]
public class MaterialAdapterTests(HeadlessFixture godot)
{
    public static IEnumerable<object[]> Cases()
    {
        yield return [typeof(MaterialAlbedoColorTween), "AlbedoColor", ""];
        yield return [typeof(MaterialAlbedoAlphaTween), "AlbedoColor", "A"];
        yield return [typeof(MaterialMetallicTween), "Metallic", ""];
        yield return [typeof(MaterialMetallicSpecularTween), "MetallicSpecular", ""];
        yield return [typeof(MaterialRoughnessTween), "Roughness", ""];
        yield return [typeof(MaterialEmissionEnergyMultiplierTween), "EmissionEnergyMultiplier", ""];
        yield return [typeof(MaterialEmissionIntensityTween), "EmissionIntensity", ""];
        yield return [typeof(MaterialNormalScaleTween), "NormalScale", ""];
        yield return [typeof(MaterialEmissionTween), "Emission", ""];
        yield return [typeof(MaterialUv1OffsetTween), "Uv1Offset", ""];
        yield return [typeof(MaterialUv1OffsetXTween), "Uv1Offset", "X"];
        yield return [typeof(MaterialUv1OffsetYTween), "Uv1Offset", "Y"];
        yield return [typeof(MaterialUv1OffsetZTween), "Uv1Offset", "Z"];
        yield return [typeof(MaterialUv1ScaleTween), "Uv1Scale", ""];
        yield return [typeof(MaterialUv1ScaleXTween), "Uv1Scale", "X"];
        yield return [typeof(MaterialUv1ScaleYTween), "Uv1Scale", "Y"];
        yield return [typeof(MaterialUv1ScaleZTween), "Uv1Scale", "Z"];
        yield return [typeof(MaterialUV2OffsetTween), "UV2Offset", ""];
        yield return [typeof(MaterialUV2OffsetXTween), "UV2Offset", "X"];
        yield return [typeof(MaterialUV2OffsetYTween), "UV2Offset", "Y"];
        yield return [typeof(MaterialUV2OffsetZTween), "UV2Offset", "Z"];
        yield return [typeof(MaterialUV2ScaleTween), "UV2Scale", ""];
        yield return [typeof(MaterialUV2ScaleXTween), "UV2Scale", "X"];
        yield return [typeof(MaterialUV2ScaleYTween), "UV2Scale", "Y"];
        yield return [typeof(MaterialUV2ScaleZTween), "UV2Scale", "Z"];
    }
    [Fact]
    public void EveryConcreteMaterialAdapterHasAContract()
    {
        var adapters = typeof(MaterialRoughnessTween).Assembly.GetTypes().Where(t => !t.IsAbstract && !t.ContainsGenericParameters
            && t.BaseType is { IsGenericType: true } b && b.GetGenericTypeDefinition() == typeof(PropertyTween<,>)
            && typeof(Material).IsAssignableFrom(b.GetGenericArguments()[0]));
        Assert.Equal(adapters.OrderBy(t => t.Name), Cases().Select(c => (Type)c[0]).OrderBy(t => t.Name));
    }
    [Theory, MemberData(nameof(Cases))]
    public void PropertyAndBothExtensionsHonorContract(Type adapter, string property, string component)
    {
        var method = GetType().GetMethod(nameof(Verify), BindingFlags.Instance | BindingFlags.NonPublic)!;
        try { method.MakeGenericMethod(adapter.BaseType!.GetGenericArguments()[1]).Invoke(this, [adapter, property, component]); }
        catch (TargetInvocationException e) when (e.InnerException is not null)
        { System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw(); }
    }
    private void Verify<T>(Type adapter, string property, string component) where T : struct
    {
        using var previousPhysicalUnits = ProjectSettings.GetSetting("rendering/lights_and_shadows/use_physical_light_units");
        ProjectSettings.SetSetting("rendering/lights_and_shadows/use_physical_light_units", true);
        try
        {
            foreach (BaseMaterial3D material in new BaseMaterial3D[] { new StandardMaterial3D(), new OrmMaterial3D() })
            using (material)
            using (var scheduler = new TweenScheduler())
            {
                var prop = typeof(BaseMaterial3D).GetProperty(property)!;
                var initial = Sample(prop.PropertyType, 0.2);
                var to = (T)Sample(typeof(T), 0.6);
                var midpoint = Expected(initial, to, component, 0.5);
                var definition = (TweenDefinition<BaseMaterial3D, T>)Activator.CreateInstance(adapter)!;
                definition.Duration = 1; definition.To = to;
                prop.SetValue(material, initial);
                var handle = scheduler.Add(material, definition);
                scheduler.Update(0.5); Close(midpoint, prop.GetValue(material)!);
                scheduler.Update(0.5); Close(Expected(initial, to, component, 1), prop.GetValue(material)!);
                Assert.Equal(TweenState.Completed, handle.State);
                prop.SetValue(material, initial);
                definition.Fill = FillMode.None;
                var restoring = scheduler.Add(material, definition);
                scheduler.Update(1); Close(initial, prop.GetValue(material)!);
                Assert.Equal(TweenState.Completed, restoring.State);
                var cancelled = scheduler.Add(material, definition);
                scheduler.Update(0.5); cancelled.Cancel(); scheduler.Update(1);
                Close(midpoint, prop.GetValue(material)!);
                prop.SetValue(material, initial);
                definition.From = to; definition.To = null; definition.Fill = FillMode.RetainFinalValue;
                var returning = scheduler.Add(material, definition);
                scheduler.Update(0.5); Close(midpoint, prop.GetValue(material)!);
                scheduler.Update(0.5); Close(initial, prop.GetValue(material)!);
                Assert.Equal(TweenState.Completed, returning.State);
                var methods = typeof(TweenExtensions).GetMethods().Where(m => m.GetParameters().Any(p => p.ParameterType == typeof(Action<>).MakeGenericType(adapter))).ToArray();
                Assert.Equal(2, methods.Length);
                var owner = new Node(); godot.Tree.Root.AddChild(owner);
                try
                {
                    foreach (var method in methods)
                    {
                        prop.SetValue(material, initial);
                        var treeScoped = method.GetParameters()[3].ParameterType == typeof(SceneTree);
                        var automatic = (TweenInstance)method.Invoke(null, treeScoped
                            ? [material, to, 1d, godot.Tree, null, null] : [material, to, 1d, owner, null])!;
                        TweenRuntime.GetRunner(godot.Tree).Scheduler.Update(0.5); Close(midpoint, prop.GetValue(material)!);
                        TweenRuntime.GetRunner(godot.Tree).Scheduler.Update(0.5);
                        Close(Expected(initial, to, component, 1), prop.GetValue(material)!);
                        Assert.Equal(TweenState.Completed, automatic.State);
                    }
                }
                finally { owner.Free(); }
            }
        }
        finally { ProjectSettings.SetSetting("rendering/lights_and_shadows/use_physical_light_units", previousPhysicalUnits); }
    }
}
