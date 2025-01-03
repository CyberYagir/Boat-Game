﻿using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Scriptable;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using Content.Scripts.SkillsSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public partial class PlayerCharacter
    {
        [System.Serializable]
        public class CharacterParameters
        {
            public enum EffectBonusValueType
            {
                Regen,
                Defence,
                Attack
            }
            
            [SerializeField] private float baseDamage = 15;
            [SerializeField] private float startSpeed = 2;
            [SerializeField] private EatHealingSO eatHealingLogic;
            [SerializeField, ReadOnly] private List<PotionLogicBaseSO> activeEffects = new List<PotionLogicBaseSO>();
            [SerializeField] private SkillObject movingSkill;
            [SerializeField] private SkillObject strengthSkill;

            private Dictionary<EffectBonusValueType, float> effectsBonuses = new Dictionary<EffectBonusValueType, float>();
            private Character selfCharacter;
            private GameDataObject gameData;
            private PlayerCharacter playerCharacter;
            private Character character;


            public float Defence => CalculateDefencePercent();
            public float Damage => CalculateDamage();
            public float Speed => CalcualteSpeed();
            public float BaseDamage => baseDamage;

            private float CalcualteSpeed()
            {
                return startSpeed + (character.GetSkillMultiplyAdd(movingSkill.SkillID));
            }

            private float CalculateDamage()
            {
                float damage = baseDamage;
                var targetCharacterWeapon = gameData.GetItem(selfCharacter.Equipment.WeaponID);
                if (targetCharacterWeapon != null)
                {
                    damage = targetCharacterWeapon.ParametersData.Damage;
                }
                return (damage * GetEffectMult(EffectBonusValueType.Attack)) + ((1f -character.GetSkillMultiply(strengthSkill.SkillID)) * damage);
            }

            public void Init(Character selfCharacter, GameDataObject gameData, PlayerCharacter playerCharacter)
            {
                this.character = selfCharacter;
                this.playerCharacter = playerCharacter; 
                this.gameData = gameData;
                this.selfCharacter = selfCharacter;

                foreach (var effect in selfCharacter.EffectsData.EffectsList)
                {
                    var itemEffectSender = gameData.GetItem(effect.ItemSenderID);
                    if (itemEffectSender)
                    {
                        var logic = ApplyEffect(itemEffectSender);

                        logic.SetSeconds(effect.RemainingSeconds);
                    }
                }
                
                playerCharacter.StartCoroutine(EffectsLoop());
            }

            private IEnumerator EffectsLoop()
            {
                List<PotionLogicBaseSO> unactiveEffects = new List<PotionLogicBaseSO>();
                while (true)
                {
                    yield return new WaitForSecondsRealtime(1f);

                    effectsBonuses.Clear();
                    unactiveEffects.Clear();
                    
                    foreach (var active in activeEffects)
                    {
                        active.RemoveSecond();
                        if (active.Seconds <= 0)
                        {
                            unactiveEffects.Add(active);
                        }
                        else
                        {
                            active.AddEffectBonus();
                        }
                    }
                    
                    foreach (var c in unactiveEffects)
                    {
                        c.StopEffect();
                        activeEffects.Remove(c);
                    }
                }
            }

            private float GetEffectMult(EffectBonusValueType effectType)
            {
                if (effectsBonuses.ContainsKey(effectType))
                {
                    return effectsBonuses[effectType];
                }

                return 1f;
            }

            public void AddEffectMult(EffectBonusValueType effectType, float mult)
            {
                if (!effectsBonuses.ContainsKey(effectType))
                {
                    effectsBonuses.Add(effectType, 0);
                }

                if (effectsBonuses[effectType] < 1f)
                {
                    effectsBonuses[effectType] += mult;
                }
                else
                {
                    effectsBonuses[effectType] += (mult - 1);
                }
            }

            private float CalculateDefencePercent()
            {
                var armor = gameData.GetItem(selfCharacter.Equipment.ArmorID);
                var helmet = gameData.GetItem(selfCharacter.Equipment.HelmetID);
                float defencePercent = 0;

                if (armor != null)
                {
                    defencePercent += armor.ParametersData.Defence;
                }

                if (helmet != null)
                {
                    defencePercent += helmet.ParametersData.Defence;
                }

                defencePercent = (1f - defencePercent);
                if (defencePercent == 0 && effectsBonuses.ContainsKey(EffectBonusValueType.Defence)) //if without armor but have effect
                {
                    defencePercent = 1;
                }

                return defencePercent * GetEffectMult(EffectBonusValueType.Defence);
            }

            public float ModifyDamageByDefence(float dmg)
            {
                return dmg * Defence;
            }

            public PotionLogicBaseSO ApplyEffect(ItemObject item)
            {
                var so = item.PotionLogic;

                if (item.Type == EResourceTypes.Eat)
                {
                    so = eatHealingLogic;
                }
                
                
                PotionLogicBaseSO logic = Instantiate(so);

                if (logic.PotionType == PotionLogicBaseSO.EPotionType.Moment)
                {
                    logic.StartEffect(playerCharacter, item);
                }
                else
                {
                    var isHaveEffect = activeEffects.Find(x => x.Uid == logic.Uid) != null;

                    if ((isHaveEffect && logic.Stackable) || !isHaveEffect)
                    {
                        activeEffects.Add(logic);
                        logic.StartEffect(playerCharacter, item);
                    }
                }

                return logic;
            }

            public List<Character.EffectsListData.SavedEffectData> GetEffectsData()
            {
                List<Character.EffectsListData.SavedEffectData> list = new List<Character.EffectsListData.SavedEffectData>();

                for (int i = 0; i < activeEffects.Count; i++)
                {
                    list.Add(new Character.EffectsListData.SavedEffectData(activeEffects[i].Sender.ID, activeEffects[i].Seconds));
                }

                return list;
            }

            public bool IsHaveEffect(EffectBonusValueType type)
            {
                return effectsBonuses.ContainsKey(type);
            }
        }
    }
}