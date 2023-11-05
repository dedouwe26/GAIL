#ifndef MESH_HEADER
#define MESH_HEADER

#include <list>

namespace GAILBindings
{
    struct Vector3
    {
        double x;
        double y;
        double z;
        Vector3();
        Vector3(double X, double Y, double Z);
    };
    struct Vector2
    {
        double x;
        double y;
        Vector2();
        Vector2(double X, double Y);
    };
    class Mesh
    {
        public:
            std::list<Face> Faces;
            Mesh(std::list<Face> f);
            ~Mesh();
    };

    class Face
    {
        public:
            Face(BaseVertex vertices[]);
            ~Face();
            BaseVertex Vertices[3];
            
    };

    class BaseVertex
    {
        public:
            BaseVertex();
            BaseVertex(Vector3 pos, Vector3 normal = Vector3(0,0,0));
            ~BaseVertex();
            Vector3 Position;
            Vector3 Normal;
    };
    class TextureVertex: public BaseVertex {
        public:
            TextureVertex(Vector3 pos, Vector3 normal = Vector3(0,0,0), Vector2 uv = Vector2(0,0));
            ~TextureVertex();
            Vector2 UV;
    };
    
} // namespace GAILBindings


#endif