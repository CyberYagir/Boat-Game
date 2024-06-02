using System.Collections.Generic;
using UnityEngine;

namespace ConsoleShell
{
    public class ConsoleVisualManagement : ConsoleManagementBase
    {
        public ConsoleVisualManagement()
        {
            ClearCommand();
            HelpCommand();
        }

        private void HelpCommand()
        {
            var command = AddCommand("/help", new List<Argument>(), null);
            
            command.Action += delegate(ArgumentsShell shell)
            {
                var commands = ConsoleService.GetCommandsStatic().Commands;

                var fullResult = "Commands List:";

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

                        fullResult += "\n    " + str;
                    }
                }

                Debug.Log(fullResult);
            };
        }


        public void ClearCommand()
        {
            AddCommand("/clear", new List<Argument>(), delegate(ArgumentsShell shell) { ConsoleService.ClearText(); });
        }
    }
}
