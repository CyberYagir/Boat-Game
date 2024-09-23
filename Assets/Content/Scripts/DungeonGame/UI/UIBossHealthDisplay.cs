using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.DungeonGame.Services;
using UnityEngine;

namespace Content.Scripts.DungeonGame.UI
{
    public class UIBossHealthDisplay : MonoBehaviour
    {
        [SerializeField] private UIBar bar;
        public void Init(DungeonEnemiesService dungeonEnemiesService)
        {
            gameObject.SetActive(false);
            dungeonEnemiesService.OnBossSpawned += OnBossSpawned;
        }

        private void OnBossSpawned(DungeonMob obj)
        {
            bar.Init(string.Empty, obj.Health, obj.MaxHealth);
            gameObject.SetActive(true);
            obj.OnDamage += UpdateBar;
            obj.OnDeath += DisableBar;
        }

        private void DisableBar(DamageObject obj)
        {
            gameObject.SetActive(false);
        }

        private void UpdateBar(float obj)
        {
            bar.UpdateBar(obj);
        }
    }
}
