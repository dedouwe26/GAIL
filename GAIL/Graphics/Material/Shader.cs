namespace GAIL.Graphics.Material
{
    /// <summary>
    /// Contains code for the GPU on how everything is drawn (per-material).
    /// </summary>
    public class Shader : IDisposable {
        /// <summary>
        /// The attribute layout of this shader.
        /// </summary>
        public List<string> attributeLayout = [];
        /// <summary>
        /// The uniform layout of this shader.
        /// </summary>
        public List<Uniform> uniforms = [];
        /// <summary>
        /// Creates a shader.
        /// </summary>
        /// <param name="vertexShader">The per-vertex shader (in SPIR-V compiled).</param>
        /// <param name="fragmentShader">The per-pixel shader (in SPIR-V compiled).</param>
        /// <param name="geometryShader">The geometry shader (in SPIR-V compiled).</param>
        public Shader(byte[] vertexShader, byte[] fragmentShader, byte[]? geometryShader = null) {
            
        }
        /// <summary>
        /// Calls <see cref="Dispose"/>.
        /// </summary>
        ~Shader() { Dispose(); }

        /// <summary>
        /// Adds a uniform value to the shader.
        /// </summary>
        /// <param name="uniform">The uniform to sent to the shader.</param>
        public void SetUniform(Uniform uniform) {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sets the attribute layout of the shader. 
        /// When it's being rendered, it will only take the specified attributes (with those names).
        /// </summary>
        /// <param name="layout">Which value to use.</param>
        public void SetAttributeLayout(List<string> layout) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
        
    }
}