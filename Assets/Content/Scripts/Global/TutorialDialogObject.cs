using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.Global
{
    [CreateAssetMenu(menuName = "Create TutorialDialogObject", fileName = "TutorialDialogObject", order = 0)]
    public class TutorialDialogObject : ScriptableObject
    {
        [TextArea]
        [SerializeField] private List<string> phrases;

        public List<string> Phrases => phrases;
    }
}
