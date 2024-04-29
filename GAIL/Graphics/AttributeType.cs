using Silk.NET.Vulkan;

namespace GAIL.Graphics
{
    /// <summary>
    /// The type of the attribute. <para/>
    /// 
    /// Long = 64-bit <br/>
    /// (Default) = 32-bit<br/>
    /// Short = 16-bit <br/>
    /// Byte = 8-bit <para/>
    /// U- = unsigned (no negatives) <br/>
    /// -Float = signed, floating-point number <br/>
    /// Int (default) = signed, integer <br/>
    /// -(2, 3, 4) = multiple components
    /// </summary>
    public enum AttributeType {
        ///
        Byte = Silk.NET.Vulkan.Format.R8Sint,
        ///
        UByte = Silk.NET.Vulkan.Format.R8Uint,
        ///
        Short = Silk.NET.Vulkan.Format.R16Sint,
        ///
        UShort = Silk.NET.Vulkan.Format.R16Uint,
        ///
        Int = Silk.NET.Vulkan.Format.R32Sint,
        ///
        UInt = Silk.NET.Vulkan.Format.R32Uint,
        ///
        Long = Silk.NET.Vulkan.Format.R64Sint,
        ///
        ULong = Silk.NET.Vulkan.Format.R64Uint,
        ///
        ShortFloat = Silk.NET.Vulkan.Format.R16Sfloat, 
        ///
        Float = Silk.NET.Vulkan.Format.R32Sfloat, 
        ///
        LongFloat = Silk.NET.Vulkan.Format.R64Sfloat, 

        ///
        Byte2 = Silk.NET.Vulkan.Format.R8G8Sint,
        ///
        UByte2 = Silk.NET.Vulkan.Format.R8G8Uint,
        ///
        Short2 = Silk.NET.Vulkan.Format.R16G16Sint,
        ///
        UShort2 = Silk.NET.Vulkan.Format.R16G16Uint,
        ///
        Int2 = Silk.NET.Vulkan.Format.R32G32Sint,
        ///
        UInt2 = Silk.NET.Vulkan.Format.R32G32Uint,
        ///
        Long2 = Silk.NET.Vulkan.Format.R64G64Sint,
        ///
        ULong2 = Silk.NET.Vulkan.Format.R64G64Uint,
        ///
        ShortFloat2 = Silk.NET.Vulkan.Format.R16G16Sfloat, 
        ///
        Float2 = Silk.NET.Vulkan.Format.R32G32Sfloat, 
        ///
        LongFloat2 = Silk.NET.Vulkan.Format.R64G64Sfloat, 

        ///
        Byte3 = Silk.NET.Vulkan.Format.R8G8B8Sint,
        ///
        UByte3 = Silk.NET.Vulkan.Format.R8G8B8Uint,
        ///
        Short3 = Silk.NET.Vulkan.Format.R16G16B16Sint,
        ///
        UShort3 = Silk.NET.Vulkan.Format.R16G16B16Uint,
        ///
        Int3 = Silk.NET.Vulkan.Format.R32G32B32Sint,
        ///
        UInt3 = Silk.NET.Vulkan.Format.R32G32B32Uint,
        ///
        Long3 = Silk.NET.Vulkan.Format.R64G64B64Sint,
        ///
        ULong3 = Silk.NET.Vulkan.Format.R64G64B64Uint,
        ///
        ShortFloat3 = Silk.NET.Vulkan.Format.R16G16B16Sfloat, 
        ///
        Float3 = Silk.NET.Vulkan.Format.R32G32B32Sfloat, 
        ///
        LongFloat3 = Silk.NET.Vulkan.Format.R64G64B64Sfloat, 

        ///
        Byte4 = Silk.NET.Vulkan.Format.R8G8B8A8Sint,
        ///
        UByte4 = Silk.NET.Vulkan.Format.R8G8B8A8Uint,
        ///
        Short4 = Silk.NET.Vulkan.Format.R16G16B16A16Sint,
        ///
        UShort4 = Silk.NET.Vulkan.Format.R16G16B16A16Uint,
        ///
        Int4 = Silk.NET.Vulkan.Format.R32G32B32A32Sint,
        ///
        UInt4 = Silk.NET.Vulkan.Format.R32G32B32A32Uint,
        ///
        ///
        Long4 = Silk.NET.Vulkan.Format.R64G64B64A64Sint,
        ///
        ULong4 = Silk.NET.Vulkan.Format.R64G64B64A64Uint,
        ///
        ShortFloat4 = Silk.NET.Vulkan.Format.R16G16B16A16Sfloat, 
        ///
        Float4 = Silk.NET.Vulkan.Format.R32G32B32A32Sfloat, 
        ///
        LongFloat4 = Silk.NET.Vulkan.Format.R64G64B64A64Sfloat, 
    }
}