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
        /// <summary>
        /// MSAA set to 2.
        /// </summary>
        MSAAx2 = SampleCountFlags.Count2Bit,
        /// <summary>
        /// MSAA set to 4.
        /// </summary>
        MSAAx4 = SampleCountFlags.Count4Bit,
        /// <summary>
        /// MSAA set to 8.
        /// </summary>
        MSAAx8 = SampleCountFlags.Count8Bit,
        /// <summary>
        /// MSAA set to 16.
        /// </summary>
        MSAAx16 = SampleCountFlags.Count16Bit,
        /// <summary>
        /// MSAA set to 32.
        /// </summary>
        MSAAx32 = SampleCountFlags.Count32Bit,
        /// <summary>
        /// MSAA set to 64.
        /// </summary>
        MSAAx64 = SampleCountFlags.Count64Bit,
    }
}