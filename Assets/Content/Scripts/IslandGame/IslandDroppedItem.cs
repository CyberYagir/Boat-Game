using System;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame
{
    public class IslandDroppedItemData : MonoBehaviour, IIslandDroppedItemData
    {
        [SerializeField] private DroppedItemBase dropItem;
        [SerializeField] private bool saveItem = true;
        private SaveDataObject saveDataObject;

        [Inject]
        private void Construct(ScenesService scenesService)
        {
            if (scenesService.GetActiveScene() == ESceneName.DungeonGame)
            {
                enabled = false;
            }
        }

        public void AfterInject(SaveDataObject saveDataObject)
        {
            this.saveDataObject = saveDataObject;
            if (saveItem && enabled)
            {
                var island = saveDataObject.GetTargetIsland();
                island.AddDroppedItem(dropItem);
            }
        }


        public void DeleteItem()
        {
            if (enabled)
            {
                saveDataObject.GetTargetIsland().RemoveDroppedItem(dropItem);
            }
        }
    }
}
