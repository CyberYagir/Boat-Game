using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame
{
    public class CrossSceneContext : MonoBehaviour
    {
        private static CrossSceneContext Instance;
        private CharacterService characterService;
        private SaveService saveService;

        [Inject]
        private void Construct(CharacterService characterService, SaveService saveService)
        {
            Instance = this;
            this.saveService = saveService;
            this.characterService = characterService;
        }

        public static CharacterService GetCharactersService() => Instance.characterService;
        public static SaveService GetSaveService() => Instance.saveService;

    }
}
