using UnityEngine;

namespace Content.Scripts.BoatGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create SoulsResourceContainerSO", fileName = "SoulsResourceContainerSO", order = 0)]
    public class SoulsResourceContainerSO : ResourcesContainerSO
    {
        [SerializeField] private CameraRendererPreview previewPrefab;
        [SerializeField] private int soulsCount;

        public int SoulsCount => soulsCount;

        public CameraRendererPreview PreviewPrefab => previewPrefab;
    }
}