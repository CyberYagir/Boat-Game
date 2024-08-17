using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.SkillsSystem;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UISkillsSubWindow : MonoBehaviour
    {
        [SerializeField] private UISkillSubWindowItem skillItem;
        [SerializeField] private List<UISkillSubWindowItem> skillItems = new List<UISkillSubWindowItem>();
        [SerializeField] private TMP_Text scoresText;
        [SerializeField] private GameObject indicator;
        
        private ISelectionService selectionService;
        private Character character;
        private UIMessageBoxManager uiMessageBoxManager;
        private GameDataObject gameData;

        private SkillObject lastSelectedSkill;
        
        public void Init(GameDataObject gameData, ISelectionService selectionService, UIMessageBoxManager uiMessageBoxManager)
        {
            this.gameData = gameData;
            this.uiMessageBoxManager = uiMessageBoxManager;
            this.selectionService = selectionService;
            skillItem.gameObject.SetActive(true);
            foreach (var skillObject in gameData.SkillsList)
            {
                Instantiate(skillItem, skillItem.transform.parent)
                    .With(x => x.Init(skillObject, this))
                    .With(x => skillItems.Add(x));
            }
            skillItem.gameObject.SetActive(false);
        }

        public void Redraw(Character character)
        {
            this.character = character;
            UpdateItems();
        }

        public void UpdateItems()
        {
            if (character == null) return;
            for (int i = 0; i < skillItems.Count; i++)
            {
                skillItems[i].UpdateData(character);
            }
            scoresText.text = "Skill points: " + character.SkillData.Scores;

            indicator.SetActive(character.SkillData.Scores != 0);
        }

        public void AddSkill(string skillObjectSkillID)
        {
            lastSelectedSkill = gameData.SkillsList.Find(x => x.SkillID == skillObjectSkillID); 
            uiMessageBoxManager.ShowMessageBox($"Do you really want to spend a leveling point on improving the {lastSelectedSkill.SkillName} skill?", AddSkillToCharacter);
        }

        private void AddSkillToCharacter()
        {
            character.AddSkillValue(lastSelectedSkill.SkillID, 1);
            character.SkillData.RemoveScore();
            
            Redraw(character);
        }
    }
}
