#pragma once

#include <list>
#include <Shader.hpp>

namespace GAIL
{
    // A 4x4 matrix.
    struct Matrix {
        float m0, m4, m8, m12;
        float m1, m5, m9, m13;
        float m2, m6, m10, m14;
        float m3, m7, m11, m15;
    };
    
    // A structure with 2 components (used for 2D space).
    struct Vector2
    {
        double x;
        double y;
    };

    // A structure with 3 components (used for 3D space).
    struct Vector3
    {
        double x;
        double y;
        double z;
    };
    
    // A structure with 3 components (used for RGBA).
    struct Vector4
    {
        double x;
        double y;
        double z;
        double w;
    };
    
    // A 3D Rotation quaternion
    struct Quaternion 
    {
        double x=0;
        double y=0;
        double z=0;
        double w=1;
    };

    // Contains rotation, scale and translation for 3D / 2D objects.
    struct Transform
    {
        Vector3 translation;
        Quaternion rotation;
        Vector3 scale;   
    };

    // A point that exists of data (used for 3d objects).
    class Vertex
    {
    public:
        Vector3 position;
        Vertex() : position{Vector3{0,0,0}} {};
        Vertex(double x, double y, double z) : position{Vector3{x,y,z}} {};
        Vertex(double x, double y) : position{Vector3{x,y,0}} {};
        ~Vertex();
    };

    // A face (triangle) of a mesh.
    class Face
    {
    public:
        Face(Vertex vertices[3]);
        ~Face();
    };

    // A 3D / 2D shape data.
    class Mesh
    {
    public:
        Mesh(std::list<Face> faces);
        ~Mesh();
    };

    // A mesh with data how it's rendered.
    class Model
    {
    public:
        Model(Mesh mesh, Shader shader, Material material);
        ~Model();
    };

    /*
     * A instanced object in 3D / 2D space based on model.
     * This can be rendered.
    */
    class Object
    {
    public:
        Object(Model model, Transform transform);
        ~Object();
    };

} // namespace GAIL
