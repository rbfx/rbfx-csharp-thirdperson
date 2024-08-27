using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class Vehicle : MoveAndOrbitComponent
    {
        private RaycastVehicle _raycastVehicle;

        public Vehicle(Context context) : base(context)
        {
            UpdateEventMask = UpdateEvent.UseUpdate;
        }

        public override void DelayedStart()
        {
            var node_ = Node;
            _raycastVehicle = node_.GetComponent<RaycastVehicle>();

            base.DelayedStart();
        }

        protected override void OnNodeSet(Node previousNode, Node currentNode)
        {
            if (currentNode != null)
                SubscribeToEvent("Use", currentNode, HandleUse);
            else
                UnsubscribeFromEvent("Use");
            //base.OnNodeSet(previousNode, currentNode);
        }

        private void HandleUse(VariantMap args)
        {
            var player = args["Player"].Ptr as Player;
            if (player != null)
            {
                player.GetIntoVehicle(this);
            }
        }
    }
}