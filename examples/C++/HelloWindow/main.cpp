#include <GAIL.hpp>

int main() {
    GAIL::Window window("Hello Window!!", 800, 500);
    GAIL::Application app(GAIL::ApplicationType::RENDER, window);
}