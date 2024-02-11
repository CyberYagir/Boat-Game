using TMPro;
using UnityEngine;

namespace Packs.YagirConsole.ShellScripts.Base.Shell
{
    public partial class ConsoleService
    {
        [System.Serializable]
        public class ConsoleOutput
        {
            private TMP_Text outputText;
            private string lastMessage;

            public void Init(TMP_Text outputText)
            {
                this.outputText = outputText;
            }

            public void OnReceivedUnityMessage(string condition, string stacktrace, LogType type)
            {
                if (condition != lastMessage)
                {
                    if (outputText.text.Length > 8192)
                    {
                        ClearText();
                    }

                    LogText(condition, (ELogType)(int)type);
                    lastMessage = condition;
                }
            }


            public void LogText(string message, ELogType type)
            {
                var text = ConsoleLogger.GetLog(message,  type);
                outputText.text += text;
                lastMessage = text;
            }


            public void ClearOutput()
            {
                outputText.text = "";
            }
        }
    }
}