using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Mobs;
using Content.Scripts.Mobs.Mob;
using Content.Scripts.Mobs.MobCrab;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Mobs.MobSnake
{
    public class SpawnedMobAggressive : SpawnedMob
    {
        [SerializeField, FoldoutGroup("Settings")] private bool aggressionAfterAttack;
        [SerializeField, FoldoutGroup("Data")] private float aggressionRadius;
        [SerializeField, FoldoutGroup("Data")] private float attackRadius;
        [SerializeField, FoldoutGroup("Data")] private float attackDamage;
        
        [SerializeField, ReadOnly] protected Transform attackedTransform;
        protected IDamagable attackedDamagable;


        public float AttackRadius => attackRadius;

        public Transform AttackedTransform => attackedTransform;
        
        private CharacterService characterService;
        private TickService tickService;

        [Inject]
        private void Construct(CharacterService characterService, TickService tickService)
        {
            this.tickService = tickService;
            this.characterService = characterService;
            tickService.OnTick += OnTick;
            
            OnDeath += OnOnDeath;
        }

        private void OnOnDeath(DamageObject obj)
        {
            tickService.OnTick -= OnTick;
        }

        public override void OnOnDamage(float obj)
        {
            Animations.TriggerDamageHit();
            aggressionAfterAttack = false;
        }


        private void OnTick(float obj)
        {
            if (aggressionAfterAttack) return;
            attackedTransform = null;
            attackedDamagable = null;
            for (int i = 0; i < characterService.SpawnedCharacters.Count; i++)
            {
                var dist = Vector3.Distance(transform.position, characterService.SpawnedCharacters[i].transform.position);
                if (dist < aggressionRadius)
                {
                    attackedDamagable = characterService.SpawnedCharacters[i];
                    attackedTransform = characterService.SpawnedCharacters[i].transform;
                    break;
                }
            }

            if (attackedDamagable != null)
            {
                if (StateMachine.CurrentStateType != EMobsState.Attack)
                {
                    StateMachine.StartAction(EMobsState.Attack);
                }
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, aggressionRadius);
            
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }

        public void AttackTarget()
        {
            if (attackedDamagable != null)
            {
                attackedDamagable.Damage(attackDamage);
            }
        }
    }
}