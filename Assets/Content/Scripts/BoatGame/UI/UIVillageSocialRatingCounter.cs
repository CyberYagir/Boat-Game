using System.Collections.Generic;
using DG.DemiLib;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageSocialRatingCounter : MonoBehaviour
    {
        [System.Serializable]
        public class EmotionRange
        {
            [SerializeField] private Range range;
            [SerializeField, PreviewField] private Sprite sprite;

            public Sprite Sprite => sprite;

            public Range Range => range;
        }

        [SerializeField, TableList] private List<EmotionRange> list;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image icon;

        public void Redraw(int value)
        {
            text.text = "Reputation: " + value.ToString("0000");

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Range.IsInRange(value))
                {
                    icon.sprite = list[i].Sprite;
                    break;
                }
            }
        }
    }
}
