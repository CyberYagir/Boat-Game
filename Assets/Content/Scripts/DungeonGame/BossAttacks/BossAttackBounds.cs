using UnityEngine;

namespace Content.Scripts.DungeonGame.BossAttacks
{
    public class BossAttackBounds : BossAttackBase
    {
        [SerializeField] private BoxCollider boxCollider;
        
        public override void OnComplete()
        {
            base.OnComplete();

            foreach (var chars in dungeonCharactersService.SpawnedCharacters)
            {
                if (IsInside(boxCollider, chars.PlayerCharacter.transform.position))
                {
                    if (!chars.IsInRoll())
                    {
                        chars.PlayerCharacter.Damage(damage, dungeonMob.gameObject);
                    }
                }
            }
        }
        
        public static bool IsInside ( Collider c , Vector3 point )
        {
            Vector3 closest = c.ClosestPoint(point);
            // Because closest=point if point is inside - not clear from docs I feel
            return closest == point;
        }
    }
}
