using GAIL.Core;
using GAIL.Window;
using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;

namespace GAIL.Graphics.Utils
{
    public class Surface : IDisposable {
        public KhrSurface surfaceExtension;
        public SurfaceKHR surface;
        private readonly Instance instance;
        public Surface(Vk vk, Instance instance, WindowManager window) {
            this.instance = instance;
            if (!vk.TryGetInstanceExtension(instance, out surfaceExtension)) {
                throw new APIBackendException("Vulkan", "Failed to get VK_KHR_surface extension.");
            }
            unsafe {
                VkNonDispatchableHandle* surfacePtr = stackalloc VkNonDispatchableHandle[1];
                int errorCode;
                if ((errorCode = window.glfw.CreateWindowSurface(instance.ToHandle(), window.Window, null, surfacePtr))!=0) {
                    throw new APIBackendException("GLFW", "Failed to create surface: "+errorCode);
                }
                Glfw.ThrowExceptions();
                surface = surfacePtr[0].ToSurface();
            }
            
        }
        public void Dispose() {
            unsafe {
                surfaceExtension.DestroySurface(instance, surface, null);
            }
            surfaceExtension.Dispose();
        }
    }
}