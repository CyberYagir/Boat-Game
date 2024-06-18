using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.ItemsSystem;
using UnityEngine;

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
            }

            public int Value => value;

            public string SkillID => skillID;

            public void Add(int i)
            {
                value += i;
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


        [SerializeField] private string name;
        [SerializeField] private string uid;
        [SerializeField] private SkillsData skillsData = new SkillsData();
        [SerializeField] private ParametersData parametersData = new ParametersData();
        [SerializeField] private EquipmentData equipmentData = new EquipmentData();
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

        public Character()
        {
            uid = Guid.NewGuid().ToString();
        }
        
        
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


        public float GetSkillMultiply(string skllId)
        {
            return 1f - (GetSkillValue(skllId) / 15f);
        }

        public bool AddSkillValue(string id, int value)
        {
            var skill = Skills.Find(x => x.SkillID == id);

            if (skill == null)
            {
                if (value >= 0)
                {
                    Skills.Add(new Skill(id, value));
                    return true;
                }
            }
            else
            {

                if (skill.Value + value >= 0)
                {
                    skill.Add(value);
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
    }
}