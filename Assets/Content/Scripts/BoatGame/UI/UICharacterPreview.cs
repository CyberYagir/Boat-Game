using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.UI
{
    public class UICharacterPreview : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter character;
        private Character targetCharacter;

        [Inject]
        private void Construct(GameDataObject gameDataObject, INavMeshProvider navMeshService)
        {
            character.Init(new Character(), gameDataObject, null, null, null, null, null, navMeshService, null);
        }
        private void OnEquipmentChange()
        {
            character.gameObject.ChangeLayerWithChilds(LayerMask.NameToLayer("Overlay"));
        }

        public void UpdateCharacterVisuals(Character ch)
        {
            if (targetCharacter != null){
                targetCharacter.Equipment.OnEquipmentChange -= OnEquipmentChange;
            }
            targetCharacter = ch;
            character.ChangeCharacter(ch);
            targetCharacter.Equipment.OnEquipmentChange += OnEquipmentChange;
            
            
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
