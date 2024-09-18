using System.Collections.Generic;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public abstract class CharacterServiceBase : MonoBehaviour
    {
        protected SaveDataObject saveData;
        public virtual List<PlayerCharacter> GetSpawnedCharacters() => null;
        
        public PlayerCharacter GetClosestCharacter(Vector3 pos, out float distance)
        {
            var minDist = 9999f;

            var characters = GetSpawnedCharacters();
            var character = characters[0];
            foreach (var c in characters)
            {
                var dist = Vector3.Distance(pos, c.transform.position);

                if (minDist > dist)
                {
                    minDist = dist;
                    character = c;
                }
            }


            distance = minDist;
            return character;
        }
        
        public void SaveCharacters()
        {
            foreach (var sp in GetSpawnedCharacters())
            {
                if (!sp.NeedManager.IsDead)
                {
                    sp.Character.SetParameters(sp.NeedManager.GetParameters());
                    sp.Character.SetEffects(sp.ParametersCalculator.GetEffectsData());
                }
            }
        }

        protected void AddSoul(Character obj)
        {
            saveData.CrossGame.AddSoul();
            saveData.SaveFile();
        }
    }
}