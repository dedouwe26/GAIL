#pragma once

#include "GAIL.hpp"
#include <vulkan/vulkan.hpp>

namespace GAIL
{
    // The type of the attribute. 
    //
    // Double = 64-bit (default: floating-point number), Short = 16-bit (default: int), Byte = 8-bit (default: int), 
    // U = unsigned (no negative numbers), 
    // Int = signed int, 
    // 2,3,4 = with 2,3,4 components.
    enum AttributeType {
        Float = VK_FORMAT_R32_SFLOAT, 
        Int = VK_FORMAT_R32_SINT,
        Double = VK_FORMAT_R64_SFLOAT, 
        DoubleInt = VK_FORMAT_R64_SINT,
        UInt = VK_FORMAT_R32_UINT,
        UDoubleInt = VK_FORMAT_R64_UINT,
        ShortFloat = VK_FORMAT_R16_SFLOAT, 
        Short = VK_FORMAT_R16_SINT,
        UShort = VK_FORMAT_R16_UINT,
        Byte = VK_FORMAT_R8_SINT,
        UByte = VK_FORMAT_R8_UINT,

        Float2 = VK_FORMAT_R32G32_SFLOAT, 
        Int2 = VK_FORMAT_R32G32_SINT, 
        Double2 = VK_FORMAT_R64G64_SFLOAT,
        DoubleInt2 = VK_FORMAT_R64G64_SINT, 
        UInt2 = VK_FORMAT_R32G32_UINT, 
        UDoubleInt2 = VK_FORMAT_R64G64_UINT,
        ShortFloat2 = VK_FORMAT_R16G16_SFLOAT, 
        Short2 = VK_FORMAT_R16G16_SINT, 
        UShort2 = VK_FORMAT_R16G16_UINT, 
        Byte2 = VK_FORMAT_R8G8_SINT, 
        UByte2 = VK_FORMAT_R8G8_UINT,

        Float3 = VK_FORMAT_R32G32B32_SFLOAT, 
        Int3 = VK_FORMAT_R32G32B32_SINT, 
        Double3 = VK_FORMAT_R64G64B64_SFLOAT, 
        DoubleInt3 = VK_FORMAT_R64G64B64_SINT, 
        UInt3 = VK_FORMAT_R32G32B32_UINT, 
        UDoubleInt3 = VK_FORMAT_R64G64B64_UINT, 
        ShortFloat3 = VK_FORMAT_R16G16B16_SFLOAT, 
        Short3 = VK_FORMAT_R16G16B16_SINT, 
        UShort3 = VK_FORMAT_R16G16B16_UINT, 
        Byte3 = VK_FORMAT_R8G8B8_SINT, 
        UByte3 = VK_FORMAT_R8G8B8_UINT, 

        Float4 = VK_FORMAT_R32G32B32A32_SFLOAT, 
        Int4 = VK_FORMAT_R32G32B32A32_SINT, 
        Double4 = VK_FORMAT_R64G64B64A64_SFLOAT, 
        DoubleInt4 = VK_FORMAT_R64G64B64A64_SINT, 
        UInt4 = VK_FORMAT_R32G32B32A32_UINT, 
        UDoubleInt4 = VK_FORMAT_R64G64B64A64_UINT, 
        ShortFloat4 = VK_FORMAT_R16G16B16A16_SFLOAT, 
        Short4 = VK_FORMAT_R16G16B16A16_SINT, 
        UShort4 = VK_FORMAT_R16G16B16A16_UINT, 
        Byte4 = VK_FORMAT_R8G8B8A8_SINT, 
        UByte4 = VK_FORMAT_R8G8B8A8_UINT, 
    };

    // A attribute (vertex buffer) in the shader, changes per vertex, from CPU to GPU (extend to use).
    // The location in the shader is based on the list order in a vertex.
    class VertexAttribute
    {
        public:
            // The type of this attribute.
            AttributeType type;
            VertexAttribute(AttributeType type);
            // Returns the data for the vertex input attributes.
            void* Use();
            ~VertexAttribute();
    };

    // A basic (per vertex) color attribute.
    class ColorAttribute : VertexAttribute
    {
        public:
            ColorAttribute(Color color);
            ~ColorAttribute();
    };
    
    // A basic (per vertex) normal attribute.
    class NormalAttribute : VertexAttribute
    {
        public:
            NormalAttribute(Vector3 color);
            ~NormalAttribute();
    };

    // A A basic (per vertex) Texture coordinates attribute.
    class UVAttribute : VertexAttribute
    {
        public:
            UVAttribute(Vector2 uv);
            ~UVAttribute();
    };

    // Contains code for the GPU on how everything is drawn (instanced).
    class Shader
    {
        public:
            Shader(string vert, string frag);
            ~Shader();
            // Sets a shader uniform.
            // Returns true if successful.
            bool SetUniform();
            // Sets all the attributes (per vertex) in the shader (vertex buffer).
            // Returns true if successful.
            bool SetAttributes(std::vector<VertexAttribute> attributes);
            
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
            void Use(std::map<string, VertexAttribute> attributes);
            // Returns the Shader.
            Shader& GetShader() {return *shader;};

            // Creates a Material from a MTL file.
            static BaseMaterial FromMtl(string path);
    };
    
    // Renders the object facing the 
    class BillboardMaterial : BaseMaterial
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
