using Content.Scripts.DungeonGame.Services;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIDungeonMobsCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Slider bar;
        [SerializeField] private RectTransform holder;
        private DungeonEnemiesService enemiesService;
        private DungeonService dungeonService;

        public void Init(DungeonEnemiesService enemiesService, DungeonService dungeonService)
        {
            this.dungeonService = dungeonService;
            this.enemiesService = enemiesService;
            enemiesService.OnChangeEnemies += UpdateWidget;

            if (dungeonService.GetDeadMobsCount() >= dungeonService.GetMaxMobsCount())
            {
                gameObject.SetActive(false);
            }

            UpdateWidget();
        }

        private void UpdateWidget()
        {
            text.text = dungeonService.GetDeadMobsCount().ToString("00") + "/" + dungeonService.GetMaxMobsCount().ToString("00");

            bar.DOValue(dungeonService.GetDeadMobsCount() / (float) dungeonService.GetMaxMobsCount(), 0.2f);

            if (dungeonService.GetDeadMobsCount() >= dungeonService.GetMaxMobsCount())
            {
                holder.DOScale(Vector3.zero, 0.2f).onComplete += ONComplete;
            }
        }

        private void ONComplete()
        {
            holder.gameObject.SetActive(false);
        }
    }
}
