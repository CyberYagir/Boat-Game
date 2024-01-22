using Content.Scripts.BoatGame.Services;
using Content.Scripts.SkillsSystem;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UISkillSubWindowItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text skillNameText;
        [SerializeField] private TMP_Text currentSkillText;
        [SerializeField] private GameObject button;
        private SkillObject skillObject;
        private UISkillsSubWindow window;

        public void Init(SkillObject skillObject, UISkillsSubWindow window)
        {
            this.window = window;
            this.skillObject = skillObject;
            skillNameText.text = skillObject.SkillName;
        }


        public void UpdateData(Character chr)
        {
            currentSkillText.text = chr.GetSkillValue(skillObject.SkillID).ToString();
            button.SetActive(chr.SkillData.Scores != 0);
        }

        public void AddSkill()
        {
            window.AddSkill(skillObject.SkillID);
        }
    }
}
