using Content.Scripts.BoatGame.UI;

namespace Content.Scripts.DungeonGame.UI
{
    public class UIExitDungeonButton : UIBigButtonBase
    {
        private UIMessageBoxManager messageBoxManager;

        public void Init(UIMessageBoxManager messageBoxManager)
        {
            this.messageBoxManager = messageBoxManager;
        }

        public override void OnButtonUp()
        {
            base.OnButtonUp();
            
            messageBoxManager.ShowMessageBox("Are you sure you want to exit the dungeon? Part of the dungeon progress will be lost.", OkAction);
        }

        private void OkAction()
        {
            print("exit");
        }
    }
}
