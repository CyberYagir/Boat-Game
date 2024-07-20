using System.Collections.Generic;
using System.Resources;
using Content.Scripts.BoatGame.Services;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIPotionsList : MonoBehaviour
    {
        [SerializeField] private UIPotionItem item;
        [SerializeField] private Transform holder;

        private List<UIPotionItem> items = new List<UIPotionItem>();
        private ResourcesService resourcesService;

        public void Init(ResourcesService resourcesService)
        {
            this.resourcesService = resourcesService;
            resourcesService.OnChangeResources += ResourceManagerOnOnChangeResources;
            ResourceManagerOnOnChangeResources();
        }

        private void ResourceManagerOnOnChangeResources()
        {
            var potions = resourcesService.AllItemsList.FindAll(x => x.Item.Type == EResourceTypes.Potions);
            gameObject.SetActive(potions.Count != 0);

            if (potions.Count != 0)
            {
                for (int i = items.Count; i < potions.Count+1; i++)
                {
                    Instantiate(item, holder)
                        .With(x => items.Add(x));
                }

                for (int i = 0; i < items.Count; i++)
                {
                    items[i].gameObject.SetActive(i < potions.Count);

                    if (i < potions.Count)
                    {
                        items[i].SetItem(potions[i]);
                    }
                }
            }
        }
    }
}
