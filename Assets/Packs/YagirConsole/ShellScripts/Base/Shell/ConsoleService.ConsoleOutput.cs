using System.Linq;
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
            private int repeatLastMessageCount;

            public void Init(TMP_Text outputText)
            {
                this.outputText = outputText;
            }

            public void OnReceivedUnityMessage(string condition, string stacktrace, LogType type)
            {
                if (outputText.text.Length > 8192)
                {
                    ClearText();
                }
                LogText(condition, (ELogType) (int) type);
            }


            public void LogText(string message, ELogType type)
            {
                if (message != lastMessage)
                {
                    var text = ConsoleLogger.GetLog(message, type);
                    outputText.text += text;
                    lastMessage = message;
                    repeatLastMessageCount = 0;
                }
                else
                {
                    repeatLastMessageCount++;
                    if (repeatLastMessageCount > 1)
                    {
                        if (repeatLastMessageCount > 2)
                        {
                            var oldCounter = GetRepeatCount(repeatLastMessageCount-1);
                            if (outputText.text.Length != 0)
                            {
                                var newText = outputText.text.Substring(0, (outputText.text.Length - oldCounter.Length));
                                newText += GetRepeatCount(repeatLastMessageCount);
                                outputText.text = newText;
                            }
                        }
                        else if (repeatLastMessageCount == 2)
                        {
                            if (!string.IsNullOrEmpty(outputText.text) && !string.IsNullOrWhiteSpace(outputText.text))
                            {
                                if (outputText.text.Last() == '\n')
                                {
                                    outputText.text = outputText.text.Substring(0, outputText.text.Length - 1);
                                }

                                outputText.text += GetRepeatCount(repeatLastMessageCount);
                            }
                        }
                    }
                }

                string GetRepeatCount(int val)
                {
                    return $" <color=#FFFFFF50>[x{val}]\n";
                }
            }


            public void ClearOutput()
            {
                outputText.text = "";
            }
        }
    }
}