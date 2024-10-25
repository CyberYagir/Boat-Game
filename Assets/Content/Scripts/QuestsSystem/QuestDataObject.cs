using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.QuestsSystem
{
    [CreateAssetMenu(menuName = "Create QuestDataObject", fileName = "QuestDataObject", order = 0)]
    public class QuestDataObject : ScriptableObject
    {
        [SerializeField] private string questName;
        [SerializeField, PreviewField] private Sprite icon;
        [SerializeField] private int maxValue;
        [SerializeField, TextArea] private string description;
    }
}
