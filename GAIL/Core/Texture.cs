namespace GAIL.Core
{
    /// <summary>
    /// A texture rendered with the GPU.
    /// </summary>
    /// <param name="width">The width of the texture.</param>
    /// <param name="height">The height of the texture.</param>
    public class Texture(int width, int height) {
        public int width;
        public int height;
        /// <summary>
        /// Format: each row has the length of the width. Height is the amount of rows.
        /// row0 + row1 + row2 + row[height]
        /// </summary>
        public Color[] colors = new Color[width * height];
        public Color GetColor(int x, int y) {
            if (x >= width||y >= height) { throw new IndexOutOfRangeException(); }
            return colors[(y*width)+x];
        }
        public static Texture FromPNG(string path) {
            return PNG.Parse(path);
        }
    }
}