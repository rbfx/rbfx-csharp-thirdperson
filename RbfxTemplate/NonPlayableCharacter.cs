using System;
using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    [Preserve(AllMembers = true)]
    public class NonPlayableCharacter : LogicComponent
    {
        private static readonly Random _rnd = new Random();
        private Vector3 _targret;
        private float _distance;
        private float _restTime;
        private MoveAndOrbitComponent _moveAndOrbitComponent;

        public NonPlayableCharacter(Context context) : base(context)
        {
            UpdateEventMask = UpdateEvent.UsePostupdate;
        }

        public override void DelayedStart()
        {
            base.DelayedStart();
            _moveAndOrbitComponent = Node.GetDerivedComponent<MoveAndOrbitComponent>();
        }

        public override void PostUpdate(float timeStep)
        {
            base.Update(timeStep);
            var currentPosition = Node.WorldPosition;
            if (_restTime > 0)
            {
                _restTime -= timeStep;
                if (_restTime > 0.0f) return;

                var dir = new Vector3(_rnd.Next(3) - 1, 0, _rnd.Next(3) - 1) * 5.0f;
                if (dir.ApproximatelyEquivalent(Vector3.Zero))
                {
                    _restTime += 1.0f;
                    return;
                }

                _targret = currentPosition + dir;
                dir = dir.Normalized;
                if (_rnd.Next(2) == 0)
                    dir *= 0.5f;
                _distance = float.MaxValue;

                _moveAndOrbitComponent.SetVelocity(dir);
            }

            var diff = _targret - currentPosition;
            diff.Y = 0;
            var distance = diff.Length;
            if (distance >= _distance)
            {
                _restTime = 2.0f;
                _moveAndOrbitComponent.SetVelocity(Vector3.Zero);
                return;
            }

            _distance = distance;
        }
    }
}