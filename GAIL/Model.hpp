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
            Vertex();
            Vertex(double x, double y, double z);
            Vertex(double x, double y);
            ~Vertex();

            bool operator==(const Vertex& other);
    };

    

    // A face (triangle) of a mesh.
    class Face
    {
        public:
            Vertex *p1;
            Vertex *p2;
            Vertex *p3;
            Face(Vertex *indices[3]);
            Face(Vertex p1, Vertex p2, Vertex p3);
            Face(Vertex *p1, Vertex *p2, Vertex *p3);
            ~Face();
    };

    // A 3D / 2D shape data.
    class Mesh
    {
        public:
            // (Face0-p1, Face0-p2, Face0-p3, Face1-p1, etc)
            std::vector<unsigned> indexFaces;
            std::vector<Vertex> vertices;
            // Checks if vertices points to the same address.
            Mesh(std::vector<Face> faces);
            Mesh(std::vector<Vertex> vertices, std::vector<unsigned> indexFaces);
            ~Mesh();
            // Returns a mesh constructed from a Wavefront obj file
            // The attributes for each vertex that can be added are:
            // "normal": NormalAttribute
            // "uv": UVAttribute
            
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
