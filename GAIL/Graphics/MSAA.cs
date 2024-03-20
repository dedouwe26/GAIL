using Silk.NET.Vulkan;

namespace GAIL.Graphics
{
    /// <summary>
    /// All the levels of MSAA (anti-aliasing).
    /// </summary>
    public enum MSAA {
        /// <summary>
        /// No MSAA (off).
        /// </summary>
        MSAAx1 = SampleCountFlags.Count1Bit,
        MSAAx2 = SampleCountFlags.Count2Bit,
        MSAAx4 = SampleCountFlags.Count4Bit,
        MSAAx8 = SampleCountFlags.Count8Bit,
        MSAAx16 = SampleCountFlags.Count16Bit,
        MSAAx32 = SampleCountFlags.Count32Bit,
        MSAAx64 = SampleCountFlags.Count64Bit,
    }
}