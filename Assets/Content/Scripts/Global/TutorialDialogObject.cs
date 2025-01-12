using System.Collections.Generic;
using Content.Scripts.IslandGame.Natives;
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
            Shaman,
            MedivalShaman
        }

        [SerializeField] private ECharacter character;
        [TextArea]
        [SerializeField] private List<string> phrases;

        public List<string> Phrases => phrases;
        public ECharacter Character => character;

        public void SetCharacter(ECharacter shaman)
        {
            character = shaman;
        }
    }
}
