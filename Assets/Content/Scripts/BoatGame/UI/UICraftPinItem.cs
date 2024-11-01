using Content.Scripts.BoatGame.Services;
using Content.Scripts.ItemsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UICraftPinItem : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;
        private int maxCount;
        private IResourcesService resourcesService;
        private ItemObject item;

        public void Init(int maxCount, IResourcesService resourcesService, ItemObject item)
        {
            this.item = item;
            this.resourcesService = resourcesService;
            this.maxCount = maxCount;
            resourcesService.OnChangeResources += UpdateCounter;
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            text.text = resourcesService.GetItemsValue(item) + "/" + maxCount;
            icon.sprite = item.ItemIcon;
        }
    }
}
