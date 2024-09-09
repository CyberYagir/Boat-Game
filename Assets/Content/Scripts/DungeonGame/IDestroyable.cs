using System;
using Content.Scripts.Mobs;
using UnityEngine;

namespace Content.Scripts.DungeonGame
{
    public interface IDestroyable
    {
        
        string UID { get; }
        public event Action OnOpen;
        Transform transform { get; }
        DropTableObject DropTable { get; }
        float ActivationDistance { get; }
        int DropsCount { get; }
        void Demolish(Vector3 pos);
        bool IsCanDemolish();
    }
}