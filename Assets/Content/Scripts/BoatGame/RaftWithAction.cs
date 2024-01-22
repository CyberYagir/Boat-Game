using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class RaftWithAction : RaftBase
    {
        [SerializeField] private ActionsHolder actionsHolder;

        public void InitActions(SelectionService selectionService)
        {
            actionsHolder.Construct(selectionService);
        } 
    }
}
