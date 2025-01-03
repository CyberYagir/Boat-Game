﻿using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Equipment;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using Content.Scripts.ManCreator;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public partial class PlayerCharacter
    {
        [System.Serializable]
        public class AppearanceManager
        {
            public enum EBones
            {
                Spine1,
                Head,
                LeftShoulder,
                RightShoulder,
                RightHand,
                BackSword,
                Hips,
                Spine2,
                RightLeg,
                LeftLeg,
                LeftHand,
                RightForeArm,
                LeftForeArm,
                BackSabre,
            }

            [SerializeField] private Renderer renderer;
            [SerializeField] private GameObject selectedCircle;
            [SerializeField] private HatsHolder hatsHolder;
            [SerializeField] private GameObject torchPrefab;
            [SerializeField] private ParticleSystem inWaterRippleParticlePrefab;
            [SerializeField] private ParticleSystem levelUpParticlesPrefab;
            [SerializeField] private ParticleSystem healingParticle;
            [SerializeField] private ParticleSystem defenceParticle;
            [SerializeField] private ParticleSystem attackParticle;
            [SerializeField] private ParticleSystem healSlaveParticle;
            [SerializeField] private List<Transform> bones;



            private Dictionary<EBones, Transform> bonesMap = new Dictionary<EBones, Transform>();
            private Character character;
            private bool inHood;
            private GameDataObject gameData;
            private bool weaponInHand;

            private GameObject spawnedHelmet;
            private GameObject spawnedArmor;
            private GameObject spawnedWeapon;


            private ItemObject helmetItem;
            private ItemObject armorItem;
            private ItemObject weaponItem;

            public ItemObject WeaponItem => weaponItem;
            public ItemObject ArmorItem => armorItem;
            public ItemObject HelmetItem => helmetItem;

            public bool InHood => inHood;

            public void Init(Character character, GameDataObject gameData)
            {
                this.gameData = gameData;
                this.character = character;
                renderer.material = gameData.SkinColors[character.SkinColorID];
                hatsHolder.ShowHat(character.HatID);

                character.Equipment.OnEquipmentChange -= OnEquipmentChange;
                character.Equipment.OnEquipmentChange += OnEquipmentChange;
                
                character.SkillData.OnLevelUp -= SkillDataOnOnLevelUp;
                character.SkillData.OnLevelUp += SkillDataOnOnLevelUp;
                
                if (bonesMap.Count == 0)
                {
                    var names = Enum.GetNames(typeof(EBones));
                    for (int i = 0; i < names.Length; i++)
                    {
                        bonesMap.Add((EBones) i, bones.Find(x => x.name.Contains(names[i])));
                    }
                }
                
                OnEquipmentChange();
            }

            public void PlayHealParticles()
            {
                healingParticle.Play(true);
            }
            public void PlayHealSlaveParticles()
            {
                healSlaveParticle.Play(true);
            }

            public void PlayDefenceParticles(bool state)
            {
                if (state)
                {
                    defenceParticle.Play(true);
                }
                else
                {
                    defenceParticle.Stop(true);
                }
            }
            
            public void PlayAttackParticles(bool state)
            {
                if (state)
                {
                    attackParticle.Play(true);
                }
                else
                {
                    attackParticle.Stop(true);
                }
            }

            private void SkillDataOnOnLevelUp()
            {
                if (levelUpParticlesPrefab)
                {
                    levelUpParticlesPrefab.Play(true);
                    WorldPopupService.StaticSpawnPopup(GetBone(EBones.Hips).transform.position, "Level Up!");
                }
            }

            private void OnEquipmentChange()
            {
                
                spawnedHelmet = RespawnItem(spawnedHelmet, character.Equipment.HelmetID, out helmetItem);
                spawnedArmor = RespawnItem(spawnedArmor, character.Equipment.ArmorID, out armorItem);
                spawnedWeapon = RespawnItem(spawnedWeapon, character.Equipment.WeaponID, out weaponItem);

                ActiveMeleeWeapon(weaponInHand);
                
                SetHatState(InHood, 0);

                GameObject RespawnItem(GameObject spawned, string id, out ItemObject item)
                {
                    if (spawned != null)
                    {
                        Destroy(spawned);
                    }

                    if (!string.IsNullOrEmpty(id))
                    {
                        item = gameData.GetItem(id);
                        if (item != null)
                        {
                            if (item.Prefab != null)
                            {
                                Transform targetBone = GetBone(item.Prefab.GetComponent<EquipmentWorker>().TargetBone);

                                var spawn = Instantiate(item.Prefab, targetBone)
                                    .With(x => x.GetComponent<EquipmentWorker>()
                                        .Init(this));

                                return spawn;
                            }
                        }
                    }

                    item = null;
                    return null;
                }
            }


            private Tween hoodTween = null;
            public void SetHatState(bool state, float time = 1)
            {
                if (hatsHolder == null) return;
                
                if (hoodTween != null)
                {
                    hoodTween.Kill();
                }
                if (!state)
                {
                    hoodTween = DOVirtual.DelayedCall(time, delegate
                    {
                        if (spawnedHelmet == null)
                        {
                            hatsHolder.gameObject.SetActive(true);
                        }
                        else
                        {
                            spawnedHelmet.gameObject.SetActive(true);
                            hatsHolder.gameObject.SetActive(false);
                        }
                    });
                }
                else
                {
                    hatsHolder.gameObject.SetActive(false);
                    if (spawnedHelmet != null)
                    {
                        spawnedHelmet.gameObject.SetActive(false);
                    }
                }
            }

            public Transform GetBone(EBones bone)
            {
                return bonesMap[bone];
            }
            
            public void ChangeSelection(bool state)
            {
                selectedCircle.SetActive(state);
            }

            public void SetInHood(bool inHood)
            {
                this.inHood = inHood;
            }

            public bool CanChangeLayerOnDeath()
            {
                if (bonesMap[EBones.Spine1].position.y < 0f)
                {
                    if (bonesMap[EBones.Spine1].gameObject.layer == LayerMask.NameToLayer("PlayerDead"))
                    {
                        Instantiate(inWaterRippleParticlePrefab, bonesMap[EBones.Spine1].position, Quaternion.identity).Play(true);
                        return true;
                    }
                }

                return false;
            }

            public void ActiveMeleeWeapon(bool state)
            {
                if (spawnedWeapon == null)
                {
                    weaponInHand = state;
                    return;
                }
                if (state)
                {
                    if (!weaponInHand)
                    {
                        ChangeWeaponParent(spawnedWeapon.transform, GetBone(EBones.RightHand));
                        weaponInHand = true;
                    }
                }
                else
                {
                    if (weaponInHand)
                    {
                        ChangeWeaponParent(spawnedWeapon.transform, GetBone(weaponItem.AnimationType != EWeaponAnimationType.Sabre ? EBones.BackSword : EBones.BackSabre));
                        weaponInHand = false;
                    }
                }
            }

            public void ChangeWeaponParent(Transform target, Transform newParent)
            {
                target.transform.parent = newParent;
                target.transform.localPosition = Vector3.zero;;
                target.transform.localEulerAngles = Vector3.zero;;
                target.transform.localScale = Vector3.one;;
            }

            public void ShowTorch()
            {
                Instantiate(torchPrefab, bonesMap[EBones.LeftHand]);
            }
        }
    }
}