using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UICharacterWindowStats : MonoBehaviour
    {
        [System.Serializable]
        public class StatRow
        {
            [SerializeField] private GameObject row;
            [SerializeField] private TMP_Text text;

            public void Set(string text, bool active)
            {
                row.gameObject.SetActive(active);
                this.text.text = text;
            }
        }

        [SerializeField] private StatRow attackRow;
        [SerializeField] private StatRow defenceRow;


        public void Redraw(PlayerCharacter character)
        {
            var dmg = character.ParametersCalculator.Damage;
            var dfc = 1f - character.ParametersCalculator.Defence;
            attackRow.Set(dmg.ToString("F1"), dmg >= 5);
            defenceRow.Set((dfc * 100f).ToString("F1") + "%", dfc >= 0.01f);
        }
    }
}
