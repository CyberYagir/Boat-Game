using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame
{
    public class CrossSceneContext : MonoBehaviour
    {
        private static CrossSceneContext Instance;
        private CharacterService characterService;

        [Inject]
        private void Construct(CharacterService characterService)
        {
            Instance = this;
            this.characterService = characterService;
        }

        public static CharacterService GetCharactersService() => Instance.characterService;

    }
}
