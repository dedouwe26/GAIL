#include "GAIL.hpp"

namespace GAIL
{
    VertexAttribute::VertexAttribute(AttributeType type) : type{type} {};
    VertexAttribute::~VertexAttribute() {delete &type;};
    ColorAttribute::ColorAttribute(Color color) : VertexAttribute(AttributeType::Float4), color{color} {};
    ColorAttribute::~ColorAttribute() {delete &color;};
    NormalAttribute::NormalAttribute(Vector3 normal) : VertexAttribute(AttributeType::Float3), normal{normal} {};
    NormalAttribute::~NormalAttribute() {delete &normal;};
    UVAttribute::UVAttribute(Vector2 uv) : VertexAttribute(AttributeType::Float2), uv{uv} {};
    UVAttribute::~UVAttribute() {delete &uv;};

    Shader::Shader(string vert, string frag, string geom = "") {
        // TODO: Load shaders
    };
    Shader::Shader(string mesh, string frag) {
        // TODO: Load mesh shader
    };
    Shader::~Shader() {
        this->attributeDescriptions.clear();
        // TODO: cleanup other resources
    };
    void Shader::SetUniforms() {
        // TODO: Set uniforms
    }
    void Shader::SetAttributeLayout(std::map<string, AttributeType> attributeLayout) {
        // TODO: Set uniforms
    }

    BaseMaterial::BaseMaterial(Shader shader) {
        this->shader = &shader;
    };
    BaseMaterial::~BaseMaterial() {
        delete this->shader;
    }
    void BaseMaterial::Use() {
        // This MUST be overridden to add funcionality.
    };
    Shader* BaseMaterial::GetShader() {return shader;};

    BasicMaterial::BasicMaterial(Color color, Texture texture, Matrix modelMatrix, Matrix viewMatrix, Matrix projectionMatrix) : BaseMaterial(Shader("", "", "" /* TODO: make shaders*/)) {
        this->color = color;
        this->texture = texture;
        this->mvp = projectionMatrix * viewMatrix * modelMatrix;
    }
    BasicMaterial::BasicMaterial(Color color, Texture texture, Transform transform, Matrix viewMatrix, Matrix projectionMatrix) : BasicMaterial{color, texture, transform.ToModelMatrix(), viewMatrix, projectionMatrix} {};
} // namespace GAIL
