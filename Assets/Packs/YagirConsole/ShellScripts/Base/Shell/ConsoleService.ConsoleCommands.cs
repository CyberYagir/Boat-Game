using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Packs.YagirConsole.ShellScripts.Base.Shell
{
    public partial class ConsoleService
    {
        [System.Serializable]
        public class ConsoleCommands
        {
            [SerializeField] private string commandsFullNamePath = "YagirConsole.Scripts.Base.Shell";
            
            private List<ICommandExecutable> commands = new List<ICommandExecutable>(100);
            public List<ICommandExecutable> Commands => commands;

            public void ReloadShellCommands()
            {
                
                var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == commandsFullNamePath && t.GetInterface(nameof(ICommandExecutable)) == typeof(ICommandExecutable)
                    select t;

                Commands.Clear();

                foreach (var type in q)
                {
                    var item = (ICommandExecutable) Activator.CreateInstance(type);
                    if (item != null)
                    {
                        Commands.Add(item);
                    }
                }
            }
        }
    }
}