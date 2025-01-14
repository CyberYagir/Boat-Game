using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Scriptable;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.DungeonGame.Mobs;
using Content.Scripts.IslandGame.Natives;
using Content.Scripts.IslandGame.Scriptable;
using Content.Scripts.IslandGame.WorldStructures;
using Content.Scripts.ItemsSystem;
using Content.Scripts.Map;
using Content.Scripts.Mobs;
using Content.Scripts.SkillsSystem;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.Global
{
    [System.Serializable]
    public class NameGenerator
    {
        public enum EGender
        {
            Male,
            Female
        }
    
        [SerializeField] private TextAsset maleNames;
        [SerializeField] private TextAsset femaleNames;

        private List<string> maleNamesList;
        private List<string> femaleNamesList;


        public void Init()
        {
            maleNamesList = maleNames.LinesToList();
            femaleNamesList = femaleNames.LinesToList();
        }


        public string GetName(EGender gender)
        {
            if (gender == EGender.Male)
            {
                return maleNamesList.GetRandomItem();
            }
            else
            {
                return femaleNamesList.GetRandomItem();
            }
        }
        
        public string GetName(EGender gender, System.Random rnd)
        {
            if (gender == EGender.Male)
            {
                return maleNamesList.GetRandomItem(rnd);
            }
            else
            {
                return femaleNamesList.GetRandomItem(rnd);
            }
        }
    }

    [System.Serializable]
    public class ItemsManager
    {
        private Dictionary<string, ItemObject> itemsMap = new Dictionary<string, ItemObject>();
        private List<ItemObject> itemsList;

        public List<ItemObject> ItemsList => itemsList;

        public void Init(List<ItemObject> items)
        {
            this.itemsList = items;
            itemsMap.Clear();

            foreach (var it in items)
            {
                itemsMap.Add(it.ID, it);
            }
        }

        public ItemObject Find(string id)
        {
            if (itemsMap.ContainsKey(id))
            {
                return itemsMap[id];
            }

            return null;
        }
    }
    
    [CreateAssetMenu(menuName = "Scriptable/GameData", fileName = "Game Data", order = 0)]
    public class GameDataObject : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameDataObject>().FromInstance(this).AsSingle();

            crafts = Resources.LoadAll<CraftObject>("Crafts").ToList();

#if UNITY_EDITOR

            var startItemsCount = buildableStructures.Count;
            
            buildableStructures.Clear();
            
            var assets = AssetDatabase.FindAssets("t:Prefab", new string[] {"Assets/Content/Prefab/IslandGame/"});

            foreach (var asset in assets)
            {
                var o = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(asset));

                if (o != null)
                {
                    var buildable = o.GetComponent<BuildableStructure>();
                    if (buildable)
                    {
                        buildableStructures.Add(buildable);
                    }
                }
            }


            if (startItemsCount != buildableStructures.Count)
            {
                EditorUtility.SetDirty(this);
            }
#endif
            
            
            
            var items = Resources.LoadAll<ItemObject>("Item").ToList();


            Items.Init(items);
            ActionsData.Init();
            RaftsPriorityData.Init();
            NativesListData.Init();
            NamesList.Init();
        }


        [SerializeField] private ActionsDataSO actionsData;
        [SerializeField] private ConfigDataObject configData;
        [SerializeField] private RaftsPriorityObject raftsPriorityData;
        [SerializeField] private NativesListSO nativesListData;
        [SerializeField] private TradesDataObject tradesData;
        [SerializeField] private TextAsset plotLines;
        [SerializeField] private ItemsManager items;
        
        [SerializeField] private NameGenerator namesList = new NameGenerator();
        [SerializeField] private List<SkillObject> skillsList;
        [SerializeField] private List<Material> skinColors;
        [SerializeField] private List<CraftObject> crafts;
        [SerializeField] private List<MobObject> mobs;
        [SerializeField] private List<BuildableStructure> buildableStructures;
        [SerializeField] private List<int> levelXps;
        [SerializeField] private List<MapPathObject> mapPaths;
        [SerializeField] private List<RandomStructureMaterialsBase.MatsByBiome> structuresMaterials;
        [SerializeField] private List<RandomStructureMaterialsBase.MatsByBiome> structuresRoofMaterials;
        [SerializeField] private List<ActionsDataSO.IconsKeys<EResourceTypes>> resourcesIcons;
        [SerializeField] private List<SoulsResourceContainerSO> soulsShopContainers;

        public List<Material> SkinColors => skinColors;

        public List<SkillObject> SkillsList => skillsList;

        public NameGenerator NamesList => namesList;

        public List<CraftObject> Crafts => crafts;

        public List<int> LevelXps => levelXps;

        public List<MapPathObject> MapPaths => mapPaths;

        public ActionsDataSO ActionsData => actionsData;

        public List<RandomStructureMaterialsBase.MatsByBiome> StructuresMaterials => structuresMaterials;
        public List<RandomStructureMaterialsBase.MatsByBiome> StructuresRoofMaterials => structuresRoofMaterials;

        public ConfigDataObject ConfigData => configData;

        public ItemsManager Items => items;

        public RaftsPriorityObject RaftsPriorityData => raftsPriorityData;

        public NativesListSO NativesListData => nativesListData;

        public TradesDataObject TradesData => tradesData;

        public TextAsset PlotLines => plotLines;

        public List<SoulsResourceContainerSO> SoulsShopContainers => soulsShopContainers;

        public List<BuildableStructure> BuildableStructures => buildableStructures;


        public ItemObject GetItem(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return items.Find(id);
            }

            return null;
        }


        public CraftObject GetCraftByID(string id)
        {
            return Crafts.Find(x => x.Uid == id);
        }

        [Button]
        public void GenerateLevelProgression()
        {
            LevelXps.Clear();

            for (int i = 0; i < 100; i++)
            {
                LevelXps.Add(Mathf.RoundToInt(Mathf.Pow(i / 0.5f, 1.15f)));
            }
        }

        public int GetLevelXP(int skillDataLevel)
        {
            return levelXps[Mathf.Clamp(skillDataLevel, 0, levelXps.Count - 1)];
        }

        public MobObject GetMob(MobObject.EMobType mobType)
        {
            return mobs.Find(x => x.Type == mobType);
        }
        
        public Sprite GetResourceIcon(EResourceTypes type)
        {
            return resourcesIcons.Find(x => x.Key == type).Icon;
        }
    }
}
