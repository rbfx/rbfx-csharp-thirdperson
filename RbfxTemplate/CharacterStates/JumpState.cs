using Urho3DNet;

namespace RbfxTemplate.CharacterStates
{
    public class JumpState : Fall
    {
        public JumpState(Character character) : base(character)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Character.CharacterController.Jump(new Vector3(0, 7.5f, 0));
        }
    }
}