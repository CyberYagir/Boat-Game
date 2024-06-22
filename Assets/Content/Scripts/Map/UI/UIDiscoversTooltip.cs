using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Map.UI
{
    public class UIDiscoversTooltip : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Content")] private UIDiscoversItem item;
        [SerializeField, FoldoutGroup("Content")] private GameObject content;
        [SerializeField, FoldoutGroup("Content")] private RectTransform arrow;
        [SerializeField, FoldoutGroup("Content")] private TMP_Text paddleCounter;
        [SerializeField, FoldoutGroup("Content")] private Image paddleIcon;
        
        [SerializeField, FoldoutGroup("Tooltip")] private TooltipDataObject tooltipItemData;
        [SerializeField, FoldoutGroup("Tooltip")] private UITooltip toolTip;
        
        private ItemObject paddleItem;

        private CharacterService characterService;
        private bool isOpened = false;
        
        private MapUIService mapUIService;
        private GameDataObject gameData;
        private UIMessageBoxManager uiMessageBoxManager;

        private int paddlesCount;

        public void Init(SaveDataObject saveData,
            UIMessageBoxManager uiMessageBoxManager,
            MapUIService mapUIService,
            GameDataObject gameData,
            CharacterService characterService)
        {
            this.uiMessageBoxManager = uiMessageBoxManager;
            this.gameData = gameData;
            this.mapUIService = mapUIService;
            this.characterService = characterService;
            
            
            paddleItem = gameData.ConfigData.PaddleItem;
            paddleIcon.sprite = paddleItem.ItemIcon;
            
            
            toolTip.Init(tooltipItemData);
            int discoversCount = 0;
            item.gameObject.SetActive(true);
            for (int i = 0; i < saveData.Map.Islands.Count; i++)
            {
                if (!string.IsNullOrEmpty(saveData.Map.Islands[i].IslandName))
                {
                    Instantiate(item, item.transform.parent)
                        .Init(
                            IslandSeedData.Generate(saveData.Map.Islands[i].IslandPos),
                            saveData.Map.Islands[i],
                            uiMessageBoxManager,
                            this);
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
            OnCharactersChanged();
        }

        private void OnCharactersChanged()
        {
            foreach (var spawned in characterService.SpawnedCharacters)
            {
                spawned.Character.Equipment.OnEquipmentChange -= OnEquipmentChanged;
                spawned.Character.Equipment.OnEquipmentChange += OnEquipmentChanged;
            }
            OnEquipmentChanged();
        }

        private void OnEquipmentChanged()
        {
            CalculatePaddlesCount();
            UpdatePaddlesCounter();
            print("Update items: " + paddlesCount);
        }

        private int CalculatePaddlesCount()
        {
            paddlesCount = characterService.CalculateWeaponsCount(paddleItem);
            return paddlesCount;
        }

        private void UpdatePaddlesCounter() => paddleCounter.text = paddlesCount + "/" + gameData.ConfigData.PaddlesToTravelCount;

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

        public void GoToIsland(int islandIslandSeed)
        {
            if (CalculatePaddlesCount() >= gameData.ConfigData.PaddlesToTravelCount)
            {
                mapUIService.GoToIsland(islandIslandSeed);
                characterService.RemoveWeapon(paddleItem);
            }
            else
            {
                uiMessageBoxManager.ShowMessageBox("There aren't enough paddles to travel to the island!", null, "Ok", "_disabled");
            }
        }
    }
}
