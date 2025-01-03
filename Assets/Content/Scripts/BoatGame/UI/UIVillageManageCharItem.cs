using Content.Scripts.BoatGame.Services;
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
        private SlaveCreatedCharacterInfo info;

        public DisplayCharacter Character => displayCharacter;


        public void Init(DisplayCharacter displayCharacter, SlaveCreatedCharacterInfo info, UIVillageManageSubWindow window, bool isSelected)
        {
            this.info = info;
            this.window = window;
            this.displayCharacter = displayCharacter;
            rawImage.texture = displayCharacter.Display.RenderTexture;
            text.text = info.Character.Name;
            selection.gameObject.SetActive(isSelected);
        }


        public void Select()
        {
            window.SelectCharacter(info);
        }
    }
}
