using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Mobs;
using UnityEngine;

namespace Content.Scripts.Mobs.MobSnake
{
    public class Mob_Snake : SpawnedMobAggressive
    {
        private int groundMask = ~0;

        public override void Init(BotSpawner botSpawner)
        {
            base.Init(botSpawner);
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

            if (IsAttacked) return;

            Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundMask);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lastPoint - transform.position), 10f * TimeService.DeltaTime);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);


            var pos = Vector3.MoveTowards(transform.position, lastPoint, speed * TimeService.DeltaTime);
            pos.y = hit.point.y;

            transform.position = pos;
        }
    }
}
