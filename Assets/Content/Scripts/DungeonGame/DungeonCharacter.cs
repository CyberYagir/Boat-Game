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
        
        private DungeonSelectionService dungeonSelectionService;

        public DungeonSelectionService SelectionService => dungeonSelectionService;

        public PlayerCharacter PlayerCharacter => playerCharacter;

        public void Init(
            Character character,
            GameDataObject gameData,
            PrefabSpawnerFabric prefabSpawnerFabric,
            INavMeshProvider navMeshProvider,
            SaveDataObject saveDataObject,
            DungeonSelectionService dungeonSelectionService
        )
        {
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
        }

        public void MoveToPoint()
        {
            PlayerCharacter.ActiveAction(EStateType.MoveTo);
        }

        public void SetPosition(Vector3 getStartRoomRandomPos)
        {
            transform.position = getStartRoomRandomPos;
        }

        public Vector3 GetVelocity()
        {
            return PlayerCharacter.AIMoveManager.NavMeshAgent.Velocity;
        }
    }
}
