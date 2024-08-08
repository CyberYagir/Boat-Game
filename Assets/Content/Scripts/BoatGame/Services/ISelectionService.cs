using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public interface ISelectionService
    {
        PlayerCharacter SelectedCharacter { get; }
        Vector3 GetUnderMousePosition(out bool isNotEmpty);
    }
}