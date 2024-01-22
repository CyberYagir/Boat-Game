using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIInventorySubWindow : MonoBehaviour
    {
        [SerializeField] private UIInventoryItem item;
        private List<UIInventoryItem> items = new List<UIInventoryItem>();
        private RaftBuildService raftBuildService;

        public void Init(GameDataObject gameData, SelectionService selectionService, RaftBuildService raftBuildService)
        {
            this.raftBuildService = raftBuildService;
            Redraw();
        }

        public void Redraw()
        {
            for (int i = 0; i < items.Count; i++)
            {
                Destroy(items[i].gameObject);
            }

            items.Clear();
            item.gameObject.SetActive(true);
            foreach (var raftStorage in raftBuildService.Storages)
            {
                var other = raftStorage.GetStorage(EResourceTypes.Other, true);

                if (other != null)
                {
                    foreach (var otherItemObject in other.ItemObjects)
                    {
                        for (int i = 0; i < otherItemObject.Count; i++)
                        {
                            Instantiate(item, item.transform.parent)
                                .With(x => x.Init(otherItemObject.Item))
                                .With(x => items.Add(x));
                        }
                    }
                }
            }
            item.gameObject.SetActive(false);
        }
    }
}
