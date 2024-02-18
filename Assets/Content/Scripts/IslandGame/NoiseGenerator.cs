using DG.DemiLib;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    [System.Serializable]
    public class NoiseGenerator
    {
        [SerializeField, OnValueChanged(nameof(GenerateTexture))]
        private int mapWidth;

        [SerializeField, OnValueChanged(nameof(GenerateTexture))]
        private int mapHeight;

        [SerializeField, OnValueChanged(nameof(GenerateTexture))]
        private float scale;

        [SerializeField, OnValueChanged(nameof(GenerateTexture))]
        private int octaves;

        [SerializeField, OnValueChanged(nameof(GenerateTexture))]
        private float persistance;

        [SerializeField, OnValueChanged(nameof(GenerateTexture))]
        private float lacunarity;

        [SerializeField, OnValueChanged(nameof(GenerateTexture))]
        private Vector2 offset;

        [SerializeField, OnValueChanged(nameof(GenerateTexture))]
        private Range clamp;


        [SerializeField, PreviewField(alignment: ObjectFieldAlignment.Center, FilterMode = FilterMode.Point, Height = 256)]
        private Texture2D texture;

        private float[,] generatedNoise;

        public float[,] GeneratedNoise => generatedNoise;

        public float[,] GetNoise(int seed, int width, int height)
        {
            generatedNoise = Noise.GenerateNoiseMap(width, height, seed, scale, octaves, persistance, lacunarity, offset);
            return GeneratedNoise;
        }


        public void GenerateTexture()
        {
            Texture2D tex = new Texture2D(mapWidth, mapHeight, TextureFormat.RG32, false);

            var cord = GetNoise(0, mapWidth, mapHeight);


            for (int i = 0; i < tex.width; i++)
            {
                for (int j = 0; j < tex.height; j++)
                {
                    if (IsInRange(cord[i, j]))
                    {
                        tex.SetPixel(i, j, Color.white * cord[i, j]);
                    }
                    else
                    {
                        tex.SetPixel(i, j, Color.black);
                    }
                }
            }

            tex.Apply();
            texture = tex;
        }

        public bool IsInRange(float f)
        {
            return f >= clamp.min && f <= clamp.max;
        }
    }
}