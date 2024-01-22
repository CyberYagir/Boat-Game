using System.Collections.Generic;
using Content.Scripts.BoatGame.PlayerActions;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public interface ISelectable
    {
        public List<PlayerAction> PlayerActions { get; }
        Transform Transform { get; }
    }
}