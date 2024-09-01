using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class Selectable : Component, IInteractable
    {
        private ComponentList _drawables;

        /// <inheritdoc/>
        [SerializeField(Mode = AttributeMode.AmDefault, Name = "Tooltip")]

        public string Tooltip { get; set; }

        /// <inheritdoc/>
        public virtual bool InteractionEnabled { get; } = false;

        /// <inheritdoc/>
        public virtual float InteractionDuration { get; }

        public Selectable(Context context) : base(context)
        {
        }

        /// <inheritdoc/>
        public virtual void OnHoverStart(Player player)
        {
            var outline = Scene.GetComponent<OutlineGroup>();
            if (outline != null)
            {
                _drawables = Node.FindComponents<Drawable>(ComponentSearchFlag.SelfOrChildrenRecursive | ComponentSearchFlag.Derived);
                foreach (var component in _drawables)
                {
                    var drawable = component as Drawable;
                    outline.AddDrawable(drawable);
                }
            }
        }

        /// <inheritdoc/>
        public virtual void OnHoverEnd(Player player)
        {
            if (this.IsExpired)
                return;

            var outline = Scene.GetComponent<OutlineGroup>();
            if (outline != null && _drawables != null)
                foreach (var component in _drawables)
                {
                    var drawable = component as Drawable;
                    outline.RemoveDrawable(drawable);
                }
        }

        /// <inheritdoc/>
        public virtual void Interact(Player player)
        {
        }
    }
}