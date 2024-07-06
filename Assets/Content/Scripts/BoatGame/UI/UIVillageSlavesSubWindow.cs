using System;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageSlavesSubWindow : AnimatedWindow
    {
        [System.Serializable]
        public class DisplayCharacter
        {
            [SerializeField] private Character character;
            [SerializeField] private UIVillageSlavesHolder display;
            [SerializeField] private GameObject prefab;
            [SerializeField] private string description;
            [SerializeField] private int cost;

            public DisplayCharacter(Character character, GameObject prefab, string description, int cost)
            {
                this.cost = cost;
                this.character = character;
                this.prefab = prefab;
                this.description = description;
            }

            public GameObject Prefab => prefab;

            public UIVillageSlavesHolder Display => display;

            public Character Character => character;

            public string Description => description;

            public int Cost => cost;

            public void SetDisplay(UIVillageSlavesHolder uiVillageSlavesHolder)
            {
                uiVillageSlavesHolder.Init(prefab);
                display = uiVillageSlavesHolder;
            }
        }

        [SerializeField] private TextAsset unitsDescriptions;
        [SerializeField] private UIVillageSlavesHolder holderPrefab;
        [SerializeField] private UIVillageSlaveItem item;
        
        private List<UIVillageSlaveItem> spawnedItems = new List<UIVillageSlaveItem>();
        private List<DisplayCharacter> characters = new List<DisplayCharacter>(5);
        private GameDataObject gameData;
        private ResourcesService resourcesService;
        private UIVillageOptionsWindow baseWindow;
        private SaveDataObject saveData;

        private string lastVillage;

        public void Init(
            GameDataObject gameData, 
            SaveDataObject saveData,
            Random rnd, 
            int islandLevel, 
            ResourcesService resourcesService, 
            UIVillageOptionsWindow window)
        {
            this.saveData = saveData;
            this.baseWindow = window;
            this.resourcesService = resourcesService;
            this.gameData = gameData;

            if (baseWindow.GetVillage().Uid != lastVillage)
            {
                lastVillage = baseWindow.GetVillage().Uid;
                
                foreach (var ch in characters)
                {
                    Destroy(ch.Display.gameObject);
                }
                characters.Clear();
            }

            
            
            var slaves = Character.GetSlavesList(rnd, this.gameData, islandLevel);

            var lines = unitsDescriptions.text.Split("\n").ToList();


            for (int i = 0; i < slaves.Count; i++)
            {
                characters.Add(new DisplayCharacter(slaves[i], gameData.NativesListData.NativesList.GetRandomItem(rnd).gameObject, lines.GetRandomItem(rnd), (int) ((gameData.NativesListData.BaseUnitCost * islandLevel) + (rnd.Next(0, 1000) * (islandLevel / 10f)))));
            }

        }


        private void OnEnable()
        {
            Redraw();
        }

        private void OnDisable()
        {
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].Display != null)
                {
                    characters[i].Display.gameObject.SetActive(true);
                }
            }
        }

        public void Redraw()
        {
            DisplaysConfiguration();
            SpawnItems();
            UpdateItems();
        }

        private void UpdateItems()
        {
            var village = baseWindow.GetVillage();
            for (int i = 0; i < spawnedItems.Count; i++)
            {
                spawnedItems[i].UpdateItem(village.IsHaveSlave(characters[i].Character.Uid));
            }
        }

        private void SpawnItems()
        {
            if (spawnedItems.Count == 0)
            {
                item.gameObject.SetActive(true);
                for (int i = 0; i < characters.Count; i++)
                {
                    var display = characters[i];
                    Instantiate(item, item.transform.parent)
                        .With(x => spawnedItems.Add(x))
                        .With(x => x.Init(display, resourcesService, gameData.ConfigData.MoneyItem, this));
                }

                item.gameObject.SetActive(false);
            }
        }

        private void DisplaysConfiguration()
        {
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].Display == null)
                {
                    var id = i;
                    Instantiate(holderPrefab, new Vector3(i * 10, 1000, 0), Quaternion.identity)
                        .With(x => characters[id].SetDisplay(x));


                    characters[i].Display.gameObject.SetActive(true);
                }
            }
        }

        public void BuySlave(DisplayCharacter displayCharacter)
        {
            if (baseWindow.BuySlave(displayCharacter.Character, displayCharacter.Cost))
            {
                UpdateItems();
            }
        }
    }
}
