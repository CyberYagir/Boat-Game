using System;
using Content.Scripts.Global;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.SkillsSystem
{
    [CreateAssetMenu(menuName = "Scriptable/SkillObject", fileName = "SkillObject", order = 0)]
    public class SkillObject : ScriptableObject
    {
        [SerializeField] private string skillName;
        [SerializeField] private Sprite skillIcon;
        [SerializeField] private TooltipDataObject tooltipData;

        [SerializeField, ReadOnly] private string skillID;


        public string SkillID => skillID;

        public Sprite SkillIcon => skillIcon;

        public string SkillName => skillName;

        public TooltipDataObject TooltipData => tooltipData;


#if UNITY_EDITOR
        [Button]
        private void GenerateID()
        {
            skillID = Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
