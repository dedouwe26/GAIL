using System.Runtime.CompilerServices;
using Silk.NET.GLFW;

namespace GAIL.Core
{
    /// <summary>
    /// A texture rendered with the GPU.
    /// </summary>
    /// <param name="width">The width of the texture.</param>
    /// <param name="height">The height of the texture.</param>
    public class Texture(uint width, uint height) {
        public uint width = width;
        public uint height = height;
        /// <summary>
        /// Format: each row has the length of the width. Height is the amount of rows.
        /// row0 + row1 + row2 + row[height]
        /// </summary>
        public Color[] colors = new Color[width * height];
        /// <summary>
        /// Returns the color at the given position.
        /// </summary>
        /// <param name="x">The x (horizontal) coordinate.</param>
        /// <param name="y">The y (vertical) coordinate.</param>
        /// <returns>The color at the given position.</returns>
        /// <exception cref="IndexOutOfRangeException">outside file</exception>
        public Color GetColor(uint x, uint y) {
            if (x >= width||y >= height) { throw new IndexOutOfRangeException(); }
            return colors[(y*width)+x];
        }
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
                    return new Image{Width = (int)width, Height = (int)height, Pixels = pixelArray};
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
                    return new Image{Width = (int)width, Height = (int)height, Pixels = pixelArray};
                }
            }
            
        }
    }
}