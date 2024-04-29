using System.Numerics;
using GAIL.Core;

namespace GAIL.Graphics
{
    /// <summary>
    /// A material for color, texture and transform only (default).
    /// </summary>
    public class BasicMaterial : IMaterial {
        /// <summary>
        /// The optional texture.
        /// </summary>
        public Texture? texture;
        /// <summary>
        /// The color to display (if no texture).
        /// </summary>
        public Color color;
        /// <summary>
        /// The model view projection matrix.
        /// </summary>
        public Matrix4x4 mvp;
        /// <summary>
        /// Creates a basic material from a color, texture and model matrix, and some camera things.
        /// </summary>
        /// <param name="color">The color to display (if no texture).</param>
        /// <param name="texture">The optional texture.</param>
        /// <param name="modelMatrix">The matrix for local coordinates to world coordinates.</param>
        /// <param name="viewMatrix">The position and rotation of the camera (offset).</param>
        /// <param name="projectionMatrix">The properties of the camera (far plane, near plane, FoV, etc).</param>
        public BasicMaterial(Color color, Texture? texture, Matrix4x4 modelMatrix, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix) {
            this.texture = texture;
            this.color = color;
            mvp = modelMatrix * viewMatrix * projectionMatrix;
        }

        /// <summary>
        /// Creates a basic material from a color, texture and transform, and some camera things.
        /// </summary>
        /// <param name="color">The color to display (if no texture).</param>
        /// <param name="texture">The optional texture.</param>
        /// <param name="transform">The transform of the model.</param>
        /// <param name="viewMatrix">The position and rotation of the camera (offset).</param>
        /// <param name="projectionMatrix">The properties of the camera (far plane, near plane, FoV, etc).</param>
        public BasicMaterial(Color color, Texture? texture, Transform transform, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix) : this(color, texture, transform.ToModelMatrix(), viewMatrix, projectionMatrix) { }

        /// <inheritdoc/>
        public Shader GetShader() {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Use() {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// A material that is rendered flat on the screen (e.g. UI).
    /// </summary>
    public class Basic2DMaterial : IMaterial {
        /// <summary>
        /// The texture to display.
        /// </summary>
        public Texture? texture;
        /// <summary>
        /// The color to display (if no texture).
        /// </summary>
        public Color color;
        
        /// <summary>
        /// Creates a basic 2D material, with a texture and a color.
        /// </summary>
        /// <param name="color">The color if the texture is null.</param>
        /// <param name="texture">The optional texture.</param>
        public Basic2DMaterial(Color color, Texture? texture) {
            this.texture = texture;
            this.color = color;
        }

        /// <inheritdoc/>
        public Shader GetShader() {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Use() {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Can render basic text in 2D.
    /// </summary>
    
    public class Text2DMaterial : IMaterial {
        /// <summary>
        /// The text to display.
        /// </summary>
        public string text;
        /// <summary>
        /// The font to use.
        /// </summary>
        public Dictionary<char, Texture> font;

        /// <summary>
        /// Creates a 2D text material.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="font">The font to use.</param>
        public Text2DMaterial(string text, Dictionary<char, Texture> font) {
            this.text = text;
            this.font = font;
        }

        /// <inheritdoc/>
        public Shader GetShader() {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Use() {
            throw new NotImplementedException();
        }
    }
}
