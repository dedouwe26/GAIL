#include <list>
#include <algorithm>
#include "Mesh.hpp"

using namespace GAILBindings;

Vector3::Vector3() {

};
Vector3::Vector3(double X, double Y, double Z) {
    x=X;
    y=Y;
    z=Z;
};
Vector2::Vector2() {

};
Vector2::Vector2(double X, double Y) {
    x=X;
    y=Y;
};
Mesh::Mesh(std::list<Face> f)
{
    this->Faces = f;
}
Mesh::~Mesh()
{
    Faces.clear();
}
Face::Face(BaseVertex vertices[]) {
        std::copy(vertices, vertices+3, Vertices);
}
Face::~Face()
{
    delete Vertices;
}
BaseVertex::BaseVertex() {

};
BaseVertex::BaseVertex(Vector3 pos, Vector3 normal = Vector3(0,0,0)) {
    Position = pos;
    Normal = normal;
};
BaseVertex::~BaseVertex() {
    delete &Position;
    delete &Normal;
};
TextureVertex::TextureVertex(Vector3 pos, Vector3 normal = Vector3(0,0,0), Vector2 uv = Vector2(0,0)) {
    UV = uv;
};
TextureVertex::~TextureVertex() {
    delete &UV;
};

