using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.DungeonGame.Characters.States;
using Content.Scripts.DungeonGame.Services;
using Content.Scripts.Global;
using Pathfinding.RVO;
using UnityEngine;

namespace Content.Scripts.DungeonGame
{
    public class DungeonCharacter : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter playerCharacter;
        [SerializeField] private RVOController rvoController;
        [SerializeField] private float attackRange;
        public event Action OnAttackStarts;
        
        
        private DungeonSelectionService dungeonSelectionService;
        private DungeonEnemiesService enemiesService;
        private DamageObject targetEnemy;
        private EStateType preRollState;

        public DungeonSelectionService SelectionService => dungeonSelectionService;
        public PlayerCharacter PlayerCharacter => playerCharacter;
        public float AttackRange => attackRange;

        public DamageObject TargetEnemy => targetEnemy;

        public void Init(
            Character character,
            GameDataObject gameData,
            PrefabSpawnerFabric prefabSpawnerFabric,
            INavMeshProvider navMeshProvider,
            SaveDataObject saveDataObject,
            DungeonSelectionService dungeonSelectionService,
            DungeonEnemiesService enemiesService
        )
        {
            this.enemiesService = enemiesService;
            this.dungeonSelectionService = dungeonSelectionService;

            PlayerCharacter.InitDungeonPlayer(character, gameData, prefabSpawnerFabric, navMeshProvider, saveDataObject);
            PlayerCharacter.AnimationManager.ShowTorch();
            PlayerCharacter.AppearanceDataManager.ShowTorch();
            
            PlayerCharacter.GetCharacterAction<Dungeon_CharActionRoll>().OnRollEnd += OnRollEnd;

            RVOSimulator.OnInited += () => rvoController.enabled = true;
        }
        

        private void OnRollEnd()
        {
            PlayerCharacter.ActiveAction(preRollState);
        }


        private void Update()
        {
            if (PlayerCharacter.CurrentState != EStateType.Roll)
            {
                UpdateAttackRange();

                if (InputService.SpaceDown)
                {
                    StartRoll();
                }
            }
        }

        private void StartRoll()
        {
            preRollState = PlayerCharacter.CurrentState;
            PlayerCharacter.ActiveAction(EStateType.Roll);
        }

        public void MoveToPoint()
        {
            if (playerCharacter.CurrentState != EStateType.Roll)
            {
                PlayerCharacter.ActiveAction(EStateType.MoveTo);
                SetTarget(null);
            }
        }

        public void SetPosition(Vector3 getStartRoomRandomPos)
        {
            transform.position = getStartRoomRandomPos;
        }

        public void UpdateAttackRange()
        {
            if (!InputService.IsLMBPressed)
            {
                if (PlayerCharacter.CurrentState == EStateType.Idle || IsAttackOnDestination())
                {
                    var enemy = enemiesService.GetNearMob(transform.position.RemoveY());
                    if (enemy != null)
                    {
                        if (enemy.transform.position.ToDistance(transform.position) < attackRange)
                        {
                            SetTarget(enemy);
                            PlayerCharacter.ActiveAction(EStateType.Attack);
                            OnAttackStarts?.Invoke();
                        }
                    }
                }
            }
        }

        private bool IsAttackOnDestination()
        {
            if (PlayerCharacter.CurrentState == EStateType.MoveTo)
            {
                var targetMovePoint = playerCharacter.AIMoveManager.NavMeshAgent.Destination;
                var enemy = enemiesService.GetNearMob(targetMovePoint);

                if (enemy != null)
                {
                    if (Vector3.Distance(enemy.transform.position, targetMovePoint) < attackRange)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<DungeonMob> GetAllEnemiesNear(Vector3 transformPosition)
        {
            return enemiesService.GetAllNearMobs(transformPosition, 1f);
        }

        public void SetTarget(DamageObject notDeadMob)
        {
            targetEnemy = notDeadMob;
        }

        public void SetRollDirection(Vector3 position)
        {
            if (playerCharacter.CurrentState != EStateType.Roll)
            {
                playerCharacter.GetCharacterAction<Dungeon_CharActionRoll>().SetCustomRollPosition(position);
                StartRoll();
            }
        }
    }
}
