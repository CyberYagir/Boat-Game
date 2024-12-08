using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts
{
    [CreateAssetMenu(menuName = "Create ItemsStrengthCalculatorSO", fileName = "ItemsStrengthCalculatorSO", order = 0)] 
    public class ItemsStrengthCalculatorSO : ScriptableObject
    {
        [SerializeField] private AnimationCurve strengthCurve;
        [SerializeField] private float maxStrength;
        private List<ItemObject> equipedItems = new List<ItemObject>();

        public float GetCharacterStrength(PlayerCharacter playerCharacter)
        {
            equipedItems.Clear();
            
            var gameDataObject = playerCharacter.GameData;
            var character = playerCharacter.Character;
            var weapon = gameDataObject.GetItem(character.Equipment.GetEquipment(EEquipmentType.Weapon));
            var body =   gameDataObject.GetItem(character.Equipment.GetEquipment(EEquipmentType.Armor));
            var helmet = gameDataObject.GetItem(character.Equipment.GetEquipment(EEquipmentType.Helmet));


            float defence = 0;

            equipedItems.Add(weapon);
            equipedItems.Add(body);
            equipedItems.Add(helmet);

            equipedItems.RemoveAll(x => x == null);
            
            for (var i = 0; i < equipedItems.Count; i++)
            {
                defence += equipedItems[i].ParametersData.Defence;
            }

            var damage = playerCharacter.ParametersCalculator.Damage - playerCharacter.ParametersCalculator.BaseDamage;
            Debug.Log(Mathf.Sqrt(damage + (defence * 100f)));
            
            var complex = Mathf.Sqrt(damage + (defence * 100f)) / maxStrength;

            return strengthCurve.Evaluate(complex);
        }

        public float GeItemStrength(ItemObject itemObject)
        {
            return strengthCurve.Evaluate(
                Mathf.Sqrt(itemObject.ParametersData.Damage + (itemObject.ParametersData.Defence * 100f)) /
                maxStrength);
        }
    }
}
