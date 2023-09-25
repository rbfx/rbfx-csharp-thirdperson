namespace RbfxTemplate.CharacterStates
{
    public class StartState : BaseState
    {
        public StartState(Character character) : base(character)
        {
        }

        public override void Update(ref Character.Inputs inputs)
        {
            if (Character.CharacterController.OnGround())
                Character.TransitionToState(CharacterState.OnGround, ref inputs);
            else
                Character.TransitionToState(CharacterState.Fall, ref inputs);
        }
    }
}