using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageManageCharItem : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private TMP_Text text;
        [SerializeField] private GameObject selection;
        private DisplayCharacter displayCharacter;
        private UIVillageManageSubWindow window;

        public DisplayCharacter Character => displayCharacter;


        public void Init(DisplayCharacter displayCharacter, UIVillageManageSubWindow window, bool isSelected)
        {
            this.window = window;
            this.displayCharacter = displayCharacter;
            rawImage.texture = displayCharacter.Display.RenderTexture;
            text.text = displayCharacter.Character.Name;
            selection.gameObject.SetActive(isSelected);
        }


        public void Select()
        {
            window.SelectCharacter(displayCharacter);
        }
    }
}
