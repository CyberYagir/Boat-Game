using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;

namespace Content.Scripts.Global
{
    public partial class SaveDataObject
    {
        [Serializable]
        public class CharactersData
        {
            [SerializeField] private List<Character> characters = new List<Character>();

            public int Count => characters.Count;

            public void AddCharacter(Character character)
            {
                characters.Add(character);
            }

            public Character GetCharacter(int i)
            {
                return characters[i];
            }

            public void RemoveCharacter(string targetUid)
            {
                characters.RemoveAll(x => x.Uid == targetUid);
            }
        }
    }
}