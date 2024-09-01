using System.Collections.Generic;
using System.Linq;
using RbfxTemplate.Inventory;
using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory]
    public partial class InventoryState : RmlUIStateBase
    {
        protected readonly SharedPtr<Scene> _scene;
        private readonly Viewport _viewport;

        private List<KeyValuePair<string, InventorySlot>> _playerInventory = new List<KeyValuePair<string, InventorySlot>>();
        private VariantList _inventory = new VariantList();
        private readonly PrefabReference _prefabReference;

        public InventoryState(UrhoPluginApplication app) : base(app, "UI/Inventory.rml")
        {
            _scene = Context.CreateObject<Scene>();
            _scene.Ptr.LoadXML("Scenes/Inventory.scene");
            _scene.Ptr.IsUpdateEnabled = false;

            var cameraNode = _scene.Ptr.FindComponent<Camera>(ComponentSearchFlag.Default)?.Node;

            _prefabReference = _scene.Ptr.FindComponent<PrefabReference>(ComponentSearchFlag.Default);

            _viewport = Context.CreateObject<Viewport>();
            _viewport.Camera = cameraNode?.GetComponent<Camera>();
            _viewport.Scene = _scene;
            SetViewport(0, _viewport);

        }

        public override void OnDataModelInitialized(GameRmlUIComponent component)
        {
            component.BindDataModelVariantVector("inventory_items", _inventory);
        }

        public override void Activate(StringVariantMap bundle)
        {
            Application.Settings.Apply(_scene.Ptr.GetComponent<RenderPipeline>());

            var player = Application.Game?.Player;
            _playerInventory.Clear();
            _inventory.Clear();
            if (player != null)
            {
                _playerInventory.AddRange(player.Inventory.OrderBy(_ => _.Value.GetName()));
                foreach (var inventorySlot in _playerInventory)
                {
                    _inventory.Add(new VariantMap { { "key", inventorySlot.Key }, { "name", inventorySlot.Value.GetName()},{ "count", inventorySlot.Value.Count } });
                }

                if (_inventory.Count > 0)
                {
                    Preview(0);
                }
            }

            base.Activate(bundle);
        }

        private void Preview(int index)
        {
            if (_prefabReference == null)
                return;

            _prefabReference.SetPrefab(null);

            if (index < 0 || index >= _inventory.Count)
                return;

            var slot = _playerInventory[index].Value;

            var prefab = slot?.ItemDefinition?.Value?.Prefab;
            if (prefab != null)
            {
                _prefabReference.SetPrefab(Context.ResourceCache.GetResource<PrefabResource>(prefab.Name));
            }
        }

        public override void TransitionComplete()
        {
            SubscribeToEvent(E.KeyUp, HandleKeyUp);
        }

        public override void TransitionStarted()
        {
            UnsubscribeFromEvent(E.KeyUp);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        private void HandleKeyUp(VariantMap args)
        {
            var key = (Key)args[E.KeyUp.Key].Int;
            switch (key)
            {
                case Key.KeyEscape:
                case Key.KeyBackspace:
                    Application.HandleBackKey();
                    return;
            }
        }
    }
}