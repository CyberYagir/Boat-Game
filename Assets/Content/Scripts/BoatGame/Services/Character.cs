using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Natives;
using Content.Scripts.ItemsSystem;
using Content.Scripts.QuestsSystem;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.BoatGame.Services
{
    [System.Serializable]
    public class Character
    {
        [System.Serializable]
        public class Skill
        {
            [SerializeField] private string skillID;
            [SerializeField] private int value;

            public Skill(string skillID, int value)
            {
                this.skillID = skillID;
                this.value = value;
                if (value == 1)
                {
                    QuestsEventBus.CallUpgradeSkill();
                }
            }

            public int Value => value;

            public string SkillID => skillID;

            public void Add(int i)
            {
                value += i;
                QuestsEventBus.CallUpgradeSkill();
            }
        }
        [System.Serializable]
        public class SkillsData
        {
            [SerializeField] private int xp;
            [SerializeField] private int scores;
            [SerializeField] private int level;
            [SerializeField] private List<Skill> skills = new List<Skill>();

            public int Level => level;

            public int Scores => scores;

            public int Xp => xp;

            public List<Skill> SkillsList => skills;

            public event Action OnLevelUp;

            public void RemoveScore()
            {
                scores--;
            }

            public void AddLevel()
            {
                xp = 0;
                level++;
                scores++;
                OnLevelUp?.Invoke();
            }

            public void AddXp(int exp)
            {
                xp += exp;
            }

            public void ClearEvents()
            {
                OnLevelUp = null;
            }
        }
        
        [System.Serializable]
        public class ParametersData
        {
            [SerializeField] private float health = 100;
            [SerializeField] private float hunger = 100;
            [SerializeField] private float thirsty = 100;

            public ParametersData(float health, float hunger, float thirsty)
            {
                this.health = health;
                this.hunger = hunger;
                this.thirsty = thirsty;
            }

            public ParametersData()
            {
            }

            public float Thirsty => thirsty;

            public float Hunger => hunger;

            public float Health => health;
        }

        [System.Serializable]
        public class EquipmentData
        {
            [SerializeField] private string helmetID, armorID, weaponID;

            public string WeaponID => weaponID;

            public string ArmorID => armorID;

            public string HelmetID => helmetID;
            public bool IsHaveHelmet => string.IsNullOrEmpty(HelmetID);


            public event Action OnEquipmentChange;


            public void SetHelmet(ItemObject item)
            {
                if (item == null)
                {
                    helmetID = "";
                    OnEquipmentChange?.Invoke();
                    return;
                }

                helmetID = item.ID;

                OnEquipmentChange?.Invoke();
            }

            public void SetArmor(ItemObject item)
            {
                if (item == null)
                {
                    armorID = "";
                    OnEquipmentChange?.Invoke();
                    return;
                }

                armorID = item.ID;

                OnEquipmentChange?.Invoke();
            }

            public void SetWeapon(ItemObject item)
            {
                if (item == null)
                {
                    weaponID = "";
                    OnEquipmentChange?.Invoke();
                    return;
                }

                weaponID = item.ID;

                OnEquipmentChange?.Invoke();
            }

            public string GetEquipment(EEquipmentType eEquipmentType)
            {
                switch (eEquipmentType)
                {
                    case EEquipmentType.Helmet:
                        return HelmetID;
                    case EEquipmentType.Armor:
                        return ArmorID;
                    case EEquipmentType.Weapon:
                        return WeaponID;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(eEquipmentType), eEquipmentType, null);
                }
            }

            public void SetEquipment(ItemObject item, EEquipmentType type)
            {
                switch (type)
                {
                    case EEquipmentType.Helmet:
                        SetHelmet(item);
                        break;
                    case EEquipmentType.Armor:
                        SetArmor(item);
                        break;
                    case EEquipmentType.Weapon:
                        SetWeapon(item);
                        break;
                }
            }

            public void ClearEvents()
            {
                OnEquipmentChange = null;
            }
        }

        [System.Serializable]
        public class EffectsListData
        {
            [System.Serializable]
            public class SavedEffectData
            {
                [SerializeField] private string itemSenderID;
                [SerializeField] private int remainingSeconds;

                public SavedEffectData(string itemSenderID, int remainingSeconds)
                {
                    this.itemSenderID = itemSenderID;
                    this.remainingSeconds = remainingSeconds;
                }

                public int RemainingSeconds => remainingSeconds;

                public string ItemSenderID => itemSenderID;
            }

            [SerializeField] private List<SavedEffectData> effectsList = new List<SavedEffectData>();
            public List<SavedEffectData> EffectsList => effectsList;

            public void SetData(List<SavedEffectData> data)
            {
                effectsList = data;
            }
        }

        [SerializeField] private string name;
        [SerializeField] private string uid;
        [SerializeField] private SkillsData skillsData = new SkillsData();
        [SerializeField] private ParametersData parametersData = new ParametersData();
        [SerializeField] private EquipmentData equipmentData = new EquipmentData();
        [SerializeField] private EffectsListData effectsData = new EffectsListData();
        [SerializeField] private int skinColorID;
        [SerializeField] private int hatID;
        
        
        public int SkinColorID => skinColorID;
        public List<Skill> Skills => skillsData.SkillsList;
        public string Name => name;
        
        public int HatID => hatID;

        public string Uid => uid;

        public ParametersData Parameters => parametersData;
        public SkillsData SkillData => skillsData;

        public EquipmentData Equipment => equipmentData;

        public EffectsListData EffectsData => effectsData;

        public event Action OnSkillUpgraded;

        public Character()
        {
            uid = Guid.NewGuid().ToString();
        }

        public void SetUid(string id) => uid = id;
        
        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetSkinID(int id)
        {
            skinColorID = id;
        }
        public void SetHatID(int id)
        {
            hatID = id;
        }

        public int GetSkillValue(string skillSkillID)
        {
            var skill = Skills.Find(x => x.SkillID == skillSkillID);


            if (skill != null)
            {
                return skill.Value;
            }

            return 0;
        }

        public const float SKILL_DIVIDER = 15f;

        public float GetSkillMultiply(string skllId)
        {
            return 1f - GetSkillMultiplyAdd(skllId);
        }
        public float GetSkillMultiplyAdd(string skllId)
        {
            return (GetSkillValue(skllId) / SKILL_DIVIDER);
        }

        public bool AddSkillValue(string id, int value)
        {
            var skill = Skills.Find(x => x.SkillID == id);

            if (skill == null)
            {
                if (value >= 0)
                {
                    Skills.Add(new Skill(id, value));
                    OnSkillUpgraded?.Invoke();
                    return true;
                }
            }
            else
            {

                if (skill.Value + value >= 0)
                {
                    skill.Add(value);
                    OnSkillUpgraded?.Invoke();
                    return true;
                }
            }

            return false;
        }

        public void SetParameters(ParametersData getParameters)
        {
            parametersData = getParameters;
        }

        public void ClearEvents()
        {
            skillsData.ClearEvents();
            equipmentData.ClearEvents();
        }
        

        public static List<SlaveCharacterInfo> GetSlavesList(System.Random rnd, GameDataObject gameData, int islandLevel)
        {
            var count = rnd.Next((int) gameData.NativesListData.SlavesOnIslandCount.min, (int) gameData.NativesListData.SlavesOnIslandCount.max);

            List<SlaveCharacterInfo> characters = new List<SlaveCharacterInfo>();
            for (int i = 0; i < count; i++)
            {
                var characterSeed = rnd.Next(-100000, 100000);
                var character = GenerateSlaveBySeed(gameData, islandLevel, characterSeed, out var type, out var skinUid);

                characters.Add(new SlaveCharacterInfo(character, type, characterSeed, islandLevel, skinUid));
            }

            return characters;
        }

        private static Character GenerateSlaveBySeed(GameDataObject gameData, int islandLevel, int seed, out ENativeType type, out string uid)
        {
            var characterRandom = new Random(seed);
            var character = new Character();
            var skin = gameData.NativesListData.GetRandomSlaveSkin(characterRandom);
            type = skin.NativeType;
            uid = skin.SkinUid;
            
            character.SetName(gameData.NamesList.GetName(type == ENativeType.Female ? NameGenerator.EGender.Female : NameGenerator.EGender.Male, characterRandom));
            character.SetUid(Extensions.GenerateSeededGuid(characterRandom).ToString());

            var level = islandLevel * gameData.SkillsList.Count - 1;
            for (int j = 0; j < level; j++)
            {
                character.AddSkillValue(gameData.SkillsList.GetRandomItem(characterRandom).SkillID, 1);
            }

            return character;
        }

        public static List<SlaveCharacterInfo> GetSlavesFromList(List<SaveDataObject.MapData.IslandData.VillageData.SlaveData> slaves, GameDataObject gameData)
        {
            List<SlaveCharacterInfo> characters = new List<SlaveCharacterInfo>();
            for (int i = 0; i < slaves.Count; i++)
            {
                var islandLevel = slaves[i].TransferInfo.IslandLevel;
                var characterSeed = slaves[i].TransferInfo.Seed;
                var character = GenerateSlaveBySeed(gameData, islandLevel, characterSeed, out var type, out var skinUid);

                characters.Add(new SlaveCharacterInfo(character, type, characterSeed, islandLevel, skinUid));
            }

            return characters;
        }

        public void SetEffects(List<EffectsListData.SavedEffectData> data)
        {
            EffectsData.SetData(data);
        }
    }
}