using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.UI;
using UnityEngine;

namespace Content.Scripts.DungeonGame.UI
{
    public class UIHealthbars : MonoBehaviour
    {
        public class PlayerHealth
        {
            private PlayerCharacter player;
            private UIBar bar;
            private float offset;

            public PlayerHealth(PlayerCharacter player, UIBar bar, float offset)
            {
                this.offset = offset;
                this.player = player;
                this.bar = bar;

                bar.Init("", player.NeedManager.Health, 100);

                player.NeedManager.OnDamaged += Update;
            }

            public void Update()
            {
                bar.UpdateBar(player.NeedManager.Health);
            }

            public Vector3 GetPlayerPosition()
            {
                if (player != null)
                {
                    return player.transform.position + Vector3.up * offset;
                }
                return Vector3.zero;
            }

            public void SetPosition(Vector3 worldToScreenPoint)
            {
                bar.transform.position = worldToScreenPoint;
            }
        }
        [SerializeField] private UIBar barPrefab;
        [SerializeField] private float offset = 2.5f;
        private List<PlayerHealth> bars = new List<PlayerHealth>(5);
        private Camera camera;

        public void Init(List<PlayerCharacter> playersList, Camera camera)
        {
            this.camera = camera;
            for (var i = 0; i < playersList.Count; i++)
            {
                bars.Add(new PlayerHealth(playersList[i], Instantiate(barPrefab, transform), offset));
            }
        }

        public void UpdateBars()
        {
            for (var i = 0; i < bars.Count; i++)
            {
                bars[i].SetPosition(camera.WorldToScreenPoint(bars[i].GetPlayerPosition()));
            }
        }
    }
}
