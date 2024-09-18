using System.Collections.Generic;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.UI
{
    public class UISoulsShopWindow : AnimatedWindow
    {
        [System.Serializable]
        public class SpawnedItem
        {
            [SerializeField] private UISoulsShopItem item;
            [SerializeField] private RenderTexture renderTexture;
            [SerializeField] private CameraRendererPreview spawned;

            public SpawnedItem(UISoulsShopItem item, CameraRendererPreview spawned)
            {
                this.item = item;
                this.spawned = spawned;
                renderTexture = spawned.CreateRenderTexture(512, 512);


                item.ApplyRenderTexture(renderTexture);
            }

            public void Active(bool state)
            {
                spawned.gameObject.SetActive(state);
            }
        }

        [SerializeField] private UISoulsShopItem item;

        private List<SpawnedItem> spawnedItems = new List<SpawnedItem>();
        private GameDataObject gameDataObject;

        public void Init(GameDataObject gameDataObject)
        {
            this.gameDataObject = gameDataObject;
        }

        public override void ShowWindow()
        {
            base.ShowWindow();
            
            Redraw();
        }

        public override void CloseWindow()
        {
            base.CloseWindow();
            foreach (var spawned in spawnedItems)
            {
                spawned.Active(false);
            }
        }

        private void Redraw()
        {
            if (spawnedItems.Count == 0)
            {
                int id = 0;
                item.gameObject.SetActive(true);
                foreach (var it in gameDataObject.SoulsShopContainers)
                {
                    var preview = Instantiate(it.PreviewPrefab, new Vector3(50 * id, 1000, 0), Quaternion.identity);
                    var spawnedItem = Instantiate(item, item.transform.parent)
                        .With(x => x.Init(it));
                    spawnedItems.Add(new SpawnedItem(spawnedItem, preview));
                    id++;
                }
                item.gameObject.SetActive(false);
            }

            for (int i = 0; i < spawnedItems.Count; i++)
            {
                spawnedItems[i].Active(true);
            }
        }
    }
}
