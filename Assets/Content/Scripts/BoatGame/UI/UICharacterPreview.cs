using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UICharacterPreview : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter character;

        public void Init(GameDataObject gameDataObject)
        {
            character.Init(new Character(), gameDataObject, null, null, null, null);
        }

        public void UpdateCharacterVisuals(Character ch)
        {
            character.ChangeCharacter(ch);
        }
    }
}
