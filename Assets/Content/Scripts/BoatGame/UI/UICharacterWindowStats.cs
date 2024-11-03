using Content.Scripts.ItemsSystem;
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
        [SerializeField] private StatRow speedRow;


        public void Redraw(PlayerCharacter character)
        {
            var dmg = character.ParametersCalculator.Damage;
            var dfc = 1f - character.ParametersCalculator.Defence;
            var speed = character.ParametersCalculator.Speed;
            attackRow.Set(dmg.ToString("F1"), dmg >= 5);
            defenceRow.Set((dfc * 100f).ToString("F1") + "%", dfc >= 0.01f);
            speedRow.Set((speed).ToString("F1") + " u/s", speed >= 2f);
        }
        
        public void Redraw(ItemObject item)
        {
            var dmg = item.ParametersData.Damage;
            var dfc = item.ParametersData.Defence;
            attackRow.Set(dmg.ToString("F1"), dmg > 0);
            defenceRow.Set((dfc * 100) + "%", dfc >= 0.01f);
            speedRow.Set("", false);
        }
    }
}
