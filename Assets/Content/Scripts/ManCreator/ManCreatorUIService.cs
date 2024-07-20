using System.Collections.Generic;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using Content.Scripts.Loading;
using Content.Scripts.SkillsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Content.Scripts.ManCreator
{
    public class ManCreatorUIService : MonoBehaviour
    {
        [System.Serializable]
        public class NamePopup
        {
            [SerializeField] private RectTransform holder;
            [SerializeField] private TMP_Text nameText;

            public void SetHolderPoint(Vector3 getNamePoint)
            {
                holder.transform.position = getNamePoint;
            }

            public void SetName(string getCharacterName)
            {
                nameText.text = getCharacterName;
            }
        }
        
        [System.Serializable]
        public class SkillsDrawer
        {
            [SerializeField] private UISkillItem item;
            private List<UISkillItem> items = new List<UISkillItem>();
            
            public void Redraw(List<SkillObject> skillObjects, CharacterCustomizationService currentCharacter)
            {
                foreach (var it in items)
                {
                    Destroy(it.gameObject);
                }
                item.gameObject.SetActive(true);
                for (int i = 0; i < skillObjects.Count; i++)
                {
                    var spawned = Instantiate(item, item.transform.parent);
                    spawned.Init(skillObjects[i], currentCharacter);
                    items.Add(spawned);
                }
                item.gameObject.SetActive(false);
            }

            public void UpdateText()
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].UpdateText();
                }
            }
        }
        
        
        
        [SerializeField] private NamePopup namePopup;
        [SerializeField] private SkillsDrawer skillsDrawer;
        [SerializeField] private UIChangeNameWindow changeNameWindow;
        [SerializeField] private Image skinColorIndicator;
        [SerializeField] private TMP_Text scoresText;
        [SerializeField] private Button applyButton;
        [SerializeField] private Button skillDiceButton;
        private CharacterCustomizationService characterService;
        private GameDataObject gameDataObject;
        private ScenesService scenesService;


        [Inject]
        public void Construct(CharacterCustomizationService characterService, GameDataObject gameDataObject, ScenesService scenesService)
        {
            this.scenesService = scenesService;
            this.gameDataObject = gameDataObject;
            this.characterService = characterService;
            
            UpdateName();
            
            applyButton.interactable = false;
            skillDiceButton.interactable = true;
            
            characterService.OnNameChanged += UpdateName;
            characterService.OnChangeSkillsCount += OnScoresChanged;
            
            skillsDrawer.Redraw(gameDataObject.SkillsList, characterService);
            changeNameWindow.Init(this);
        }
        
        public void ShowChangeNameWindow(){
            changeNameWindow.gameObject.SetActive(true);
            changeNameWindow.ShowWindow();
        }

        private void OnScoresChanged(int value)
        {
            scoresText.text = "Free scores: " + value;

            applyButton.interactable = value == 0;
            skillDiceButton.interactable = value != 0;
        }

        private void UpdateName()
        {
            namePopup.SetName(characterService.GetCharacterName());
        }
    

        private void Update()
        {
            namePopup.SetHolderPoint(characterService.GetNamePoint());
        }


        public void ChangeName()
        {
            characterService.ChangeName();
        }

        public void ChangeName(string str)
        {
            characterService.ChangeName(str);
        }

        public void NextSkinColor()
        {
            skinColorIndicator.color = gameDataObject.SkinColors[characterService.NextSkinColor()].color;
        }

        public void NextHat()
        {
            characterService.NextHat();
        }
        public void PrevHat()
        {
            characterService.PrevHat();
        }

        public void AddRandomSkill()
        {
            characterService.AddSkill(gameDataObject.SkillsList.GetRandomItem(), 1);
            skillsDrawer.UpdateText();
        }


        public void Apply()
        {
            scenesService.FadeScene(ESceneName.BoatGame, delegate
            {
                characterService.ApplyCharacter();
            });
        }
    }
}
