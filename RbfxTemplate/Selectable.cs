using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class Selectable : Component, IInteractable
    {
        private ComponentList _drawables;

        [SerializeField(Mode = AttributeMode.AmDefault, Name = "Title")]
        public string Title { get; set; }


        public Selectable(Context context) : base(context)
        {
        }

        protected override void OnNodeSet(Node previousNode, Node currentNode)
        {
            base.OnNodeSet(previousNode, currentNode);
            if (currentNode != null)
            {
                SubscribeToEvent("Selected", currentNode, Select);
                SubscribeToEvent("Unselected", currentNode, Unselect);
            }
            else
            {
                UnsubscribeFromAllEvents();
            }
        }

        private void Unselect(VariantMap obj)
        {
            var outline = Scene.GetComponent<OutlineGroup>();
            if (outline != null && _drawables != null)
                foreach (var component in _drawables)
                {
                    var drawable = component as Drawable;
                    outline.RemoveDrawable(drawable);
                }
        }

        private void Select(VariantMap obj)
        {
            var outline = Scene.GetComponent<OutlineGroup>();
            if (outline != null)
            {
                _drawables = Node.GetComponents<StaticModel>(true);
                foreach (var component in _drawables)
                {
                    var drawable = component as Drawable;
                    outline.AddDrawable(drawable);
                }
            }
        }
    }
}