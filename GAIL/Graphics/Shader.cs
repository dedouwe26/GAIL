namespace GAIL.Graphics
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
        public List<(AttributeType type, byte[] value)> uniforms = [];
        /// <summary>
        /// Creates a shader.
        /// </summary>
        /// <param name="vertexShader">The per-vertex shader (in SPIR-V compiled).</param>
        /// <param name="fragmentShader">The per-pixel shader (in SPIR-V compiled).</param>
        /// <param name="geometryShader">The geometry shader (in SPIR-V compiled).</param>
        public Shader(byte[] vertexShader, byte[] fragmentShader, byte[]? geometryShader = null) {
            
        }
        ~Shader() { Dispose(); }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Adds a uniform value to the shader (first uniform set will be at position 0).
        /// </summary>
        /// <param name="type">The type of the uniform.</param>
        /// <param name="value">The value of the uniform.</param>
        public void SetUniform(AttributeType type, byte[] value) {
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
    }
}