using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Content.Scripts.Map.UI
{
    public class UIDiscoversTooltip : MonoBehaviour
    {
        [SerializeField] private UIDiscoversItem item;
        [SerializeField] private GameObject content;
        [SerializeField] private RectTransform arrow;
        [SerializeField] private TooltipDataObject tooltipItemData;
        [SerializeField] private UITooltip toolTip;
        [SerializeField] private TMP_Text paddleCounter;
        [SerializeField] private ItemObject paddleItem;

        private CharacterService characterService;
        private bool isOpened = false;
        
        int paddlesCount = 0;
        
        public void Init(SaveDataObject saveData)
        {
            characterService = CrossSceneContext.GetCharactersService();
            
            
            toolTip.Init(tooltipItemData);
            int discoversCount = 0;
            item.gameObject.SetActive(true);
            for (int i = 0; i < saveData.Map.Islands.Count; i++)
            {
                if (!string.IsNullOrEmpty(saveData.Map.Islands[i].IslandName))
                {
                    Instantiate(item, item.transform.parent)
                        .Init(IslandSeedData.Generate(saveData.Map.Islands[i].IslandPos), saveData.Map.Islands[i].IslandName);
                    discoversCount++;
                }
            }
            item.gameObject.SetActive(false);

            
            if (discoversCount == 0)
            {
                gameObject.SetActive(false);
                return;
            }
            
            
            Toggle();
            
            characterService.OnCharactersChange += OnCharactersChanged;
            OnEquipmentChanged();
        }

        private void OnCharactersChanged()
        {
            foreach (var spawned in characterService.SpawnedCharacters)
            {
                spawned.Character.Equipment.OnEquipmentChange -= OnEquipmentChanged;
                spawned.Character.Equipment.OnEquipmentChange += OnEquipmentChanged;
            }
        }

        private void OnEquipmentChanged()
        {
            paddlesCount = 0;
            foreach (var spawned in characterService.SpawnedCharacters)
            {
                if (spawned.AppearanceDataManager.WeaponItem != null)
                {
                    if (paddleItem == spawned.AppearanceDataManager.WeaponItem)
                    {
                        paddlesCount++;
                    }
                } 
            }

            UpdatePaddlesCounter();
        }

        private void UpdatePaddlesCounter()
        {
            paddleCounter.text = paddlesCount + "/1";
        }

        public void Toggle()
        {
            isOpened = !isOpened;

            if (isOpened)
            {
                arrow.DORotate(Vector3.forward * -90, 0.2f).SetLink(gameObject);
            }
            else
            {
                arrow.DORotate(Vector3.zero, 0.2f).SetLink(gameObject);
            }

            content.gameObject.SetActive(isOpened);
        }
        
    }
}
