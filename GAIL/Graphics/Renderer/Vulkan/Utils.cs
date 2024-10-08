using GAIL.Core;
using OxDED.Terminal.Logging;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

/// <summary>
/// A delegate that is wrapping an Vulkan array getter.
/// </summary>
/// <typeparam name="T">The type of the array values.</typeparam>
/// <param name="pointer">The pointer to the array.</param>
/// <param name="count">The count from the Vulkan method.</param>
/// <returns>The result from the Vulkan method.</returns>
public delegate Result VulkanArrayGetter<T>(Pointer<T> pointer, ref uint count) where T : unmanaged;
/// <summary>
/// A delegate that is wrapping an Vulkan array getter.
/// </summary>
/// <typeparam name="T">The type of the array values.</typeparam>
/// <param name="pointer">The pointer to the array.</param>
/// <param name="count">The count from the Vulkan method.</param>
/// <returns>The result from the Vulkan method.</returns>
public delegate Result VulkanArrayGetterPtr<T>(Pointer<T> pointer, Pointer<uint> count) where T : unmanaged;

/// <summary>
/// Contains utilities for working with Vulkan.
/// </summary>
public static class Utils {
    /// <summary>
    /// Will check if the result is okay. Otherwise it will error
    /// </summary>
    /// <param name="result">The result of the vulkan method.</param>
    /// <param name="logger">The logger of the graphics part.</param>
    /// <param name="msg">The message to log.</param>
    /// <param name="fatal">If it is fatal (if this is not recoverable).</param>
    /// <exception cref="APIBackendException"></exception>
    /// <returns>If it was successful.</returns>
    public static bool Check(Result result, Logger logger, string msg, bool fatal = false) {
        if (result!=Result.Success) {
            msg+=$" ({result})";
            if (fatal) {
                logger.LogFatal("Vulkan: "+msg);
                throw new APIBackendException("Vulkan", msg);
            }
            logger.LogError("Vulkan: "+msg);
            return false;
        }
        return true;
    }
    /// <summary>
    /// Gets an array from a Vulkan array getter method.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    /// <param name="arrayGetter">The wrapper around the Vulkan array getter.</param>
    /// <param name="array">The array that is created.</param>
    /// <param name="logger">The logger if an error has been encountered.</param>
    /// <param name="arrayName">The array name if an error has been encountered (example: SwapchainImages).</param>
    /// <param name="fatal">If the program can recover after an error occured.</param>
    /// <returns>True if it was successful.</returns>
    public static bool GetArray<T>(VulkanArrayGetter<T> arrayGetter, out T[] array, Logger logger, string arrayName, bool fatal = false) where T : unmanaged {
        uint count = 0;
        if (!Check(
            arrayGetter(Pointer<T>.FromNull(), ref count),
            logger,
            $"Failed to get array '{arrayName}' of type {typeof(T).Name}.",
            fatal
        )) {
            array = [];
            return false;
        }
        return GetArray(count, arrayGetter, out array, logger, arrayName, fatal);
    }
    /// <summary>
    /// Gets an array from a Vulkan array getter method.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    /// <param name="arrayGetter">The wrapper around the Vulkan array getter.</param>
    /// <param name="array">The array that is created.</param>
    /// <param name="logger">The logger if an error has been encountered.</param>
    /// <param name="arrayName">The array name if an error has been encountered (example: SwapchainImages).</param>
    /// <param name="fatal">If the program can recover after an error occured.</param>
    /// <returns>True if it was successful.</returns>
    public static bool GetArray<T>(VulkanArrayGetterPtr<T> arrayGetter, out T[] array, Logger logger, string arrayName, bool fatal = false) where T : unmanaged {        
        uint count = 0;
        if (!Check(
            arrayGetter(Pointer<T>.FromNull(), Pointer<uint>.From(ref count)),
            logger,
            $"Failed to get array '{arrayName}' of type {typeof(T).Name}.",
            fatal
        )) {
            array = [];
            return false;
        }

        return GetArray(count, (Pointer<T> pointer, ref uint c) => {
            return arrayGetter(pointer, Pointer<uint>.From(ref c));
        }, out array, logger, arrayName, fatal);
    }
    /// <summary>
    /// Gets an array from a Vulkan array getter method using a length.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    /// <param name="count">The length of the array.</param>
    /// <param name="arrayGetter">The wrapper around the Vulkan array getter.</param>
    /// <param name="array">The array that is created.</param>
    /// <param name="logger">The logger if an error has been encountered.</param>
    /// <param name="arrayName">The array name if an error has been encountered (example: SwapchainImages).</param>
    /// <param name="fatal">If the program can recover after an error occured.</param>
    /// <returns>True if it was successful.</returns>
    public static bool GetArray<T>(uint count, VulkanArrayGetter<T> arrayGetter, out T[] array, Logger logger, string arrayName, bool fatal = false) where T : unmanaged {
        array = new T[count];
        if (count == 0) {
            return true;
        }
        if (!Check(
            arrayGetter(Pointer<T>.FromArray(ref array), ref count),
            logger, $"Failed to get array '{arrayName}' of type {typeof(T).Name}.",
            fatal
        )) {
            array = [];
            return false;
        }
        return true;
    }
}