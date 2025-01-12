using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Scriptable;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using TMPro;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.UI
{
    public class UISoulsShopWindow : AnimatedWindow
    {
        [System.Serializable]
        public class SpawnedItem
        {
            [SerializeField] private UISoulsShopItem item;
            [SerializeField] private RenderTexture renderTexture;
            [SerializeField] private CameraRendererPreview spawned;

            public SpawnedItem(UISoulsShopItem item, CameraRendererPreview spawned)
            {
                this.item = item;
                this.spawned = spawned;
                renderTexture = spawned.CreateRenderTexture(512, 512);


                item.ApplyRenderTexture(renderTexture);
            }

            public void Active(bool state)
            {
                spawned.gameObject.SetActive(state);
            }

            public void UpdateItem()
            {
                item.UpdateItem();
            }
        }

        [SerializeField] private UISoulsShopItem item;
        [SerializeField] private TMP_Text soulsCounter;
        private List<SpawnedItem> spawnedItems = new List<SpawnedItem>();
        private GameDataObject gameDataObject;
        private SaveDataObject saveDataObject;

        private bool isContainerOwned = false;
        private GameStateService gameStateService;


        public void Init(GameDataObject gameDataObject, SaveDataObject saveDataObject, SelectionService selectionService, GameStateService gameStateService)
        {
            this.gameStateService = gameStateService;
            this.saveDataObject = saveDataObject;
            this.gameDataObject = gameDataObject;
            
            selectionService.OnChangeSelectCharacter += OnChangeSelectCharacter;
            OnChangeSelectCharacter(selectionService.SelectedCharacter);
        }
        
        private void OnChangeSelectCharacter(PlayerCharacter obj)
        {
            if (obj == null) return;
            var furnaceAction = obj.GetCharacterAction<CharActionSoulsShopView>();
            furnaceAction.OnOpenWindow -= ShowWindow;
            furnaceAction.OnOpenWindow += ShowWindow;
            
            obj.NeedManager.OnDeath -= OnDeath;
            obj.NeedManager.OnDeath += OnDeath;
        }
        
        private void OnDeath(Character obj)
        {
            CloseWindow();
        }

        public override void ShowWindow()
        {
            if (gameStateService.GameState != GameStateService.EGameState.Normal) return;
            if (isContainerOwned) return;
            base.ShowWindow();
            
            Redraw();
        }

        public override void CloseWindow()
        {
            base.CloseWindow();
            foreach (var spawned in spawnedItems)
            {
                spawned.Active(false);
            }
        }

        private void Redraw()
        {
            if (spawnedItems.Count == 0)
            {
                int id = 0;
                item.gameObject.SetActive(true);
                foreach (var it in gameDataObject.SoulsShopContainers)
                {
                    var preview = Instantiate(it.PreviewPrefab, new Vector3(50 * id, 1000, 0), Quaternion.identity);
                    var spawnedItem = Instantiate(item, item.transform.parent)
                        .With(x => x.Init(it, saveDataObject, this));
                    spawnedItems.Add(new SpawnedItem(spawnedItem, preview));
                    id++;
                }
                item.gameObject.SetActive(false);
            }

            for (int i = 0; i < spawnedItems.Count; i++)
            {
                spawnedItems[i].Active(true);
                spawnedItems[i].UpdateItem();
            }


            soulsCounter.text = saveDataObject.CrossGame.SoulsCount.ToString();
        }

        public void BuyContainer(SoulsResourceContainerSO container)
        {
            saveDataObject.CrossGame.RemoveSouls(container.SoulsCount);
            saveDataObject.PlayerInventory.AddItemsToPlayerStorage(container.Resources.ToList());
            saveDataObject.SaveFile();
            isContainerOwned = true;
            CloseWindow();
        }
    }
}
