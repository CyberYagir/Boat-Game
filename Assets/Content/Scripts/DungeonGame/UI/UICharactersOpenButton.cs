using Content.Scripts.BoatGame.UI;
using Content.Scripts.DungeonGame.Services;

namespace Content.Scripts.DungeonGame.UI
{
    public class UICharactersOpenButton : UIBigButtonBase
    {
        private DungeonUIService uiService;

        public void Init(DungeonUIService uiService)
        {
            this.uiService = uiService;
        }

        public override void OnButtonUp()
        {
            base.OnButtonUp();

            uiService.OpenCharactersPreview();
        }
    }
}
