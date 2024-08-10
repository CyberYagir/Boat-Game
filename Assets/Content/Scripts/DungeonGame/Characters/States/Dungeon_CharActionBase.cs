using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.DungeonGame.Services;
using Pathfinding.RVO;

namespace Content.Scripts.DungeonGame.Characters.States
{
    public class Dungeon_CharActionBase : CharActionBase
    {
        private DungeonSelectionService dungeonSelectionService;
        private DungeonCharacter character;

        public DungeonSelectionService SelectionService => dungeonSelectionService;
        public DungeonCharacter DungeonCharacter => character;

        public override void StartState()
        {
            base.StartState();
            if (SelectionService == null)
            {
                character = Machine.Get<DungeonCharacter>();
                dungeonSelectionService = character.SelectionService;
            }
        }
    }
}