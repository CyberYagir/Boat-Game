using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.DungeonGame.Services;

namespace Content.Scripts.DungeonGame.UI
{
    public class UIExitDungeonButton : UIBigButtonBase
    {
        private UIMessageBoxManager messageBoxManager;
        private DungeonUIService uiService;
        private ICharacterService characterService;

        public void Init(UIMessageBoxManager messageBoxManager, DungeonUIService uiService, ICharacterService characterService)
        {
            this.characterService = characterService;
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
            if (characterService.GetSpawnedCharacters().Count != 0)
            {
                uiService.ExitDungeon();
            }
        }
    }
}
