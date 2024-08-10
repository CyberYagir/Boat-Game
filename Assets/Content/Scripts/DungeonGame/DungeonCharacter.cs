using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
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

        private DungeonSelectionService dungeonSelectionService;
        private DungeonEnemiesService enemiesService;
        private DungeonMob targetEnemy;

        public DungeonSelectionService SelectionService => dungeonSelectionService;
        public PlayerCharacter PlayerCharacter => playerCharacter;
        public float AttackRange => attackRange;

        public DungeonMob TargetEnemy => targetEnemy;

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
        }

        private void Update()
        {
            if (!rvoController.enabled)
            {
                rvoController.enabled = true;
            }

            UpdateAttackRange();
        }

        public void MoveToPoint()
        {
            PlayerCharacter.ActiveAction(EStateType.MoveTo);
            targetEnemy = null;
        }

        public void SetPosition(Vector3 getStartRoomRandomPos)
        {
            transform.position = getStartRoomRandomPos;
        }

        public void UpdateAttackRange()
        {
            if (!InputService.IsLMBPressed)
            {
                if (PlayerCharacter.CurrentState == EStateType.Idle)
                {
                    if (dungeonSelectionService.LastPoint.ToDistance(transform.position) < attackRange)
                    {
                        var enemy = enemiesService.GetNearMob(transform.position);
                        if (enemy != null)
                        {
                            if (enemy.transform.position.ToDistance(transform.position) < attackRange)
                            {
                                targetEnemy = enemy;
                                PlayerCharacter.ActiveAction(EStateType.Attack);
                            }
                        }
                    }
                }
            }
        }
    }
}
