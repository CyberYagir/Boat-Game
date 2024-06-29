using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using Content.Scripts.SkillsSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame
{
    public partial class PlayerCharacter
    {
        [System.Serializable]
        public class NeedsManager
        {
            
            public const int MINIMAL_SCORES = 40;
            public const int REGEN_SCORES = 40;
            [System.Serializable]
            public class PopUp
            {
                [SerializeField] private Transform needPopup;
                [SerializeField] private Image image;
                [SerializeField] private Sprite eat, water, skull;
                
                
                private Transform parent;
                private Vector3 needPopupPoint;

                public bool isActive => needPopup.gameObject.active;
                
                
                public void Init()
                {
                    parent = needPopup.parent;
                    needPopupPoint = needPopup.localPosition;
                
                    needPopup.transform.SetParent(null);
                    needPopup.gameObject.SetActive(false);
                }


                public void Update(SelectionService selectionService)
                {
                    if (needPopup.gameObject.active)
                    {
                        needPopup.transform.LookAt(selectionService.Camera.transform);
                        needPopup.transform.position = Vector3.Lerp(needPopup.transform.position, parent.TransformPoint(needPopupPoint), 10 * TimeService.DeltaTime);
                    }
                }
                
                public void ShowNeedPopup(bool state, float thirsty, float hunger)
                {
                    if (!needPopup.gameObject.activeInHierarchy && state)
                    {
                        needPopup.transform.position = parent.TransformPoint(needPopupPoint);
                    }
                    needPopup.gameObject.SetActive(state);
                    if (state)
                    {
                        image.sprite = GetSprite(thirsty, hunger);
                    }
                }

                public Sprite GetSprite(float thirsty, float hunger)
                {
                    if (hunger < MINIMAL_SCORES && thirsty < MINIMAL_SCORES || (hunger <= 0 || thirsty <= 0))
                    {
                        return skull;
                    }
                    if (hunger < MINIMAL_SCORES)
                    {
                        return eat;
                    }
                    if (thirsty < MINIMAL_SCORES)
                    {
                        return water;
                    }

                    return null;
                }
            }

            
            [SerializeField] private float health = 100;
            [SerializeField] private float hunger = 100;
            [SerializeField] private float thirsty = 100;

            [SerializeField, ReadOnly] private bool isDead;
            [SerializeField] private float rate;
            [SerializeField] private PopUp popUp;
            [SerializeField] private SkillObject vitalitySkill;

            public Action<Character> OnDeath;
            public Action OnDamaged;
            
 
            private WeatherService.WeatherModifiers currentModifiers;
            private SelectionService selectionService;
            private Character selfCharacter;
            [SerializeField, ReadOnly] private bool godMode;

            public bool IsDead => isDead;
            public float Health => health;
            public float Hunger => hunger;
            public float Thirsty => thirsty;

            public void Init(Character character, WeatherService weatherService, SelectionService selectionService)
            {
                selfCharacter = character;
                this.selectionService = selectionService;
                var parametersData = character.Parameters;
                health = parametersData.Health;
                hunger = parametersData.Hunger;
                thirsty = parametersData.Thirsty;

                currentModifiers = weatherService.CurrentModifiers;

                popUp.Init();
            }

            public void SetGodMode(bool state = true)
            {
                godMode = state;
            }

            public void OnTick(float delta)
            {
                if (!IsDead)
                {
                    delta *= rate;
                    var vitalityModify = selfCharacter.GetSkillMultiply(vitalitySkill.SkillID);
                    RemoveParameters(delta, vitalityModify);
                    CalculateHealth(delta, vitalityModify);
                    ClampValues();
                }
            }


            public void Update()
            {
                if (!IsDead)
                {
                    popUp.Update(selectionService);
                }
            }

            private void RemoveParameters(float delta, float vitalityModify)
            {
                hunger -= (delta * currentModifiers.Hunger * vitalityModify);
                thirsty -= (delta * currentModifiers.Thirsty * vitalityModify);

                popUp.ShowNeedPopup(Hunger < MINIMAL_SCORES || Thirsty < MINIMAL_SCORES, Thirsty, Hunger);
            }

            private void CalculateHealth(float delta, float vitalityModify)
            {
                
                if (Thirsty > REGEN_SCORES && Hunger > REGEN_SCORES)
                {
                    health += delta * (1 + (1f - vitalityModify));
                }

                if (Thirsty <= 0 || Hunger <= 0)
                {
                    if (!godMode)
                    {
                        health -= (delta * vitalityModify);
                    }
                    else
                    {
                        health = 100;
                    }

                    HealthCheck();
                }
            }

            private void HealthCheck()
            {
                if (Health <= 0 && !godMode)
                {
                    popUp.ShowNeedPopup(false, 0, 0);
                    Death();
                }
            }

            public void ClampValues()
            {
                health = Mathf.Clamp(Health, 0, 100);
                hunger = Mathf.Clamp(Hunger, 0, 100);
                thirsty = Mathf.Clamp(Thirsty, 0, 100);
            }

            public void AddParametersByItemName(ItemObject item)
            {
                var param = item.ParametersData;

                AddParameters(param);
            }

            public void AddParameters(Character.ParametersData param)
            {
                health += param.Health;
                hunger += param.Hunger;
                thirsty += param.Thirsty;
            }

            public Character.ParametersData GetParameters()
            {
                return new Character.ParametersData(Health, Hunger, Thirsty);
            }

            public void Death()
            {
                isDead = true;
                health = -100;
                hunger = 0;
                thirsty = 0;
                OnDeath?.Invoke(selfCharacter);
            }


            public Sprite GetCurrentIcons(out bool isActive)
            {
                isActive = popUp.isActive;
                return popUp.GetSprite(thirsty, hunger);
            }

            public void Damage(float dmg)
            {
                if (godMode) return;
                print(dmg);
                health -= dmg;
                HealthCheck();
                OnDamaged?.Invoke();
            }
        }
    }
}