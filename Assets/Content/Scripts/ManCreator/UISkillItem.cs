using Content.Scripts.BoatGame.Services;
using Content.Scripts.SkillsSystem;
using TMPro;
using UnityEngine;

namespace Content.Scripts.ManCreator
{
    public class UISkillItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text skillNameText;
        [SerializeField] private TMP_Text valueText;
        
        private SkillObject skill;
        private CharacterCustomizationService characterCustomizationService;

        public void Init(SkillObject skill, CharacterCustomizationService characterCustomizationService)
        {
            this.characterCustomizationService = characterCustomizationService;
            this.skill = skill;

            skillNameText.text = skill.SkillName;
            UpdateText();
        }

        public void UpdateText()
        {
            valueText.text = characterCustomizationService.GetCharacter().GetSkillValue(skill.SkillID).ToString();
        }

        public void AddSkill()
        {
            characterCustomizationService.AddSkill(skill, 1);
            UpdateText();
        }
        
        public void RemoveSkill()
        {
            characterCustomizationService.AddSkill(skill, -1);
            UpdateText();
        }
    }
}
