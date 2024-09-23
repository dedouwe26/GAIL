using GAIL.Core.File;
using Silk.NET.GLFW;

namespace GAIL.Core
{
    /// <summary>
    /// A image or texture.
    /// </summary>
    public class Texture : IEquatable<Texture> {
        /// <summary>
        /// The width of this texture.
        /// </summary>
        public readonly uint Width;
        /// <summary>
        /// The height of this texture;
        /// </summary>
        public readonly uint Height;
        /// <summary>
        /// Format: each row has the length of the width. Height is the amount of rows.
        /// row0 + row1 + row2 + row[height]
        /// </summary>
        public Color[] colors;

        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        public Texture(uint width, uint height) {
            Width = width;
            Height = height;
            colors = new Color[width * height];
        }

        /// <summary>
        /// Returns the color at the given position.
        /// </summary>
        /// <param name="x">The x (horizontal) coordinate.</param>
        /// <param name="y">The y (vertical) coordinate.</param>
        /// <returns>The color at the given position.</returns>
        /// <exception cref="IndexOutOfRangeException">outside file</exception>
        public Color GetColor(uint x, uint y) {
            if (x >= Width||y >= Height) { throw new IndexOutOfRangeException(); }
            return colors[(y*Width)+x];
        }

        /// <summary>
        /// Creates a texture from an PNG file.
        /// </summary>
        /// <param name="path">The path to the PNG file.</param>
        /// <returns>The texture of the PNG file.</returns>
        public static Texture FromPNG(string path) {
            return PNG.Parse(path);
        }
        /// <summary>
        /// Creates a GLFW image from this texture (RGBa).
        /// </summary>
        /// <returns>The GLFW image for this texture (RGBa).</returns>
        public Image ToGLFW() {
            unsafe {
                List<byte> byteList = [];
                foreach (Color color in colors) {
                    byteList = [.. byteList, .. color.ToBytes()];
                }
                fixed (byte* pixelArray = byteList.ToArray()) {
                    return new Image{Width = (int)Width, Height = (int)Height, Pixels = pixelArray};
                }
            }
            
        }
        /// <summary>
        /// Creates a GLFW image from this texture (RGB).
        /// </summary>
        /// <returns>The GLFW image for this texture (RGB).</returns>
        public Image ToGLFWRGB() {
            unsafe {
                List<byte> byteList = [];
                foreach (Color color in colors) {
                    byteList = [.. byteList, .. color.ToBytes()];
                }
                fixed (byte* pixelArray = byteList.ToArray()) {
                    return new Image{Width = (int)Width, Height = (int)Height, Pixels = pixelArray};
                }
            }
        }

        /// <inheritdoc/>
        public bool Equals(Texture? other) {
            if (other == null) { return false; }
            return colors==other.colors;
        }
        /// <inheritdoc/>
        public override bool Equals(object? obj) {
            return Equals(obj as Texture);
        }
        
        ///
        public static bool operator ==(Texture? a, Texture? b) {
            if (a==null&&b==null) { return true; }
            if (a==null) { return false; }
            return a.Equals(b);
        }
        ///
        public static bool operator !=(Texture? a, Texture? b) {
            if (a==null&&b==null) { return false; }
            if (a==null) { return true; }
            return !a.Equals(b);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            return colors.GetHashCode();
        }
    }
}