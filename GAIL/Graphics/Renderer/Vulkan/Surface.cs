using GAIL.Core;
using GAIL.Window;
using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace GAIL.Graphics.Renderer.Vulkan
{
    public class Surface : IDisposable {
        public KhrSurface surfaceExtension;
        public SurfaceKHR surface;
        private readonly Instance instance;
        public Surface(VulkanRenderer renderer, WindowManager window) {
            renderer.Logger.LogDebug("Creating Surface.");

            instance = renderer.instance!;

            if (!API.Vk.TryGetInstanceExtension(instance, out surfaceExtension)) {
                renderer.Logger.LogFatal("Vulkan: Failed to get Surface extension!");
                throw new APIBackendException("Vulkan", "Failed to get VK_KHR_surface extension.");
            }

            unsafe {
                VkNonDispatchableHandle* surfacePtr = stackalloc VkNonDispatchableHandle[1];

                _ = Utils.Check((Result)API.Glfw.CreateWindowSurface(((Silk.NET.Vulkan.Instance)instance).ToHandle(), window.Window, null, surfacePtr), renderer.Logger, "Failed to create window surface", true);
                
                Glfw.ThrowExceptions();
                
                surface = surfacePtr[0].ToSurface();
            }
            
        }

        /// <inheritdoc/>
        public void Dispose() {
            unsafe {
                surfaceExtension.DestroySurface(instance, surface, null);
            }
            surfaceExtension.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}