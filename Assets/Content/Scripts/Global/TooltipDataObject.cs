using UnityEngine;

namespace Content.Scripts.Global
{
    [CreateAssetMenu(menuName = "Create TooltipDataObject", fileName = "TooltipDataObject", order = 0)]
    public class TooltipDataObject : ScriptableObject
    {
        [SerializeField, TextArea] private string text;

        public string Text => text;
    }
}
