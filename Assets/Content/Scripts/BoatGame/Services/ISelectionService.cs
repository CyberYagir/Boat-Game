using System;
using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public interface ISelectionService
    {
        event Action<PlayerCharacter> OnChangeSelectCharacter;
        PlayerCharacter SelectedCharacter { get; }
        Vector3 GetUnderMousePosition(out bool isNotEmpty);
    }
}