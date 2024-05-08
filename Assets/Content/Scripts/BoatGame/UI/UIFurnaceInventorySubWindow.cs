using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIFurnaceInventorySubWindow : DragAreaWindow
    {
        [SerializeField] private UIInventoryItem item;
        
        private Dictionary<ItemObject, int> stackedItems = new Dictionary<ItemObject, int>(50);
        private List<UIInventoryItem> items = new List<UIInventoryItem>();
        private RaftBuildService raftBuildService;

        public void Init(RaftBuildService raftBuildService)
        {
            this.raftBuildService = raftBuildService;
            Redraw();
        }
        

        protected override void AddItemToStack()
        {
            if (stackedItems[DragManager.DraggedItem] > DragManager.ItemsInStack)
            {
                DragManager.SetStack(DragManager.ItemsInStack + 1);
                UpdateStackText();
            }
        }

        private void UpdateStackText()
        {
            var spawnedItem = items.Find(x => x.Item == DragManager.DraggedItem);
            spawnedItem.With(x => x.Init(DragManager.DraggedItem, this))
                .With(x => x.SetValue(stackedItems[DragManager.DraggedItem] - DragManager.ItemsInStack));
        }


        public override void OnDragStarted()
        {
            UpdateStackText();
        }

        public override void OnDragDropped()
        {
            Redraw();
        }

        public void Redraw()
        {
            stackedItems.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                items[i].DisableItem();
                Destroy(items[i].gameObject);
            }

            items.Clear();
            item.gameObject.SetActive(true);
            foreach (var raftStorage in raftBuildService.Storages)
            {
                var other = raftStorage.GetItem(EResourceTypes.Eat);
                var build = raftStorage.GetItem(EResourceTypes.Build);

                foreach (var item in other)
                {
                    CalculateItemToStack(item);
                }
                
                foreach (var item in build)
                {
                    CalculateItemToStack(item);
                }
            }

            foreach (var stacked in stackedItems)
            {
                Instantiate(item, item.transform.parent)
                    .With(x => x.Init(stacked.Key, this))
                    .With(x => x.SetValue(stacked.Value))
                    .With(x => items.Add(x));
            }
            
            item.gameObject.SetActive(false);
        }

        private void CalculateItemToStack(RaftStorage.StorageItem item)
        {
            if (item.Item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanFuel) || item.Item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanSmelt))
            {
                if (!stackedItems.ContainsKey(item.Item))
                {

                    stackedItems.Add(item.Item, 0);
                }

                stackedItems[item.Item] += item.Count;
            }
        }
    }
}
