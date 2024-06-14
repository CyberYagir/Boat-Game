using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Mobs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.Mobs.MobCrab
{
    public class Mob_Crab : Mob_Peaceful
    {

        [SerializeField, FoldoutGroup("Other")] private GameObject crabShell;

        public override void Init(BotSpawner botSpawner, bool initStateMachine = true)
        {
            base.Init(botSpawner, initStateMachine);
            crabShell.gameObject.SetActive(Random.value < 0.35f);
        }

        public override void MoveToPoint(Vector3 lastPoint)
        {
            base.MoveToPoint(lastPoint);
            
            if (IsAttacked) return;
            
            transform.position = Vector3.MoveTowards(transform.position, lastPoint, speed * TimeService.DeltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, speed * TimeService.DeltaTime);
            
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                transform.SetYPosition(hit.point.y);
                var old = transform.rotation;
                transform.LookAt(lastPoint);
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                targetQuaternion = transform.rotation;
                transform.rotation = old;
            }
        }
    }
}
