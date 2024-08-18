using Content.Scripts.BoatGame.Characters;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Mobs.States
{
    public class DgMobActionBase : StateAction<DungeonMob>
    {
        protected void RotateToTarget()
        {
            var pos = Machine.AttackedPlayer.transform.position - Machine.AttackedPlayer.transform.forward;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(pos.x, Machine.transform.position.y, pos.z) - Machine.transform.position);
            Machine.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 30 * Time.deltaTime);
        }

        protected bool IsCanAttack(float attackDistance)
        {
            return Vector3.Distance(transform.position, Machine.AttackedPlayer.transform.position) <= attackDistance;
        }
    }
}