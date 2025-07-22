using GAIL.Core;
using GAIL.Window;
using LambdaKit.Logging;
using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;

namespace GAIL.Graphics.Renderer.Vulkan
{
    public class Surface : IDisposable {
        /// <summary>
        /// If this class is already disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }
        private KhrSurface? extension;
        public KhrSurface Extension { get {
            if (extension == null) {
                if (!API.Vk.TryGetInstanceExtension(instance, out extension) || extension == null) {
                    Logger.LogFatal("Vulkan: Failed at getting Surface extension!");
                    throw new APIBackendException("Vulkan", "Failed at getting VK_KHR_surface extension.");
                }
            }
            return extension;
        } }
        public readonly SurfaceKHR surface;
        private readonly Instance instance;
        private readonly Logger Logger;
        public Surface(VulkanRenderer renderer, WindowManager window) {
            Logger = renderer.Logger;
            renderer.Logger.LogDebug("Creating Surface.");

            instance = renderer.instance;

            unsafe {
                VkNonDispatchableHandle* surfacePtr = stackalloc VkNonDispatchableHandle[1];

                _ = Utils.Check((Result)API.Glfw.CreateWindowSurface(((Silk.NET.Vulkan.Instance)instance).ToHandle(), window.Window, Allocator.allocatorPtr, surfacePtr), Logger, "Failed to create window surface", true);
                
                Glfw.ThrowExceptions();
                
                surface = surfacePtr[0].ToSurface();
            }
            
        }

        /// <inheritdoc/>
        public void Dispose() {
            if (IsDisposed) { return; }
            
            unsafe {
                Extension.DestroySurface(instance, surface, Allocator.allocatorPtr);
            }
            extension?.Dispose();
            
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}