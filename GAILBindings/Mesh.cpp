#include <list>
#include <algorithm>

namespace ogl
{
    struct Vector3
    {
        double x;
        double y;
        double z;
        Vector3() {

        };
        Vector3(double X, double Y, double Z) {
            x=X;
            y=Y;
            z=Z;
        };
    };
    struct Vector2
    {
        double x;
        double y;
        Vector2() {

        };
        Vector2(double X, double Y) {
            x=X;
            y=Y;
        };
    };
    
    class Mesh
    {
        public:
        std::list<Face> Faces;
            Mesh(std::list<Face> f)
            {
                this->Faces = f;
            }
            ~Mesh()
            {
                Faces.clear();
            }
    };

    class Face
    {
        public:
            Face(BaseVertex vertices[]) {
                    std::copy(vertices, vertices+3, Vertices);
            }
            ~Face()
            {
                delete Vertices;
            }
            BaseVertex Vertices[3];
            
    };

    class BaseVertex
    {
        public:
            BaseVertex() {

            };
            BaseVertex(Vector3 pos, Vector3 normal = Vector3(0,0,0)) {
                Position = pos;
                Normal = normal;
            };
            ~BaseVertex() {
                delete &Position;
                delete &Normal;
            };
            Vector3 Position;
            Vector3 Normal;
    };
    class TextureVertex: public BaseVertex {
        public:
            TextureVertex(Vector3 pos, Vector3 normal = Vector3(0,0,0), Vector2 uv = Vector2(0,0)) {
                UV = uv;
            };
            ~TextureVertex() {
                delete &UV;
            };
            Vector2 UV;
    };
    
} // namespace ogl
