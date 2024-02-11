using System.Collections.Generic;
using Packs.YagirConsole.ShellScripts.Base.Shell;
using UnityEngine;

namespace Packs.YagirConsole.ShellScripts.Base.Commands
{
    public class ConsoleVisualManagement : ConsoleManagementBase
    {
        public ConsoleVisualManagement()
        {
            ClearCommand();
            HelpCommand();
            CommandTest();
        }

        private void HelpCommand()
        {
            var command = new ConsoleCommandData("/help", new List<Argument>());
            
            command.Action.AddListener(delegate(ArgumentsShell arg0)
            {
                var commands = ConsoleService.GetCommands();
                Debug.Log("Commands List:");
                for (int i = 0; i < commands.Count; i++)
                {
                    for (int j = 0; j < commands[i].CommandsList.Count; j++)
                    {
                        if (commands[i].CommandsList[j].CommandBase == command.CommandBase) continue;
                        
                        
                        string str = commands[i].CommandsList[j].CommandBase;

                        for (int k = 0; k < commands[i].CommandsList[j].Arguments.Count; k++)
                        {
                            str += $" <u>({commands[i].CommandsList[j].Arguments[k].Type.ToString()})`{commands[i].CommandsList[j].Arguments[k].ArgumentName}`</u>";
                        }

                        Debug.Log(str);
                    }
                }
            });

            commands.Add(command);
        }


        private void CommandTest()
        {
            var command = new ConsoleCommandData("/test", new List<Argument>()
            {
                new Argument("int", ArgumentType.Number),
                new Argument("string", ArgumentType.String),
                new Argument("bool", ArgumentType.Bool)
            });
            commands.Add(command);
        }


        public void ClearCommand()
        {
            var command = new ConsoleCommandData("/clear", new List<Argument>());
            command.Action.AddListener(delegate(ArgumentsShell args)
            {
                ConsoleService.ClearText();
            });
            
            commands.Add(command);
        }
    }
}
