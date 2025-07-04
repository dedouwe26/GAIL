using Silk.NET.Assimp;
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
        /// <summary>
        /// The Assimp API.
        /// </summary>
        public static Assimp Assimp {get {return Instance.assimp;}}
        private readonly Glfw glfw = Glfw.GetApi();
        private readonly AL al = AL.GetApi();
        private readonly ALContext alc = ALContext.GetApi();
        private readonly Vk vk = Vk.GetApi();
        private readonly Assimp assimp = Assimp.GetApi();

        /// <summary>Disposes all API's.</summary>
        ~API() {
            Dispose(true);
        }

        /// <inheritdoc/>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool dispose = false) {
            if (dispose) {
                vk.Dispose();
                glfw.Dispose();
                alc.Dispose();
                al.Dispose();
            }
        }
    }

}