#include <vector>

#include <GAIL.hpp>

// Define points of the triangle.
GAIL::Vertex point1{.5, 0};
GAIL::Vertex point2{0, .5};
GAIL::Vertex point3{-.5, 0};

// Create a triangle mesh
GAIL::Mesh triangleMesh(std::vector<GAIL::Face>{GAIL::Face(point1, point2, point3)});
// Create a model with a basic shader.
GAIL::Model triangle(triangleMesh, GAIL::Basic2DMaterial{{0.5, 0.5, 0, 1}});


int main () {
    // Initialize the application and window.
    GAIL::Application app{"Hello triangle"};

    
    // Sets an update function.
    app.SetOnUpdate([](GAIL::Application app, double _){
        // Renders the triangle this frame.
        app.GetGraphicsManager().Render2D(std::vector<GAIL::Model>{triangle});
    });
}