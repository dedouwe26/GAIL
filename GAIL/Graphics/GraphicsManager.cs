using Silk.NET.Vulkan;

namespace GAIL.Graphics
{
    /// <summary>
    /// This handles all the graphics of GAIL.
    /// </summary>
    public class GraphicsManager : IManager {
        /// <summary>
        /// Vulkan Instance.
        /// </summary>
        public Instance instance;
        /// <summary>
        /// Vulkan Physical Device.
        /// </summary>
        public PhysicalDevice physicalDevice;
        /// <summary>
        /// Vulkan Logical Device.
        /// </summary>
        public Device device;
        /// <summary>
        /// Current MSAA size, use setMSAA to change it.
        /// </summary>
        public MSAA MSAAsize = MSAA.MSAAx1;

        public GraphicsManager() {
            
        }

        ~GraphicsManager() {
            Dispose();
        }

        public void Dispose() {
            
        }
        public MSAA GetMaxMSAA() {

        }
        public void SetMSAA() {

        }
        public void Render3D(List<Model> models) {

        }
        public void Render3DInstanced(List<InstancedModel> models) {

        }
        public void Render2D(List<Model> models) {

        }
        public void Render2DInstanced(List<InstancedModel> models) {
            
        }
    }
}