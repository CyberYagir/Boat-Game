using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.DungeonGame.Services;

namespace Content.Scripts.DungeonGame.UI
{
    public class UICharactersOpenButton : UIBigButtonBase
    {
        private DungeonUIService uiService;
        private ICharacterService characterService;

        public void Init(DungeonUIService uiService, ICharacterService characterService)
        {
            this.characterService = characterService;
            this.uiService = uiService;
        }

        public override void OnButtonUp()
        {
            base.OnButtonUp();

            if (characterService.GetSpawnedCharacters().Count != 0)
            {
                uiService.OpenCharactersPreview();
            }
        }
    }
}
