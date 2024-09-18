using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Content.Scripts.BoatGame
{
    public class CameraRendererPreview : MonoBehaviour
    {
        [SerializeField] private Camera camera;

        private void Awake()
        {
            camera.backgroundColor = Color.clear;
        }


        public RenderTexture CreateRenderTexture(int width, int height)
        {
            RenderTexture rt = new RenderTexture(width, height, 16, GraphicsFormat.R8G8B8A8_UNorm, 0);

            SetRenderTexture(rt);
            return rt;
        }
        public void SetRenderTexture(RenderTexture rt)
        {
            camera.targetTexture = rt;
        }
    }
}
