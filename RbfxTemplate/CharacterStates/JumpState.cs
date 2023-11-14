using Urho3DNet;

namespace RbfxTemplate.CharacterStates
{
    public class JumpState : Fall
    {
        public JumpState(Character character) : base(character)
        {
        }

        public override void Enter(object argument)
        {
            base.Enter(argument);
            Character.CharacterController.Jump(new Vector3(0, 7.5f, 0));
        }
    }
}