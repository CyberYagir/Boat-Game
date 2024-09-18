using Content.Scripts.BoatGame.Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UISoulsShopItem: MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private UICraftSubItem subItem;
        [SerializeField] private TMP_Text soulsCount;
        private SoulsResourceContainerSO container;

        public void Init(SoulsResourceContainerSO it)
        {
            container = it;
            subItem.gameObject.SetActive(true);
            for (int i = 0; i < it.Resources.Length; i++)
            {
                var i1 = i;
                Instantiate(subItem, subItem.transform.parent)
                    .With(x => x.Init(0, it.Resources[i1].Item.ItemIcon))
                    .With(x => x.DrawValues(it.Resources[i1].Count));
            }
            subItem.gameObject.SetActive(false);

            soulsCount.text = it.SoulsCount.ToString();
        }

        public void ApplyRenderTexture(RenderTexture renderTexture)
        {
            rawImage.texture = renderTexture;
        }
    }
}