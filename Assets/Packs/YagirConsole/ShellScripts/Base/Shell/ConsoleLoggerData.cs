using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packs.YagirConsole.ShellScripts.Base.Shell
{
    [CreateAssetMenu(menuName = "Scriptable/ConsoleLogger", fileName = "ConsoleLogger", order = 0)]
    public class ConsoleLoggerData : ScriptableObject
    {
        [System.Serializable]
        public class LogColors
        {
            [SerializeField] private ELogType type;
            [SerializeField] private Color color;
            [SerializeField] private string colorHex;

            public string ColorHex => colorHex;

            public ELogType Type => type;

            public void Init()
            {
                colorHex = "#" + ColorUtility.ToHtmlStringRGB(color) + ((int)(color.a * 255f)).ToString("X");
            }
        }
        [SerializeField] private List<LogColors> colors;
        [SerializeField] private float maxLogNameLength;

        public float MaxLogNameLength => maxLogNameLength;


        private void OnValidate()
        {
            for (int i = 0; i < colors.Count; i++)
            {
                colors[i].Init();
            }
            maxLogNameLength = Enum.GetNames(typeof(ELogType)).Max(x => x.Length);
        }


        public string GetHex(ELogType type)
        {
            return colors.Find(x => x.Type == type).ColorHex;
        }
    } 
}