using Urho3DNet;

namespace RbfxTemplate
{
    [DerivedFrom]
    public interface IInteractable
    {
        /// <summary>
        /// Text to display when hovering over object.
        /// </summary>
        string Tooltip { get; }

        /// <summary>
        /// Is interaction enabled.
        /// </summary>
        bool InteractionEnabled { get; }

        /// <summary>
        /// For how long the player should keep Use button pressed to complete interaction.
        /// </summary>
        float InteractionDuration { get; }

        /// <summary>
        /// Called by <see cref="Player"/> when cursor hovers over this object.
        /// </summary>
        /// <param name="player">Player component.</param>
        void OnHoverStart(Player player);

        /// <summary>
        /// Called by <see cref="Player"/> when cursor leaves this object.
        /// </summary>
        /// <param name="player">Player component.</param>
        void OnHoverEnd(Player player);

        /// <summary>
        /// Interact with player.
        /// </summary>
        /// <param name="player">Player component.</param>
        void Interact(Player player);
    }
}