using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Content.Scripts.BoatGame.Scriptable;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIPotionsList : MonoBehaviour
    {
        [SerializeField] private UIPotionItem item;
        [SerializeField] private Transform holder;
        [SerializeField] private Image draggedItem;
        [SerializeField] private bool drawEat;

        private List<UIPotionItem> items = new List<UIPotionItem>();
        private IResourcesService resourcesService;
        private ISelectionService selectionService;
        private ICharacterService characterService;
        private UICharactersList charactersList;
        private ScenesService scenesService;

        public void Init(IResourcesService resourcesService, ISelectionService selectionService, ICharacterService characterService, UICharactersList charactersList, ScenesService scenesService)
        {
            this.scenesService = scenesService;
            this.charactersList = charactersList;
            this.characterService = characterService;
            this.selectionService = selectionService;
            this.resourcesService = resourcesService;
            resourcesService.OnChangeResources += ResourceManagerOnOnChangeResources;
            ResourceManagerOnOnChangeResources();
            draggedItem.gameObject.SetActive(false);


            scenesService.OnChangeActiveScene += OnChangeScene;
        }

        private void OnChangeScene(ESceneName scene)
        {
            ResourceManagerOnOnChangeResources();
        }

        private void OnDestroy()
        {
            scenesService.OnChangeActiveScene -= OnChangeScene;
        }

        // private void ScenesServiceOnOnLoadOtherScene(ESceneName obj)
        // {
        //     RemoveEvents();
        // }
        //
        // private void ScenesServiceOnOnUnLoadOtherScene(ESceneName obj)
        // {
        //     if (obj == ESceneName.Map)
        //     {
        //         RemoveEvents();
        //     }
        // }

        // private void RemoveEvents()
        // {
        //     scenesService.OnChangeActiveScene -= OnChangeScene;
        //     scenesService.OnLoadOtherScene -= ScenesServiceOnOnUnLoadOtherScene;
        //     scenesService.OnUnLoadOtherScene -= ScenesServiceOnOnUnLoadOtherScene;
        // }

        private static readonly List<EResourceTypes> PotionsList = new List<EResourceTypes>() {EResourceTypes.Potions};
        private static readonly List<EResourceTypes> PotionsAndEatList = new List<EResourceTypes>() {EResourceTypes.Eat, EResourceTypes.Potions};

        private void ResourceManagerOnOnChangeResources()
        {

            List<RaftStorage.StorageItem> potions = null;
            if (drawEat)
            {
                potions = resourcesService.GetItemsByTypes(PotionsAndEatList);
            }
            else
            {
                potions = resourcesService.GetItemsByTypes(PotionsList);
            }

            gameObject.SetActive(potions.Count != 0 && scenesService.GetActiveScene() != ESceneName.Map);

            if (potions.Count != 0)
            {
                for (int i = items.Count; i < potions.Count + 1; i++)
                {
                    Instantiate(item, holder)
                        .With(x => items.Add(x));
                }

                for (int i = 0; i < items.Count; i++)
                {
                    items[i].gameObject.SetActive(i < potions.Count);

                    if (i < potions.Count)
                    {
                        items[i].Init(potions[i], this);
                    }
                }
            }
        }

        public void StartDrag(RaftStorage.StorageItem storageItem)
        {
            draggedItem.transform.DOKill();
            draggedItem.transform.localScale = Vector3.zero;
            draggedItem.transform.DOScale(1, 0.2f);
            draggedItem.sprite = storageItem.Item.ItemIcon;
            draggedItem.gameObject.SetActive(true);

            StartCoroutine(DragProcess(storageItem.Item));
        }

        IEnumerator DragProcess(ItemObject storageItem)
        {
            while (InputService.IsLMBPressed)
            {
                yield return null;
                draggedItem.transform.position = InputService.MousePosition;
            }

            draggedItem.transform.DOKill();
            draggedItem.transform.DOScale(0, 0.2f).onComplete += delegate { draggedItem.gameObject.SetActive(false); };

            DropItem(storageItem);
        }

        private void DropItem(ItemObject storageItem)
        {
            PlayerCharacter character = null;
            if (charactersList)
            {
                character = charactersList.GetCharacterUnderMouse();
                if (character != null)
                {
                    AddPotionEffect(storageItem, character);
                    return;
                }
            }

            var pos = selectionService.GetUnderMousePosition(out var isHitted);
            if (isHitted)
            {
                character = characterService.GetClosestCharacter(pos, out var minDist);

                if (minDist < 3)
                {
                    AddPotionEffect(storageItem, character);
                    return;
                }
            }

            WorldPopupService.StaticSpawnCantPopup(pos);
        }

        private void AddPotionEffect(ItemObject storageItem, ICharacter character)
        {
            var storageItemPotionLogic = storageItem.PotionLogic;


            if (storageItem.Type == EResourceTypes.Eat || storageItemPotionLogic.PotionType == PotionLogicBaseSO.EPotionType.Moment)
            {
                character.ActivatePotion(storageItem);
            }
            else
            {
                var ch = characterService.GetSpawnedCharacters().Find(x => !x.IsHaveEffect(storageItemPotionLogic.GetPotionBonusValue()));
                if (ch)
                {
                    ch.ActivatePotion(storageItem);
                }
                else
                {
                    character.ActivatePotion(storageItem);
                }
            }

            resourcesService.RemoveItemFromAnyRaft(storageItem);
            resourcesService.PlayerItemsList();
        }

        public void AddEffectToSelected(ItemObject storageItem)
        {
            AddPotionEffect(storageItem, selectionService.SelectedCharacter);
        }
    }
}
