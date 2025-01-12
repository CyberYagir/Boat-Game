using System;
using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public class GameStateService : MonoBehaviour
    {
        public enum EGameState
        {
            Normal,
            Building,
            Removing,
            BuildingStructures
        }

        [SerializeField] private EGameState gameState;

        public EGameState GameState => gameState;

        public event Action OnChangeState;
        public event Action<EGameState> OnChangeEState;

        public void ChangeGameState(EGameState gameState)
        {
            this.gameState = gameState;
            OnChangeState?.Invoke();
            OnChangeEState?.Invoke(this.gameState);
        }
    }
}
