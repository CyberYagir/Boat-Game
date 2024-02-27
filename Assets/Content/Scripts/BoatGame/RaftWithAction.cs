using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class RaftWithAction : RaftBase
    {
        [SerializeField] private ActionsHolder actionsHolder;

        public void InitActions(SelectionService selectionService, GameDataObject gameDataObject)
        {
            actionsHolder.Construct(selectionService,gameDataObject);
        } 
    }
}
