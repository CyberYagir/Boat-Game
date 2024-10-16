using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Content.Scripts.BoatGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create LightningSO", fileName = "LightningSO", order = 0)]
    public class LightningSO : ScriptableObject
    {
        [SerializeField] private Color lightColor;
        [SerializeField] private float lightIntensity;
        [SerializeField, ColorUsage(true, true)] private Color groundColor;
        [SerializeField, ColorUsage(true, true)] private Color horizontalColor;
        [SerializeField, ColorUsage(true, true)] private Color skyColor;
        [SerializeField, ColorUsage(true, true)] private Color shadowColor;
        [SerializeField] private bool haveFog;
        [SerializeField, ShowIf("@haveFog")] private FogMode fogMode;
        [SerializeField, ShowIf("@haveFog")] private Color fogColor;
        
        [SerializeField, ShowIf("@haveFog && fogMode == FogMode.Linear")] private float fogStart, fogEnd;
        [SerializeField, ShowIf("@haveFog && fogMode == FogMode.Exponential || fogMode == FogMode.ExponentialSquared")] private float density;


        [Button]
        public void Apply()
        {
            RenderSettings.fog = haveFog;
            RenderSettings.fogMode = fogMode;
            RenderSettings.fogDensity = density;
            RenderSettings.fogStartDistance = fogStart;
            RenderSettings.fogEndDistance = fogEnd;
            RenderSettings.fogColor = fogColor;

            RenderSettings.subtractiveShadowColor = shadowColor;


            RenderSettings.ambientMode = AmbientMode.Trilight;

            RenderSettings.ambientSkyColor = skyColor;
            RenderSettings.ambientEquatorColor = horizontalColor;
            RenderSettings.ambientGroundColor = groundColor;

            RenderSettings.sun.color = lightColor;
            RenderSettings.sun.intensity = lightIntensity;
        }
    }
}
