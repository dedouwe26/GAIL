#pragma once

#include <string>
#include "GAIL.hpp"

using string = std::string;

namespace GAIL
{

    // A attribute (vertex buffer) in the shader, changes per vertex, from CPU to GPU.
    struct VertexAttribute
    {
        // The data of a attribute
        char data[];
        // All the types of a attribute;
        struct AttributeType {Double, Float, Vector2, Vector3, Vector4, Vector2Int, Vector4UInt};
        // The type of this attribute.
        AttributeType type;
        // The location of the attribute (vertex buffer) in the shader.
        int location;
    };

    // Contains code for the GPU on how everything is drawn (instanced).
    class Shader
    {
        public:
            Shader(string vert, string frag);
            ~Shader();
            // Sets a shader uniform.
            void SetUniform();
    };

    // Contains the shader and shader data (instanced).
    class BaseMaterial
    {
        private:
            Shader *shader;
        public:
            BaseMaterial(Shader shader);
            ~BaseMaterial();
            // Sets all the uniforms (used before rendering).
            void Use(std::list<VertexAttribute> attributes);
            // Returns the Shader.
            Shader& GetShader() {return *shader;};

            // Creates a Material from a MTL file.
            static BaseMaterial FromMtl(string path);
    };
    
    // Renders the object facing the 
    class BillboardMaterial
    {
        public:
            BillboardMaterial();
            ~BillboardMaterial();
    };

    // A material for color and view matrix only (default).
    class BasicMaterial : BaseMaterial
    {
        public:
            Texture texture;
            Color color;
            Matrix viewMatrix;
            BasicMaterial(Color color, Texture texture, Matrix viewMatrix);
            ~BasicMaterial();
    };

    // A material that is rendered flat on the screen (e.g. UI).
    class Basic2DMaterial
    {
        public:
            Basic2DMaterial();
            ~Basic2DMaterial();
    };
    
} // namespace GAIL
