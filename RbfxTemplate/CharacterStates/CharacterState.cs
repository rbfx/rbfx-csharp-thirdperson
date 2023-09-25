namespace RbfxTemplate.CharacterStates
{
    /// <summary>
    ///     Character states.
    /// </summary>
    public enum CharacterState
    {
        /// <summary>
        ///     Start state. On first update in changes into actual state based on input data.
        /// </summary>
        Start,

        /// <summary>
        ///     Character is standing or running on ground.
        /// </summary>
        OnGround,

        /// <summary>
        ///     Character is jumping.
        /// </summary>
        Jump,

        /// <summary>
        ///     Character is in free fall.
        /// </summary>
        Fall,

        /// <summary>
        ///     Total number of valid states.
        ///     This should be the last state!
        /// </summary>
        NumStates
    }
}