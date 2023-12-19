#pragma once

#include "GAIL.hpp"

namespace GAIL
{
    class VertexAttribute;
    class BaseMaterial;

    // A point that exists of data (used for 3d objects).
    class Vertex
    {
        public:
            // The position of this vertex.
            Vector3 position;
            // Attributes for this vertex shown in shader.
            // Name and VertexAttribute.
            std::map<string, VertexAttribute> attributes;
            Vertex() {this->position={0,0,0};};
            Vertex(double x, double y, double z) {this->position={x,y,z};};
            Vertex(double x, double y) {this->position = {x,y,0};};
            ~Vertex();
    };

    

    // A face (triangle) of a mesh.
    class Face
    {
        public:
            Vertex *Indices[3];
            Face(Vertex *indices[3]);
            Face(Vertex p1, Vertex p2, Vertex p3);
            ~Face();
    };

    // A 3D / 2D shape data.
    class Mesh
    {
        public:
            std::vector<unsigned int> Faces;
            std::vector<Vertex> vertices;
            Mesh(std::vector<Face> faces);
            Mesh(std::vector<Vertex> vertices, std::vector<unsigned int> IndexFaces);
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
    
    /*
     * A model in 3D / 2D space based on a mesh.
     * This can be rendered.
     */
    class InstancedModel {
        public:
            Mesh* mesh;
            BaseMaterial material;
            InstancedModel(Mesh* mesh, BaseMaterial material);
            ~InstancedModel();
    };

} // namespace GAIL
