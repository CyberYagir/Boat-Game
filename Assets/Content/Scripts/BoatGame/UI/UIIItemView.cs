using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.ItemsSystem;
using Content.Scripts.Misc;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIIItemView : AnimatedWindow, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemDescription;
        [SerializeField] private Image spriteView;
        [SerializeField] private RawImage preview3D;
        [SerializeField] private ItemPreviewSpawner preview3DSpawner;
        [SerializeField] private ItemsStrengthCalculatorSO itemsStrengthCalculator;
        

        private ItemPreviewSpawner spawned;
        
        [Button]
        public void ShowItem(ItemObject itemObject)
        {
            if (spawned != null)
            {
                Destroy(spawned.gameObject);
            }
            itemName.text = itemObject.ItemName;
            if (itemObject.Prefab == null)
            {
                spriteView.enabled = true;
                preview3D.enabled = false;

                spriteView.sprite = itemObject.ItemIcon;
            }
            else
            {
                spriteView.enabled = false;
                preview3D.enabled = true;

                spawned = Instantiate(preview3DSpawner, new Vector3(0, 1000, 0), Quaternion.identity);
                
                spawned.Spawn(itemObject.Prefab, itemObject);
            }

            itemDescription.text = "No description provided";


            if (itemObject.ItemType == EItemType.Armor)
            {
                if (itemObject.Equipment == EEquipmentType.Armor || itemObject.Equipment == EEquipmentType.Helmet)
                {
                    itemDescription.text = "Def: " + (itemObject.ParametersData.Defence * 100f).ToString("F1") + "%\n";
                    itemDescription.text += "Pwr: " + itemsStrengthCalculator.GeItemStrength(itemObject).ToString("F1");
                }
                else
                {
                    itemDescription.text = "";
                    if (itemObject.ParametersData.Defence != 0)
                    {
                        itemDescription.text += "Def: " + (itemObject.ParametersData.Defence * 100f).ToString("F1") + "%\n";
                    }
                    
                    itemDescription.text += "Dmg: " + (itemObject.ParametersData.Damage).ToString("F1") + "\n";
                    itemDescription.text += "Pwr: " + itemsStrengthCalculator.GeItemStrength(itemObject).ToString("F1");
                }
            }
            else if (itemObject.ItemType == EItemType.Item)
            {
                itemDescription.text = "Used in crafts.";
            }

            ShowWindow();
        }


        public override void CloseWindow()
        {
            base.CloseWindow();

            Destroy(spawned.gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            spawned.SetActive(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            spawned.SetActive(false);
        }
    }
}
