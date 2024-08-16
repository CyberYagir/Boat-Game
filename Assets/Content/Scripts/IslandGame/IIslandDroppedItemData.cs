using Content.Scripts.Global;

namespace Content.Scripts.IslandGame
{
    public interface IIslandDroppedItemData
    {
        void AfterInject(SaveDataObject saveDataObject);
        void DeleteItem();
    }
}