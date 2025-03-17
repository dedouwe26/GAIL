using Silk.NET.GLFW;
using Silk.NET.OpenAL;
using Silk.NET.Vulkan;

namespace GAIL {
    
    /// <summary>
    /// Contains all the backend APIs.
    /// </summary>
    public sealed class API : IDisposable {
        static API() {
            Instance = new API();
        }
        private API() { }
        /// <summary>
        /// Instance of API holder.
        /// </summary>
        public static API Instance { get; private set; }
        /// <summary>
        /// The GLFW API.
        /// </summary>
        public static Glfw Glfw {get {return Instance.glfw;}}
        /// <summary>
        /// The OpenAL API.
        /// </summary>
        public static AL Al {get {return Instance.al;}}
        /// <summary>
        /// The OpenAL Context API.
        /// </summary>
        public static ALContext Alc {get {return Instance.alc;}}
        /// <summary>
        /// The Vulkan API.
        /// </summary>
        public static Vk Vk {get {return Instance.vk;}}
        
        private readonly Glfw glfw = Glfw.GetApi();
        private readonly AL al = AL.GetApi();
        private readonly ALContext alc = ALContext.GetApi();
        private readonly Vk vk = Vk.GetApi();

        /// <summary>Disposes all api's.</summary>
        ~API() {
            SubDispose();
        }

        /// <inheritdoc/>
        public void Dispose() {
            SubDispose();
            GC.SuppressFinalize(this);
        }
        private void SubDispose() {
            vk.Dispose();
            glfw.Dispose();
            alc.Dispose();
            al.Dispose();
        }
    }

}