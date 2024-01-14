#include "GAIL.hpp"
#include "Shader.hpp"

namespace GAIL
{
    bool VertexAttribute::operator==(const VertexAttribute &b) const {
        return this->type==b.type&&typeid(*this)==typeid(b);
    }
    VertexAttribute::VertexAttribute() : VertexAttribute(AttributeType::Float3) {};
    VertexAttribute::VertexAttribute(AttributeType type) : type{type} {};
    VertexAttribute::~VertexAttribute() {delete &type;};
    ColorAttribute::ColorAttribute(Color color) : VertexAttribute(AttributeType::Float4), color{color} {};
    ColorAttribute::~ColorAttribute() {delete &color;};
    NormalAttribute::NormalAttribute(Vector3 normal) : VertexAttribute(AttributeType::Float3), normal{normal} {};
    NormalAttribute::~NormalAttribute() {delete &normal;};
    UVAttribute::UVAttribute(Vector2 uv) : VertexAttribute(AttributeType::Float2), uv{uv} {};
    UVAttribute::~UVAttribute() {delete &uv;};

    Shader::Shader(string vert, string frag, string geom) {
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
    Basic2DMaterial::Basic2DMaterial(Color color, Texture texture) : BaseMaterial(Shader("", "", "" /* TODO: make shaders*/)) {
        this->color = color;
        this->texture = texture;
    };
    Basic2DMaterial::~Basic2DMaterial() {
        delete &this->texture;
        delete &this->color;
    };
    Text2DMaterial::Text2DMaterial(string text, std::map<char, Texture> font) : BaseMaterial(Shader("", "", "" /* TODO: make shaders*/)) {
        this->text = text;
        this->font = font;
    };
    Text2DMaterial::~Text2DMaterial() {
        this->text.clear();
        delete &this->text;
        this->font.clear();
        delete &this->font;
    };
} // namespace GAIL
