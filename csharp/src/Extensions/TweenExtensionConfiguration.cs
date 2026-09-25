// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace tweens.gd;

public static partial class TweenExtensions
{
    private static TDefinition ConfigureDefinition<TDefinition>(TDefinition definition, Action<TDefinition>? configure)
        where TDefinition : TweenOptions
    {
        configure?.Invoke(definition);
        return definition;
    }

    // The explicit duration argument takes precedence over options.Duration.
    private static TDefinition ApplyOptions<TDefinition>(TDefinition definition, TweenOptions options)
        where TDefinition : TweenOptions
    {
        ArgumentNullException.ThrowIfNull(options);
        var duration = definition.Duration;
        options.CopyTo(definition);
        definition.Duration = duration;
        return definition;
    }
}
