using Content.Scripts.BoatGame;
using Content.Scripts.DungeonGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Mobs
{
    public class DungeonBoss : MonoBehaviour
    {
        private DungeonService dungeonService;
        private DungeonSaveService dungeonSaveService;

        [Inject]
        private void Construct(DungeonService dungeonService, DungeonSaveService dungeonSaveService)
        {
            this.dungeonSaveService = dungeonSaveService;
            this.dungeonService = dungeonService;
            GetComponent<DamageObject>().OnDeath += SaveBossDefeat;
        }

        private void SaveBossDefeat(DamageObject obj)
        {
            dungeonService.SetBossDead();
            dungeonSaveService.SaveWorld();
        }
    }
}
