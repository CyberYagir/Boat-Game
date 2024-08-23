using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public interface ICharacterService
    {
        List<PlayerCharacter> GetSpawnedCharacters();
        PlayerCharacter GetClosestCharacter(Vector3 pos, out float distance);
        void SaveCharacters();
    }
}