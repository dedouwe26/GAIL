#pragma once

namespace GAIL
{
    class Shader
    {
    public:
        Shader();
        ~Shader();
    };

    class Material
    {
    public:
        Material(Shader shader);
        ~Material();
    };
    
    
} // namespace GAIL
