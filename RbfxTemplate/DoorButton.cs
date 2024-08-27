using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class DoorButton : Selectable
    {
        private ResourceRef _openAnimationAttr = new ResourceRef(nameof(Animation));
        private ResourceRef _closeAnimationAttr = new ResourceRef(nameof(Animation));
        private Animation _openAnimation;
        private Animation _closeAnimation;

        public DoorButton(Context context) : base(context)
        {
        }

        [SerializeField(Mode = AttributeMode.AmDefault, Name = "Open Animation")]
        public ResourceRef OpenAnimationAttr
        {
            get => _openAnimationAttr;
            set
            {
                if (_openAnimationAttr != value)
                {
                    _openAnimationAttr = value;
                    _openAnimation = Context.ResourceCache.GetResource<Animation>(_openAnimationAttr.Name);
                }
            }
        }

        [SerializeField(Mode = AttributeMode.AmDefault, Name = "Close Animation")]
        public ResourceRef CloseAnimationAttr
        {
            get => _closeAnimationAttr;
            set
            {
                if (_closeAnimationAttr != value)
                {
                    _closeAnimationAttr = value;
                    _closeAnimation = Context.ResourceCache.GetResource<Animation>(_closeAnimationAttr.Name);
                }
            }
        }

        public Animation OpenAnimation
        {
            get => _openAnimation;
            set
            {
                _openAnimation = value;
                _openAnimationAttr.Name = _openAnimation?.Name ?? "";
            }
        }

        public Animation CloseAnimation
        {
            get => _closeAnimation;
            set
            {
                _closeAnimation = value;
                _closeAnimationAttr.Name = _closeAnimation?.Name ?? "";
            }
        }

        [SerializeField]
        public bool Open { get; set; }

        public override void Interact(Player player)
        {
            var controller = Node.Parent.GetComponent<AnimationController>(true);
            if (controller.NumAnimations > 0)
                return;

            var animationParameters = new AnimationParameters(Open ? CloseAnimation : OpenAnimation)
            {
                RemoveOnCompletion = true
            };

            if (animationParameters.Animation == null)
                return;

            Open = !Open;
            controller.PlayNewExclusive(animationParameters, 0.0f);
        }
    }
}