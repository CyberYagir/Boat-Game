using UnityEngine;

namespace Content.Scripts.DungeonGame.BossAttacks
{
    public class BossAttackRadius : BossAttackBase
    {
        [SerializeField] private Transform[] points;
        [SerializeField] private float radius;

        public override void OnComplete()
        {
            base.OnComplete();

            foreach (var chars in dungeonCharactersService.SpawnedCharacters)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    if (Vector3.Distance(chars.transform.position, points[i].position) <= radius)
                    {
                        chars.PlayerCharacter.Damage(damage, dungeonMob.gameObject);
                    }
                }
            }
        }


        private void OnDrawGizmos()
        {
            for (int i = 0; i < points.Length; i++)
            {
                Gizmos.DrawWireSphere(points[i].position, radius);
            }
        }
    }
}
