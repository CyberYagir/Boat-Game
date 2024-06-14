using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageSocialRatingCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public void Redraw(int value)
        {
            text.text = "Reputation: " + value.ToString("0000");
        }
    }
}
