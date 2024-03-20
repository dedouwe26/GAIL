namespace GAIL.Graphics
{
    /// <summary>
    /// This is how anything is drawn to the window, contains the shader and shader data (instanced, per model different).
    /// </summary>
    public interface IMaterial {
        /// <summary>
        /// Sets all the uniforms and attributes (used before rendering).
        /// </summary>
        public void Use();

        /// <summary>
        /// Gives the shader.
        /// </summary>
        /// <returns>The shader to use.</returns>
        public Shader GetShader();
        
    }
}