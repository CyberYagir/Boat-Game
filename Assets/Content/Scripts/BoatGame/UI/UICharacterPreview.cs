using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UICharacterPreview : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter character;
        private Character targetCharacter;

        public void Init(GameDataObject gameDataObject)
        {
            character.Init(new Character(), gameDataObject, null, null, null, null, null);
        }

        private void OnEquipmentChange()
        {
            character.gameObject.ChangeLayerWithChilds(LayerMask.NameToLayer("Overlay"));
        }

        public void UpdateCharacterVisuals(Character ch)
        {
            targetCharacter = ch;
            character.Character.Equipment.OnEquipmentChange -= OnEquipmentChange;
            character.ChangeCharacter(ch);
            character.Character.Equipment.OnEquipmentChange += OnEquipmentChange;
            OnEquipmentChange();
        }

        private void OnDestroy()
        {
            if (targetCharacter != null)
            {
                targetCharacter.Equipment.OnEquipmentChange -= OnEquipmentChange;
            }
        }
    }
}
