using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Mobs;
using UnityEngine;

namespace Content.Scripts.Mobs.MobSnake
{
    public class Mob_Snake : SpawnedMobAggressive
    {
        private int groundMask = ~0;

        public override void Init(BotSpawner botSpawner, bool initStateMachine = true)
        {
            base.Init(botSpawner, initStateMachine);
            groundMask = LayerMask.GetMask("Default", "Terrain");
        }

        public override void OnOnAttackedStart()
        {
            base.OnOnAttackedStart();
            Animations.StopMove();
        }

        public override void MoveToPoint(Vector3 lastPoint)
        {
            base.MoveToPoint(lastPoint);
            BaseMovement(lastPoint, groundMask);
        }
    }
}
