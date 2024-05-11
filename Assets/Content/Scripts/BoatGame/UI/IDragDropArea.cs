using Content.Scripts.ItemsSystem;

namespace Content.Scripts.BoatGame.UI
{
    public interface IDragDropArea
    {
        void AddToInventory(ItemObject draggedItem);
    }
}