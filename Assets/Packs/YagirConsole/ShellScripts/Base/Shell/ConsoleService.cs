using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YagirConsole.Scripts.Base.Shell
{

    public class ConsoleService : MonoBehaviour
    {
        private static ConsoleService Instance;

        [System.Serializable]
        public class LogsColors
        {
            [SerializeField] private LogType type;
            [SerializeField] private Color color;
            private string colorHex;

            public string ColorHex => colorHex;

            public LogType Type => type;

            public void Init()
            {
                colorHex = "#" + ColorUtility.ToHtmlStringRGB(color);
            }
        }

        [SerializeField] private Canvas canvas;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private TMP_Text outputText, hitText;
        [SerializeField] private List<LogsColors> colors;
        
        
        
        [SerializeField] private GameObject hintsHolder;
        [SerializeField] private List<ConsoleHintItem> hintsList;

        [SerializeField] private string commandsFullNamePath = "YagirConsole.Scripts.Base.Shell";
        [SerializeField] private KeyCode consoleKey = KeyCode.F2;
        private int selectedHint;

        private string lastMessage;
        private bool cursorVisible;
        private CursorLockMode cursorMode;

        private RectTransform rectTransform;
        private bool isOpened;


        private List<ICommandExecutable> commands = new List<ICommandExecutable>(100);

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Instance = this;
                outputText.gameObject.SetActive(false);
                hintsHolder.gameObject.SetActive(false);
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }

            foreach (var color in colors)
            {
                color.Init();
            }

            rectTransform = canvas.GetComponent<RectTransform>();
            canvas.enabled = false;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);
            ReloadShellCommands(commandsFullNamePath);
            input.onSubmit.AddListener(delegate { input.ActivateInputField(); });
            Application.logMessageReceived += ApplicationOnlogMessageReceived;
        }

        private void ApplicationOnlogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (condition != lastMessage)
            {
                if (outputText.text.Length > 8192)
                {
                    ClearText();
                }

                ShowText(condition, type);
                lastMessage = condition;
            }
        }

        private void LateUpdate()
        {
            OpenClose();
            if (isOpened)
            {
                HintMove();

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (input.text.Trim() != "")
                    {
                        if (selectedHint != -1)
                        {
                            if (!input.text.Contains(hintsList[selectedHint].GetText()))
                            {
                                input.text = hintsList[selectedHint].GetText() + " ";
                                input.caretPosition = input.text.Length;
                                return;
                            }
                        }

                        ShowText(input.text, LogType.Log);
                        CalculateText(input.text);
                        input.text = "";
                    }
                }
            }
        }

        public void HintMove()
        {
            if (input.text.Length != 0)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    selectedHint--;
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    selectedHint++;
                }

                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (selectedHint < -1)
                    {
                        selectedHint = -1;
                    }

                    if (selectedHint != -1)
                    {
                        if (selectedHint >= hintsList.Count || hintsList[selectedHint].gameObject.active == false)
                        {
                            selectedHint = 0;
                        }
                    }

                    for (int i = 0; i < hintsList.Count; i++)
                    {
                        if (i == selectedHint)
                            hintsList[i].Select();
                        else
                            hintsList[i].Deselect();
                    }

                    SetHintText();
                }
            }
        }

        List<string> normalCommands = new List<string>(10);
        List<List<Argument>> arguments = new List<List<Argument>>(10);

        public void UpdateHint()
        {

            input.text = input.text.Replace("  ", " ");
            
            var wordsCount = input.text.Trim().Split(' ').Length;
            if (wordsCount == 0 || wordsCount == 1)
            {
                CalculateHint();
            }

            if (input.text != "")
            {
                if ((wordsCount == 1 && input.text.Last() == ' ') || wordsCount > 1)
                {
                    SetArgumentsHint(wordsCount);
                }
            }
        }


        public void SetArgumentsHint(int wordsCount)
        {
            var commandText = input.text.Trim().ToLower().Split(' ')[0];
            var commandClass = commands.Find(x => x.CommandsList.Find(y => y.CommandBase == commandText) != null);
            if (commandClass != null)
            {
                var command = commandClass.CommandsList.Find(x => x.CommandBase == commandText);
                if (command.Arguments.Count != 0)
                {
                    hitText.text = input.text + " ";
                    hitText.text = hitText.text.Replace("  ", " ");
                    
                    for (int i = wordsCount - 1; i < command.Arguments.Count; i++)
                    {
                        if (i < command.Arguments.Count)
                        {
                            hitText.text += command.Arguments[i].ArgumentName + " ";
                        }
                    }
                }
            }
        }

        public void CalculateHint()
        {
            normalCommands.Clear();
            arguments.Clear();
            if (input.text.Length != 0 && input.text.First() == '/')
            {
                foreach (var scripts in commands)
                {
                    foreach (var command in scripts.CommandsList)
                    {
                        if (input.text.Length <= command.CommandBase.Length)
                        {
                            if (command.CommandBase.Contains(input.text))
                            {
                                normalCommands.Add(command.CommandBase);
                                arguments.Add(command.Arguments);
                            }
                        }
                    }
                }
            }

            selectedHint = -1;
            for (int i = 0; i < hintsList.Count; i++)
            {
                hintsList[i].Deselect();
            }
            SetHintText();
        }

        public void SetHintText()
        {
            if (normalCommands.Count != 0 && input.text.Length != 0)
            {
                var num = selectedHint == -1 ? 0 : selectedHint;
                hitText.text = normalCommands[num] + " ";
                for (int i = 0; i < arguments[num].Count; i++)
                {
                    hitText.text += arguments[num][i].ArgumentName + " ";
                }

                hintsHolder.gameObject.SetActive(true);
                for (int i = 0; i < hintsList.Count; i++)
                {
                    if (i < normalCommands.Count)
                    {
                        hintsList[i].SetText(normalCommands[i]);
                        hintsList[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        hintsList[i].gameObject.SetActive(false);
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(hintsHolder.GetComponent<RectTransform>());
            }
            else
            {
                hintsHolder.gameObject.SetActive(false);
                hitText.text = "";
            }
        }


        public static void ReloadShellCommands(string path)
        {
            string nspace = path;

            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                where t.IsClass && t.Namespace == nspace && t.GetInterface(nameof(ICommandExecutable)) == typeof(ICommandExecutable)
                select t;

            Instance.commands.Clear();

            foreach (var type in q)
            {
                var item = (ICommandExecutable) Activator.CreateInstance(type);
                if (item != null)
                {
                    Instance.commands.Add(item);
                }
            }
        }

        public static List<ICommandExecutable> GetCommands()
        {
            return Instance.commands;
        }

        private void CalculateText(string inputText)
        {
            var items = inputText.Trim().ToLower().Split(' ');

            if (items.Length >= 1)
            {

                var commandsClass = commands.Find(x => x.CommandsList.Find(y => y.CommandBase == items[0]) != null);
                if (commandsClass != null)
                {
                    var command = commandsClass.CommandsList.Find(y => y.CommandBase == items[0]);
                    if (command != null)
                    {
                        command.Execute(items.Skip(1).ToArray());
                        return;
                    }
                }

                Debug.LogError("Command not found");
            }
        }


        public void ShowText(string message, LogType type)
        {

            string str = "\n" + DateTime.Now.ToString("hh:mm:ss") + $" - <color={colors.Find(x => x.Type == type).ColorHex}>";
            str += message;
            str += $"</color>";

            outputText.text += str;


            lastMessage = str;
        }

        private void OpenClose()
        {
            if (Input.GetKeyDown(consoleKey))
            {
                isOpened = !isOpened;
                if (isOpened)
                {
                    rectTransform.DOKill();
                    canvas.enabled = true;
                    outputText.gameObject.SetActive(true);
                    rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 500), 0.2f);
                    input.Select();
                    cursorVisible = Cursor.visible;
                    cursorMode = Cursor.lockState;

                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    HideConsole();
                }
            }
        }

        public static void HideConsole()
        {
            Instance.rectTransform.DOSizeDelta(new Vector2(Instance.rectTransform.sizeDelta.x, 0), 0.2f).onComplete += () =>
            {
                Instance.canvas.enabled = false;
                Instance.outputText.gameObject.SetActive(false);
                Instance.hintsHolder.gameObject.SetActive(false);
            };

            Cursor.visible = Instance.cursorVisible;
            Cursor.lockState = Instance.cursorMode;

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                new GameObject("EventSystem").AddComponent<EventSystem>();
            }

            Instance.isOpened = false;
        }

        public static void ClearText()
        {
            Instance.outputText.text = "";
        }
    }
}
