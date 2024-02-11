using System;
using UnityEngine;

namespace Packs.YagirConsole.ShellScripts.Base.Shell
{
    [System.Serializable]
    public class ConsoleLogger
    {
        private static ConsoleLogger instance;
        public static ConsoleLogger Instance => instance;

        [SerializeField] private ConsoleLoggerData loggerData;

        public void Init()
        {
            instance = this;
        }

        public static string GetLog(string message, ELogType type)
        {
            if (message.Contains(notFormattingFlag)) return message.Replace(notFormattingFlag, "");
            
            var color = Instance.loggerData.GetHex(type);
            var spaces = Instance.loggerData.MaxLogNameLength - type.ToString().Length;
            var str = $"<color={color}>[{type.ToString()}]";
            for (int i = 0; i < spaces; i++)
            {
                str += " ";
            }

            str += $" {message}</color>\n";

            return str;
        }

        private const string notFormattingFlag = "-0/_";
        
        public static void Log(string message, ELogType type)
        {
            switch (type)
            {
                case ELogType.Error:
                    Debug.LogError(message);
                    break;
                case ELogType.Assert:
                    Debug.LogAssertion(message);
                    break;
                case ELogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case ELogType.Log:
                    Debug.Log(message);
                    break;
                case ELogType.Exception:
                    Debug.LogException(new Exception(message));
                    break;
                case ELogType.CommandExeption:
                    Debug.Log(notFormattingFlag + GetLog(message, type));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}