using Content.Scripts.IslandGame.Natives;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageSlavesHolder : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private Transform holder;
        [SerializeField] private GameObject soldSign;
        
        private RenderTexture renderTexture;

        public RenderTexture RenderTexture => renderTexture;

        public void Init(GameObject prefab)
        {
            renderTexture = new RenderTexture(1024, 1024, 32, GraphicsFormat.R8G8B8A8_SNorm);

            camera.targetTexture = RenderTexture;

            Instantiate(prefab, holder)
                .With(x=>x.ChangeLayerWithChilds(LayerMask.NameToLayer("WorldUI")))
                .With(x=>soldSign.transform.parent = x.GetComponent<NativeSoldSign>().NeckBone);
            
            
            soldSign.transform.localPosition = Vector3.zero;
            
            soldSign.gameObject.SetActive(false);
        }


        public void ActiveSold() => soldSign.gameObject.SetActive(true);
    }
}
