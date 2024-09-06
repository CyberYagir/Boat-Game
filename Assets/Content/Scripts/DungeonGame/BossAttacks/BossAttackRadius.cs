using UnityEngine;

namespace Content.Scripts.DungeonGame.BossAttacks
{
    public class BossAttackRadius : BossAttackBase
    {
        [SerializeField] private Transform[] points;
        [SerializeField] private float radius;
        


        private void OnDrawGizmos()
        {
            for (int i = 0; i < points.Length; i++)
            {
                Gizmos.DrawWireSphere(points[i].position, radius);
            }
        }
    }
}
