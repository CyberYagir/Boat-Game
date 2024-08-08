using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.DungeonGame.Services;
using Pathfinding.RVO;

namespace Content.Scripts.DungeonGame.Characters.States
{
    public class Dungeon_CharActionBase : CharActionBase
    {
        private DungeonSelectionService dungeonSelectionService;

        public DungeonSelectionService SelectionService => dungeonSelectionService;

        public override void StartState()
        {
            base.StartState();
            if (SelectionService == null)
            {
                dungeonSelectionService = Machine.Get<DungeonCharacter>().SelectionService;
               
            }
        }
    }
}