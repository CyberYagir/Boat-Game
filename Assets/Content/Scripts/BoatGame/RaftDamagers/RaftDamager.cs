using Content.Scripts.BoatGame.Services;
using UnityEngine;

namespace Content.Scripts.BoatGame.RaftDamagers
{
    public abstract class RaftDamager : MonoBehaviour
    {
        private RaftBase targetRaft;
        private RaftDamagerService raftDamagerService;

        public RaftBase TargetRaft => targetRaft;

        public RaftDamagerService DamagerService => raftDamagerService;

        public virtual void Init(RaftBase targetRaft, RaftDamagerService raftDamagerService)
        {
            this.raftDamagerService = raftDamagerService;
            this.targetRaft = targetRaft;
        }
    }
}