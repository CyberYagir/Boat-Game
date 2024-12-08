using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UICharacterWindowStrength : MonoBehaviour
    {
        [SerializeField] private ItemsStrengthCalculatorSO strengthCalculator;
        [SerializeField] private TMP_Text text;

        public void Redraw(PlayerCharacter character)
        {
            text.text = "<sprite=7>" + strengthCalculator.GetCharacterStrength(character).ToString("F1");
        }
    }
}
