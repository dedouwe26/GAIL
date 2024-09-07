using GAIL.Core;
using GAIL.Window;
using OxDED.Terminal.Logging;
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
        public Surface(Instance instance, Logger logger, WindowManager window) {
            logger.LogDebug("Creating Surface.");
            this.instance = instance;
            if (!API.Vk.TryGetInstanceExtension(instance, out surfaceExtension)) {
                logger.LogFatal("Vulkan: Failed to get Surface extension!");
                throw new APIBackendException("Vulkan", "Failed to get VK_KHR_surface extension.");
            }
            unsafe {
                VkNonDispatchableHandle* surfacePtr = stackalloc VkNonDispatchableHandle[1];
                int errorCode;
                if ((errorCode = API.Glfw.CreateWindowSurface(((Silk.NET.Vulkan.Instance)instance).ToHandle(), window.Window, null, surfacePtr))!=0) {
                    logger.LogFatal("Vulkan: Failed to create surface: "+errorCode);
                    throw new APIBackendException("GLFW", "Failed to create surface: "+errorCode);
                }
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