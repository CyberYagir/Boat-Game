using Content.Scripts.BoatGame.UI;
using Content.Scripts.DungeonGame.Services;

namespace Content.Scripts.DungeonGame.UI
{
    public class UIExitDungeonButton : UIBigButtonBase
    {
        private UIMessageBoxManager messageBoxManager;
        private DungeonUIService uiService;

        public void Init(UIMessageBoxManager messageBoxManager, DungeonUIService uiService)
        {
            this.uiService = uiService;
            this.messageBoxManager = messageBoxManager;
        }

        public override void OnButtonUp()
        {
            base.OnButtonUp();
            
            messageBoxManager.ShowMessageBox("Are you sure you want to exit the dungeon? Part of the dungeon progress will be lost.", OkAction);
        }

        private void OkAction()
        {
            uiService.ExitDungeon();
        }
    }
}
