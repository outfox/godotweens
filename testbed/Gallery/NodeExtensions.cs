// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
namespace testbed;

public static class NodeExtensions
{
    /// <summary>AddChild that returns the child, so a node can be initialized, attached and kept in one expression.</summary>
    public static T Add<T>(this Node parent, T child) where T : Node
    {
        parent.AddChild(child);
        return child;
    }
}
