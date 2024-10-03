using GAIL.Core;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

/// <summary>
/// Contains code for Vulkan Memory Allocation.
/// </summary>
public static class Allocator {
    static Allocator() {
        allocator = null;
        allocatorPtr = Pointer<AllocationCallbacks>.FromNull();
    }
    /// <summary>
    /// The allocator itself.
    /// </summary>
    public static readonly AllocationCallbacks? allocator;
    /// <summary>
    /// The pointer to the allocator.
    /// </summary>
    public static readonly Pointer<AllocationCallbacks> allocatorPtr;
}