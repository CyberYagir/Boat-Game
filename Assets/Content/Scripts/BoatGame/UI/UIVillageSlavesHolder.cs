using Content.Scripts.Global;
using Content.Scripts.IslandGame.Natives;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using static Content.Scripts.Global.SaveDataObject.MapData.IslandData.VillageData;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageSlavesHolder : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private Transform holder;
        [SerializeField] private GameObject soldSign;
        [SerializeField] private GameObject graveyard;

        private GameObject character;
        private RenderTexture renderTexture;
        private SlaveData slaveData;

        public RenderTexture RenderTexture => renderTexture;

        public void Init(GameObject prefab, SlaveData slaveData)
        {
            this.slaveData = slaveData;
            renderTexture = new RenderTexture(1024, 1024, 32, GraphicsFormat.R8G8B8A8_SNorm);

            camera.targetTexture = RenderTexture;

            character = Instantiate(prefab, holder)
                .With(x=>x.ChangeLayerWithChilds(LayerMask.NameToLayer("WorldUI")))
                .With(x=>soldSign.transform.parent = x.GetComponent<NativeSoldSign>().NeckBone);
            
            
            soldSign.transform.localPosition = Vector3.zero;
            
            graveyard.gameObject.SetActive(false);
            soldSign.gameObject.SetActive(false);

            DeadCheck();
        }

        public void DeadCheck()
        {
            if (slaveData.IsDead)
            {
                character.gameObject.SetActive(false);
                graveyard.gameObject.SetActive(true);
                camera.Render();
                camera.gameObject.SetActive(false);
            }
        }


        public void ActiveSold() => soldSign.gameObject.SetActive(true);
    }
}
