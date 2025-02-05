using GAIL.Core;
using GAIL.Window;
using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace GAIL.Graphics.Renderer.Vulkan
{
    public class Surface : IDisposable {
        /// <summary>
        /// If this class is already disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }
        public readonly KhrSurface surfaceExtension;
        public readonly SurfaceKHR surface;
        private readonly Instance instance;
        public Surface(VulkanRenderer renderer, WindowManager window) {
            renderer.Logger.LogDebug("Creating Surface.");

            instance = renderer.instance;

            if (!API.Vk.TryGetInstanceExtension(instance, out surfaceExtension)) {
                renderer.Logger.LogFatal("Vulkan: Failed at getting Surface extension!");
                throw new APIBackendException("Vulkan", "Failed at getting VK_KHR_surface extension.");
            }

            unsafe {
                VkNonDispatchableHandle* surfacePtr = stackalloc VkNonDispatchableHandle[1];

                _ = Utils.Check((Result)API.Glfw.CreateWindowSurface(((Silk.NET.Vulkan.Instance)instance).ToHandle(), window.Window, Allocator.allocatorPtr, surfacePtr), renderer.Logger, "Failed to create window surface", true);
                
                Glfw.ThrowExceptions();
                
                surface = surfacePtr[0].ToSurface();
            }
            
        }

        /// <inheritdoc/>
        public void Dispose() {
            if (IsDisposed) { return; }
            
            unsafe {
                surfaceExtension.DestroySurface(instance, surface, Allocator.allocatorPtr);
            }
            surfaceExtension.Dispose();
            
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}