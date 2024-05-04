using UnityEngine;

namespace Content.Scripts.Mobs
{
    public class Mob_Peacock : Mob_Peaceful
    {
        public override void MoveToPoint(Vector3 lastPoint)
        {
            base.MoveToPoint(lastPoint);
            BaseMovement(lastPoint, groundMask);
        }
    }
}