using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.DungeonGame.UI
{
    public class UICharacterPreviewItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private RawImage rawImage;
        private UICharacterPreview preview;
        private PlayerCharacter character;
        private UISelectCharacterWindow selectCharacterWindow;

        public void Init(UICharacterPreview preview, string name, PlayerCharacter character, UISelectCharacterWindow selectCharacterWindow)
        {
            this.selectCharacterWindow = selectCharacterWindow;
            this.character = character;
            this.preview = preview;

            text.text = name;
            rawImage.texture = preview.RenderTexture;
        }
        
        public void OnClick()
        {
            selectCharacterWindow.SelectCharacter(character);
        }
    }
}
