using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace YagirConsole.Scripts.Base.Shell
{
    public class ConsoleCommandData
    {
        private string commandBase;
        private List<Argument> arguments = new List<Argument>();
        private UnityEvent<ArgumentsShell> action = new UnityEvent<ArgumentsShell>();


        public List<Argument> Arguments => arguments;

        public string CommandBase => commandBase;

        public UnityEvent<ArgumentsShell> Action => action;

        public ConsoleCommandData(string commandBase, List<Argument> arguments)
        {
            this.commandBase = commandBase;
            this.arguments = arguments;
        }

        public void Execute(string[] arguments)
        {
            var isValid = false;
            var args = StringToArguments(arguments, out isValid);

            if (isValid)
            {
                var shell = new ArgumentsShell(args);
                try
                {
                    Action.Invoke(shell);
                }
                catch (Exception e)
                {
                    Debug.LogError("Execution error\n   " + e.Message);
                }
            }
        }
        
        private List<Argument> StringToArguments(string[] arguments, out bool isCan)
        {
            var args = new List<Argument>();

            isCan = true;
            for (int i = 0; i < arguments.Length; i++)
            {
                args.Add(new Argument(arguments[i]));
            }

            if (args.Count == this.arguments.Count)
            {
                for (int i = 0; i < this.arguments.Count; i++)
                {
                    if (this.arguments[i] != args[i])
                    {
                        isCan = false;
                        Debug.LogError("Argument types do not match");
                        break;
                    }
                    else
                    {
                        args[i].SetName(this.arguments[i].ArgumentName);
                    }
                }

                if (isCan)
                {
                    return args;
                }
            }
            else
            {
                Debug.LogError("Invalid number of arguments");
            }
            isCan = false;
            return args;
        }
    }
}