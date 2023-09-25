namespace RbfxTemplate.CharacterStates
{
    /// <summary>
    ///     Base class for character state.
    /// </summary>
    public class BaseState
    {
        /// <summary>
        ///     Construct BaseState.
        /// </summary>
        /// <param name="character">Character component.</param>
        protected BaseState(Character character)
        {
            Character = character;
        }

        /// <summary>
        ///     Character component.
        /// </summary>
        public Character Character { get; }

        /// <summary>
        ///     Executes when the state becomes active.
        /// </summary>
        public virtual void Enter()
        {
        }

        /// <summary>
        ///     Executes when the state becomes inactive.
        /// </summary>
        public virtual void Exit()
        {
        }

        /// <summary>
        ///     Update state.
        /// </summary>
        public virtual void Update(ref Character.Inputs inputs)
        {
        }
    }
}