using System;
using System.Linq;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.DungeonGame.Characters.States;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class DungeonSelectionService : MonoBehaviour, ISelectionService
    {
        [SerializeField] private Camera camera;
        [SerializeField] private GameObject graphicsRaycastersHolder;
        
        private OverUIChecker overUIChecker;
        private DungeonCharactersService dungeonCharactersService;
        private Vector3 lastPoint;

        private bool isDown;

        public event Action<Vector3> OnPointChange;
        public Vector3 LastPoint => lastPoint;

        [Inject]
        private void Construct(DungeonCharactersService dungeonCharactersService)
        {
            this.dungeonCharactersService = dungeonCharactersService;
            overUIChecker = new OverUIChecker(graphicsRaycastersHolder);
        }
        private void Update()
        {
            overUIChecker.CheckUILogic();
            
            if (overUIChecker.IsUIBlocked && !isDown) return;
            
            if (InputService.IsLMBDown)
            {
                SetNewClickPosition();
                isDown = true;
            }else if (InputService.IsLMBPressed && isDown)
            {
                SetNewClickPosition();
            }else
            if (!InputService.IsLMBPressed)
            {
                isDown = false;
            }
            
            if (InputService.IsRMBPressed)
            {
                SetRollDirections();
            }
        }

        private void SetRollDirections()
        {
            var hit = camera.MouseRaycast(out var isHit, InputService.MousePosition);
            if (isHit)
            {
                for (int i = 0; i < dungeonCharactersService.SpawnedCharacters.Count; i++)
                {
                    dungeonCharactersService.SpawnedCharacters[i].SetRollDirection(hit.point);
                }
            }
        }

        private void SetNewClickPosition()
        {
            var hit = camera.MouseRaycast(out var isHit, InputService.MousePosition);

            if (isHit)
            {
                lastPoint = hit.point;
                foreach (var cha in dungeonCharactersService.SpawnedCharacters)
                {
                    cha.MoveToPoint();
                }

                OnPointChange?.Invoke(lastPoint);
            }
        }

        public event Action<PlayerCharacter> OnChangeSelectCharacter;

        private PlayerCharacter activePlayer;
        
        public PlayerCharacter SelectedCharacter
        {
            get
            {
                if (activePlayer)
                {
                    return activePlayer;
                }
                var minHealth = dungeonCharactersService.SpawnedCharacters.Min(x => x.PlayerCharacter.NeedManager.Health);
                return dungeonCharactersService.SpawnedCharacters.Find(x=>x.PlayerCharacter.NeedManager.Health <= minHealth).PlayerCharacter;
            }
        }

        public Vector3 GetUnderMousePosition(out bool isNotEmpty)
        {
            var hit = camera.MouseRaycast(out var isHit, InputService.MousePosition, Mathf.Infinity, LayerMask.GetMask("Player", "Terrain"));
            isNotEmpty = isHit;
            return hit.point;
        }

        public void SetActiveCharacter(PlayerCharacter playerCharacter)
        {
            activePlayer = playerCharacter;

            OnChangeSelectCharacter?.Invoke(activePlayer);
        }
    }
}
