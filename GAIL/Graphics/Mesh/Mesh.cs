using System.Collections.Immutable;
using GAIL.Core.File;
using GAIL.Graphics.Material;

namespace GAIL.Graphics.Mesh
{
	/// <summary>
	/// 3D or 2D shape data.
	/// </summary>
	public class Mesh {
		/// <summary>
		/// Returns a mesh constructed from a Wavefront obj file.
		/// </summary>
		/// <remarks>
		/// The attributes for each vertex that can be added are:
		/// <list type="bullet">
		///     <item>0: PositionAttribute</item>
		///     <item>1: NormalAttribute</item>
		///     <item>2: UVAttribute</item>
		/// </list>
		/// </remarks>
		/// <param name="path">The path where the mesh is.</param>
		/// <returns>The parsed Mesh.</returns>
		public static Mesh FromObj(string path) { return Obj.Parse(path); }

		/// <summary>
		/// Creates the raw data of the mesh ready of the shader to use.
		/// </summary>
		/// <param name="shader">The shader for which this mesh should be compiled.</param>
		/// <param name="buffer">The baked mesh.</param>
		public void Bake(IShader shader, out byte[] buffer) { // TODO: add index faces, optimize
			ImmutableArray<FormatInfo> requiredAttributes = shader.RequiredAttributes;
			uint size = 0;
			foreach (FormatInfo f in requiredAttributes) {
				size += f.size;
			}
			uint offset = 0;
			buffer = new byte[size*vertices.Length];
			byte[] attributeBuffer;
			foreach (Vertex vertex in vertices) {
				foreach (VertexAttribute attribute in vertex.Supply(requiredAttributes)) {
					attributeBuffer = attribute.Use();
					Array.Copy(attributeBuffer, 0, buffer, offset, attributeBuffer.Length);
					offset += (uint)attributeBuffer.Length;
				}
			}
		}

		/// <summary>
		/// The vertices for the <see cref="indexFaces"/>.
		/// </summary>
		public Vertex[] vertices;

		/// <summary>
		/// The indices that make the faces in the following format: (Face0-p1, Face0-p2, Face0-p3, Face1-p1, ...).
		/// </summary>
		public uint[]? indexFaces;


		/// <summary>
		/// Creates an mesh from faces.
		/// </summary>
		/// <param name="faces">The faces to create from.</param>
		public Mesh(List<Face> faces) {
			throw new NotImplementedException(); // TODO: parse faces to index faces.
		}
		/// <summary>
		/// Creates a mesh from index faces.
		/// </summary>
		/// <param name="vertices">The vertices for the indices.</param>
		/// <param name="indexFaces">The face indices.</param>
		public Mesh(Vertex[] vertices, uint[]? indexFaces = null) {
			this.vertices = vertices;
			this.indexFaces = indexFaces;
		}
	}
}