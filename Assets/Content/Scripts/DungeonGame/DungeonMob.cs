using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.DungeonGame.Services;
using Content.Scripts.Mobs.Mob;
using Content.Scripts.Mobs.MobCrab;
using Pathfinding;
using Pathfinding.RVO;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Content.Scripts.DungeonGame
{
    public class DungeonMob : DamageObject
    {
        [System.Serializable]
        public class AggressionModule
        {
            [SerializeField] private float agrDistance;
            [SerializeField] private float unAgrDistance;
            private ICharacterService characterService;
            [SerializeField, ReadOnly] private bool isAgred;
            private Transform transform;
            private PlayerCharacter playerCharacter;

            public event Action<bool> OnAgressionChanged;

            public PlayerCharacter PlayerCharacter => playerCharacter;

            public void Init(ICharacterService characterService, Transform transform)
            {
                this.transform = transform;
                this.characterService = characterService;
            }

            public void Update()
            {
                var players = characterService.GetSpawnedCharacters();
                if (players.Count == 0) return;
                
                if (isAgred)
                {
                    var minDistance = players.Min(x => Vector3.Distance(x.transform.position, transform.position));
                    playerCharacter = players.Find(x => Vector3.Distance(x.transform.position, transform.position) <= minDistance);

                    if (Vector3.Distance(transform.position.RemoveY(), playerCharacter.transform.position.RemoveY()) > unAgrDistance)
                    {
                        isAgred = false;
                        playerCharacter = null;
                        OnAgressionChanged?.Invoke(false);
                        return;
                    }
                }
                else
                {

                    for (int i = 0; i < players.Count; i++)
                    {
                        if (Vector3.Distance(transform.position.RemoveY(), players[i].transform.position.RemoveY()) < agrDistance)
                        {
                            isAgred = true;
                            playerCharacter = players[i];
                            OnAgressionChanged?.Invoke(true);
                            return;
                        }
                    }
                }
            }


            public void Gizmo(Transform transform)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, agrDistance);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, unAgrDistance);
            }
        }

        [System.Serializable]
        public class AIModule
        {
            private INavAgentProvider agent;

            public void Init(Transform transform) => agent = transform.GetComponent<INavAgentProvider>();
            public void MoveToPoint(Vector3 pos) => agent.SetDestination(pos);

            public bool IsArrived() => agent.IsArrived();

            public void Stop()
            {
                agent.Disable();
            }

            public bool IsMoving() => agent.Velocity.magnitude >= 0.1f;
        }


        [SerializeField] private DungeonEnemyStateMachine stateMachine;
        [SerializeField] private MobAnimator mobAnimator;
        [SerializeField] private AggressionModule aggressionModule;
        [SerializeField] private RVOController rvoController;
        private DungeonEnemiesService enemiesService;
        private AIModule aiModule;

        public MobAnimator MobAnimator => mobAnimator;

        public AIModule AIManager => aiModule;
        public PlayerCharacter AttackedPlayer => aggressionModule.PlayerCharacter;

        // [SerializeField] [SerializeReference] [ListDrawerSettings(DefaultExpandedState = true, ListElementLabelName = "Name", ShowPaging = false, ShowIndexLabels = true)]
        // private List<BaseModule> modules;

        [Inject]
        private void Construct(DungeonCharactersService characterService, DungeonEnemiesService enemiesService)
        {
            this.enemiesService = enemiesService;
            SetHealth(MaxHealth * characterService.SpawnedCharacters.Count);
            aiModule = new AIModule()
                .With(x => x.Init(transform));
            aggressionModule.Init(characterService, transform);
            stateMachine.Init(this);
            aggressionModule.OnAgressionChanged += OnAggressionChange;
            enemiesService.AddMob(this);
            
            
            RVOSimulator.OnInited += () => rvoController.enabled = true;
        }

        public override void Damage(float dmg, GameObject sender)
        {
            base.Damage(dmg, sender);
            mobAnimator.TriggerDamageHit();
        }

        public override void Death()
        {
            base.Death();
            stateMachine.StartAction(EMobsState.Idle);
            stateMachine.enabled = false;
            MobAnimator.TriggerDeath();
            rvoController.enabled = false;
            
            enemiesService.RemoveMob(this);
            aiModule.Stop();
            gameObject.ChangeLayerWithChilds(LayerMask.NameToLayer("Default"));
        }

        private void OnAggressionChange(bool state)
        {
            if (state)
            {
                stateMachine.StartAction(EMobsState.Attack);
            }
        }

        public void Update()
        {
            aggressionModule.Update();
        }

        private void OnDrawGizmosSelected()
        {
            aggressionModule.Gizmo(transform);
        }

        public void MoveToTargetPlayer()
        {
            aiModule.MoveToPoint(aggressionModule.PlayerCharacter.transform.position);
        }

        public void MoveToRandomPoint()
        {
            aiModule.MoveToPoint(transform.position + UnityEngine.Random.insideUnitSphere * Random.Range(5, 15));
        }
    }
}
