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
}
