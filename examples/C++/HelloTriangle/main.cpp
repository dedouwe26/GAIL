#include <GAIL.hpp>

int main () {
    // Initialize the application and window.
    GAIL::Application app{"Hello triangle"};

    GAIL::Vertex point1{};
    GAIL::Vertex point2{};
    GAIL::Vertex point3{};

    GAIL::Mesh triangleMesh(GAIL::Face(point1, point2, point3));

    GAIL::Model triangle(triangleMesh, GAIL::Basic2DMaterial{{0.5, 0.5, 0, 1}})
}