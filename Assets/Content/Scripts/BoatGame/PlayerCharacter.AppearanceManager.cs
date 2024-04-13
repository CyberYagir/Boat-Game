using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Equipment;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
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
                Spine2
            }

            [SerializeField] private Renderer renderer;
            [SerializeField] private GameObject selectedCircle;
            [SerializeField] private HatsHolder hatsHolder;
            [SerializeField] private ParticleSystem inWaterRippleParticlePrefab;
            [SerializeField] private ParticleSystem levelUpParticlesPrefab;
            [SerializeField] private List<Transform> bones;



            private Dictionary<EBones, Transform> bonesMap = new Dictionary<EBones, Transform>();
            private Character character;
            private bool inHood;
            private GameDataObject gameData;
            private bool weaponInHand;

            private GameObject spawnedHelmet;
            private GameObject spawnedArmor;
            private GameObject spawnedWeapon;

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

            private void SkillDataOnOnLevelUp()
            {
                if (levelUpParticlesPrefab)
                {
                    levelUpParticlesPrefab.Play(true);
                }
            }

            private void OnEquipmentChange()
            {

                spawnedHelmet = RespawnItem(spawnedHelmet, character.Equipment.HelmetID);
                spawnedArmor = RespawnItem(spawnedArmor, character.Equipment.ArmorID);
                spawnedWeapon = RespawnItem(spawnedWeapon, character.Equipment.WeaponID);

                ActiveMeleeWeapon(weaponInHand);
                
                SetHatState(InHood, 0);

                GameObject RespawnItem(GameObject spawned, string id)
                {
                    if (spawned != null)
                    {
                        Destroy(spawned);
                    }

                    if (!string.IsNullOrEmpty(id))
                    {
                        var item = gameData.GetItem(id);
                        if (item != null)
                        {
                            Transform targetBone = GetBone(item.Prefab.GetComponent<EquipmentWorker>().TargetBone);
                            var spawn = Instantiate(item.Prefab, targetBone)
                                .With(x => x.GetComponent<EquipmentWorker>()
                                    .Init(this));

                            return spawn;
                        }
                    }

                    return null;
                }
            }


            private Tween hoodTween = null;
            public void SetHatState(bool state, float time = 1)
            {
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
                        ChangeWeaponParent(spawnedWeapon.transform, GetBone(EBones.BackSword));
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
        }
    }
}