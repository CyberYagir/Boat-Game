using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIBarClockwise : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void UpdateBar(float val)
        {
            image.fillAmount = val;
        }
    }
}
