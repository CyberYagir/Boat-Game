using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ConsoleShell
{
    public partial class ConsoleService
    {
        [System.Serializable]
        public class ConsoleOutput
        {
            [Flags]
            public enum EAcceptedLogTypes
            {
                Errors = 8,
                Warnings = 16,
                Logs = 32
            }
            
            
            [SerializeField] private TextsPool copyTextsPool;
            private EAcceptedLogTypes acceptedLogTypes;
            private List<string> allMessages = new List<string>(100);
            private List<ELogType> messagesTypes = new List<ELogType>(100);
            private string lastMessage;
            private int repeatLastMessageCount;
            private TMP_Text lastTextInstance;

            public List<string> AllMessages => allMessages;

            public EAcceptedLogTypes AcceptedLogTypes => acceptedLogTypes;


            public void Init()
            {
                acceptedLogTypes = EAcceptedLogTypes.Errors | EAcceptedLogTypes.Logs | EAcceptedLogTypes.Warnings;
                copyTextsPool.Init();
            }

            public void OnReceivedUnityMessage(string condition, string stacktrace, LogType type)
            {
                LogText(condition, (ELogType) (int) type);
            }


            public void LogText(string message, ELogType type)
            {
                if (message != lastMessage)
                {
                    var text = ConsoleLogger.GetLog(message, type);
                    var textInstance = copyTextsPool.Get();
                    textInstance.text = text;
                    lastTextInstance = textInstance;
                    lastMessage = message;
                    repeatLastMessageCount = 0;
                    AllMessages.Add(text);
                    messagesTypes.Add(type);
                    
                    textInstance.gameObject.SetActive(true);
                    textInstance.name = type.ToString();
                    textInstance.GetComponent<ContentSizeFitter>().SetLayoutVertical();

                    textInstance.enabled = IsCanLogThisType(type);
                }
                else
                {
                    repeatLastMessageCount++;
                    if (repeatLastMessageCount > 1)
                    {
                        if (repeatLastMessageCount > 2)
                        {
                            var oldCounter = GetRepeatCount(repeatLastMessageCount - 1);
                            if (lastTextInstance.text.Length != 0)
                            {
                                var newText = lastTextInstance.text.Substring(0, (lastTextInstance.text.Length - oldCounter.Length));
                                newText += GetRepeatCount(repeatLastMessageCount);
                                lastTextInstance.text = newText;
                            }
                        }
                        else if (repeatLastMessageCount == 2)
                        {
                            if (!string.IsNullOrEmpty(lastTextInstance.text) && !string.IsNullOrWhiteSpace(lastTextInstance.text))
                            {
                                if (lastTextInstance.text.Last() == '\n')
                                {
                                    lastTextInstance.text = lastTextInstance.text.Substring(0, lastTextInstance.text.Length - 1);
                                }

                                lastTextInstance.text += GetRepeatCount(repeatLastMessageCount);
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
                messagesTypes.Clear();
                copyTextsPool.ClearAllSpawnedButtons();
            }


            private bool IsCanLogThisType(ELogType type)
            {
                if (type == ELogType.Error || type == ELogType.Exception || type == ELogType.CmdException)
                {
                    return AcceptedLogTypes.HasFlag(EAcceptedLogTypes.Errors);
                }

                if (type == ELogType.Assert || type == ELogType.Warning)
                {
                    return AcceptedLogTypes.HasFlag(EAcceptedLogTypes.Warnings);
                }

                if (type == ELogType.CmdSuccess || type == ELogType.Log)
                {
                    return acceptedLogTypes.HasFlag(EAcceptedLogTypes.Logs);
                }

                return false;
            }

            public void ToggleLogType(EAcceptedLogTypes type)
            {
                if (acceptedLogTypes.HasFlag(type))
                {
                    acceptedLogTypes &= ~type;
                }
                else
                {
                    acceptedLogTypes |= type;
                }

                UpdateListByLoggedTypes();
            }

            private void UpdateListByLoggedTypes()
            {
                for (var i = 0; i < copyTextsPool.Spawned.Count; i++)
                {
                    copyTextsPool.Spawned[i].enabled = IsCanLogThisType(messagesTypes[i]);
                }
            }
        }
    }
}