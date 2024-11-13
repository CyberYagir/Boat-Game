using System;
using UnityEngine;

namespace Content.Scripts.DungeonGame.BossAttacks
{
    public class BossAttackRadiusNoCenter : BossAttackBase
    {
        [SerializeField] private float notDamageRadius1;
        [SerializeField] private float damageRadius2;
        [SerializeField] private float notDamageRadius3;
        [SerializeField] private float damageRadius4;
        [SerializeField] private Transform center;
        
        
        public override void OnComplete()
        {
            base.OnComplete();

            foreach (var chars in dungeonCharactersService.SpawnedCharacters)
            {
                if (!chars.IsInRoll())
                {
                    if (Vector3.Distance(center.transform.position, chars.PlayerCharacter.transform.position) < notDamageRadius1)
                    {
                        continue;
                    }

                    if (Vector3.Distance(center.transform.position, chars.PlayerCharacter.transform.position) < damageRadius2)
                    {
                        chars.PlayerCharacter.Damage(damage, dungeonMob.gameObject);
                        continue;
                    }

                    if (Vector3.Distance(center.transform.position, chars.PlayerCharacter.transform.position) < notDamageRadius3)
                    {
                        continue;
                    }

                    if (Vector3.Distance(center.transform.position, chars.PlayerCharacter.transform.position) < damageRadius4)
                    {
                        chars.PlayerCharacter.Damage(damage, dungeonMob.gameObject);
                        continue;
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(center.transform.position, damageRadius4);
            Gizmos.DrawSphere(center.transform.position, damageRadius2);
            
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(center.transform.position, notDamageRadius1);
            Gizmos.DrawSphere(center.transform.position, notDamageRadius3);
        }
    }
}
