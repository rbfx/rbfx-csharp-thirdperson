using System.ComponentModel;
using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class GameRmlUIComponent : RmlUIComponent
    {
        private RmlUIStateBase _state;

        public GameRmlUIComponent(Context context) : base(context)
        {
        }

        public RmlUIStateBase State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    if (_state != null)
                        _state.PropertyChanged -= OnPropertyChanged;

                    _state = value;

                    if (_state != null)
                        _state.PropertyChanged += OnPropertyChanged;
                }
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateProperty(e.PropertyName);
        }

        public void UpdateProperties()
        {
            DirtyAllVariables();
        }

        public void UpdateProperty(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
                DirtyVariable(propertyName);
        }

        protected override void OnDataModelInitialized()
        {
            State?.OnDataModelInitialized(this);

            base.OnDataModelInitialized();
        }
    }
}