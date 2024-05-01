using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.IslandGame.WorldStructures;
using Content.Scripts.ItemsSystem;
using Content.Scripts.Map;
using Content.Scripts.Mobs;
using Content.Scripts.SkillsSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Global
{
    [CreateAssetMenu(menuName = "Scriptable/GameData", fileName = "Game Data", order = 0)]
    public class GameDataObject : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameDataObject>().FromInstance(this).AsSingle();

            crafts = Resources.LoadAll<CraftObject>("Crafts").ToList();
            items = Resources.LoadAll<ItemObject>("Item").ToList();

            ActionsData.Init();
        }


        [SerializeField] private ActionsDataSO actionsData;
        [SerializeField] private ConfigDataObject configData;
        [SerializeField] private List<string> namesList;
        [SerializeField] private List<SkillObject> skillsList;
        [SerializeField] private List<Material> skinColors;
        [SerializeField] private List<CraftObject> crafts;
        [SerializeField] private List<ItemObject> items;
        [SerializeField] private List<MobObject> mobs;
        [SerializeField] private List<int> levelXps;
        [SerializeField] private List<MapPathObject> mapPaths;
        [SerializeField] private List<RandomStructureMaterialsBase.MatsByBiome> structuresMaterials;
        [SerializeField] private List<RandomStructureMaterialsBase.MatsByBiome> structuresRoofMaterials;
        [SerializeField] private List<ActionsDataSO.IconsKeys<EResourceTypes>> resourcesIcons;

        public List<Material> SkinColors => skinColors;

        public List<SkillObject> SkillsList => skillsList;

        public List<string> NamesList => namesList;

        public List<CraftObject> Crafts => crafts;

        public List<int> LevelXps => levelXps;

        public List<MapPathObject> MapPaths => mapPaths;

        public ActionsDataSO ActionsData => actionsData;

        public List<RandomStructureMaterialsBase.MatsByBiome> StructuresMaterials => structuresMaterials;
        public List<RandomStructureMaterialsBase.MatsByBiome> StructuresRoofMaterials => structuresRoofMaterials;

        public ConfigDataObject ConfigData => configData;

        public List<ItemObject> Items => items;


        public ItemObject GetItem(string id)
        {
            return Items.Find(x => x.ID == id);
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
