using System;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using UnityEngine;

namespace Content.Scripts.DungeonGame
{
    public class Projectile : MonoBehaviour
    {
        private static Collider[] results = new Collider[5];
        
        [SerializeField] private float speed;
        [SerializeField] private float radius;
        [SerializeField] private GameObject particles;
        private int mask;
        private float damage;

        public void Init(float damage)
        {
            this.damage = damage;
            mask = LayerMask.GetMask("Player", "Default", "Obstacle");
            particles.gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            transform.Translate(transform.forward * TimeService.DeltaTime * speed, Space.World);

            var count = Physics.OverlapSphereNonAlloc(transform.position, radius, results, mask);

            for (int i = 0; i < count; i++)
            {
                var character = results[i].GetComponent<DungeonCharacter>();
                if (character)
                {
                    if (character.PlayerCharacter.CurrentState != EStateType.Roll)
                    {
                        character.PlayerCharacter.Damage(damage, gameObject);
                    }
                }
            }

            if (count != 0)
            {
                particles.transform.parent = null;
                particles.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
