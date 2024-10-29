using Content.Scripts.BoatGame.Services;
using Content.Scripts.ItemsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageSlaveItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text charName, charDescription, charCost;
        [SerializeField] private RawImage rawImage;
        [SerializeField] private UICustomButton customButton;
        [SerializeField] private GameObject costDisplay;
        [SerializeField] private GameObject darker;
        
        
        private DisplayCharacter displayCharacter;
        private IResourcesService resourcesService;
        private ItemObject moneyItem;
        private UIVillageSlavesSubWindow window;
        private SlaveCreatedCharacterInfo info;


        public void Init(DisplayCharacter displayCharacter, SlaveCreatedCharacterInfo info, IResourcesService resourcesService, ItemObject moneyItem, UIVillageSlavesSubWindow window)
        {
            this.info = info;
            this.window = window;
            this.moneyItem = moneyItem;
            this.resourcesService = resourcesService;
            this.displayCharacter = displayCharacter;
            rawImage.texture = displayCharacter.Display.RenderTexture;

            charName.text = info.Character.Name;
            charDescription.text = displayCharacter.Description;
            charCost.text = info.Cost.ToString();

            resourcesService.OnChangeResources += ResourcesServiceOnOnChangeResources;
        }

        private void ResourcesServiceOnOnChangeResources()
        {
            customButton.SetInteractable(resourcesService.IsHaveItem(new RaftStorage.StorageItem(moneyItem, info.Cost)));
        }

        public void UpdateItem(bool isHave)
        {            
            ResourcesServiceOnOnChangeResources();

            customButton.gameObject.SetActive(!isHave);
            costDisplay.SetActive(!isHave);
            
            darker.gameObject.SetActive(isHave);
            if (isHave)
            {
                displayCharacter.Display.ActiveSold();
            }
            
        }

        public void Buy() => window.BuySlave(info);

        public void Dispose()
        {
            resourcesService.OnChangeResources -= ResourcesServiceOnOnChangeResources;
        }
    }
}
