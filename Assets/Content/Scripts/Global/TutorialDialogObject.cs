using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Content.Scripts.Global
{
    [CreateAssetMenu(menuName = "Create TutorialDialogObject", fileName = "TutorialDialogObject", order = 0)]
    public class TutorialDialogObject : ScriptableObject
    {
        public enum ECharacter
        {
            Pirate,
            Shaman
        }

        [SerializeField] private ECharacter character;
        [TextArea]
        [SerializeField] private List<string> phrases;

        public List<string> Phrases => phrases;
        public ECharacter Character => character;
    }
}
