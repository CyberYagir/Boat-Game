using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.PlayerActions
{
    public class ActionsHolder : MonoBehaviour, ISelectable
    {
        public List<PlayerAction> PlayerActions => playerActions;
        public Transform Transform => transform;

        [SerializeField] private List<PlayerAction> playerActions = new List<PlayerAction>();


        [Inject]
        public void Construct(SelectionService selectionService)
        {
            foreach (var ac in playerActions)
            {
                ac.Init(selectionService);
            }
        }
    }
}
