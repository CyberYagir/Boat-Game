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
            [System.Serializable]
            public class PopUp
            {
                [SerializeField] private Transform needPopup;
                [SerializeField] private Image image;
                [SerializeField] private Sprite eat, water, skull;
                private Transform parent;
                private Vector3 needPopupPoint;


                public void Init()
                {
                    parent = needPopup.parent;
                    needPopupPoint = needPopup.localPosition;
                
                    needPopup.transform.parent = null;
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
                    needPopup.gameObject.SetActive(state);
                    if (state)
                    {
                        if (hunger < 40 && thirsty < 40)
                        {
                            image.sprite = skull;
                            return;
                        }

                        if (hunger < 40)
                        {
                            image.sprite = eat;
                            return;
                        }

                        if (thirsty < 40)
                        {
                            image.sprite = water;
                            return;
                        }
                    }
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
            
 
            private WeatherService.WeatherModifiers currentModifiers;
            private GameDataObject gameDataObject;
            private SelectionService selectionService;
            private Character selfCharacter;

            public bool IsDead => isDead;
            public float Health => health;
            public float Hunger => hunger;
            public float Thirsty => thirsty;

            public void Init(Character character, WeatherService weatherService, GameDataObject gameDataObject, SelectionService selectionService)
            {
                selfCharacter = character;
                this.selectionService = selectionService;
                this.gameDataObject = gameDataObject;
                var parametersData = character.Parameters;
                health = parametersData.Health;
                hunger = parametersData.Hunger;
                thirsty = parametersData.Thirsty;

                currentModifiers = weatherService.CurrentModifiers;

                popUp.Init();
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

                popUp.ShowNeedPopup(Hunger < 40 || Thirsty < 40, Thirsty, Hunger);
            }

            private void CalculateHealth(float delta, float vitalityModify)
            {
                if (Thirsty > 60 && Hunger > 60)
                {
                    health += delta * (1 + (1f - vitalityModify));
                }

                if (Thirsty <= 0 || Hunger <= 0)
                {
                    health -= (delta * vitalityModify);

                    if (Health <= 0)
                    {
                        popUp.ShowNeedPopup(false, 0, 0);
                        Death();
                    }
                }
            }

            private void ClampValues()
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
        }
    }
}