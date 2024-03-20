namespace GAIL.Audio
{
    /// <summary>
    /// An effect template for sounds.
    /// </summary>
    public interface ISoundEffect {
        /// <summary>
        /// Applies effects to the sound.
        /// </summary>
        /// <param name="baseSound">The original sound.</param>
        /// <returns>The effected sound.</returns>
        Sound Use(Sound baseSound);
    }
}