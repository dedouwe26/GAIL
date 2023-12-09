#pragma once

#include <map>
#include "Shader.hpp"

namespace GAIL
{

    // A point that exists of data (used for 3d objects).
    class Vertex
    {
        public:
            Vector3 position;
            // Attributes for this vertex shown in shader.
            // Name and VertexAttribute.
            std::map<string, VertexAttribute> attributes;
            Vertex() {this->position={0,0,0};};
            Vertex(double x, double y, double z) {this->position={x,y,z};};
            Vertex(double x, double y) {this->position = {x,y,0};};
            ~Vertex();
    };

    // A index that represents a vertex, used for multiple points in one place.
    class Index {
        public:
            int index = -1;
            Vertex *vertex;
            // Index num is calculated at render.
            Index(Vertex *vertex);
            // Static index (index is not calculated at render).
            Index(Vertex *vertex, int index);
            ~Index();
    };

    // A face (triangle) of a mesh.
    class Face
    {
        public:
            Index *Indices[3];
            Face(Index *indices[3]);
            Face(Index p1, Index p2, Index p3);
            ~Face();
    };

    // A 3D / 2D shape data.
    class Mesh
    {
        public:
            std::vector<Face> Faces;
            Mesh(std::vector<Face> faces);
            ~Mesh();
            static Mesh FromObj(string path);
    };

    /*
     * A model in 3D / 2D space based on a mesh.
     * This can be rendered.
     */
    class Model
    {
        public:
            Mesh mesh;
            BaseMaterial material;
            Model(Mesh mesh, BaseMaterial material);
            ~Model();
    };

} // namespace GAIL
