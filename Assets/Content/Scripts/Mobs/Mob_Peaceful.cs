using Content.Scripts.IslandGame.Mobs;
using Content.Scripts.Mobs.Mob;
using Content.Scripts.Mobs.MobCrab;
using UnityEngine;

namespace Content.Scripts.Mobs
{
    public class Mob_Peaceful : SpawnedMob
    {
        protected int groundMask;
        public override void Init(BotSpawner botSpawner, bool initStateMachine = true)
        {
            base.Init(botSpawner, initStateMachine);
            groundMask = LayerMask.GetMask("Default", "Terrain");
        }

        public override void OnOnAttackedEnd()
        {
            base.OnOnAttackedEnd();
            ChangeStateTo(EMobsState.Idle);
        }

        public override void OnOnAttackedStart()
        {
            base.OnOnAttackedStart();
            Animations.StopMove();
        }
    }
}