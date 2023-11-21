#pragma once

#include <list>
#include <map>
#include "Shader.hpp"

namespace GAIL
{

    // A point that exists of data (used for 3d objects).
    class Vertex
    {
    public:
        Vector3 position;
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
            Index(Vertex *vertex);
            Index(Vertex *vertex, int index);
            ~Index();
    };

    // A face (triangle) of a mesh.
    class Face
    {
        private:
            Index *Indices[3];
        public:
            Face(Index *indices[3]);
            ~Face();
    };

    // A 3D / 2D shape data.
    class Mesh
    {
        private:
            std::list<Face> Faces;
            std::list<Index> Indices;
        public:
            Mesh(std::list<Face> faces, std::list<Index> indices);
            ~Mesh();
            static Mesh FromObj(string path);
    };

    /*
     * A model in 3D / 2D space based on a mesh.
     * This can be rendered.
     */
    class Model
    {
        private:
            Mesh mesh;
            BaseMaterial material;
        public:
            Model(Mesh mesh, BaseMaterial material);
            ~Model();
    };

} // namespace GAIL
