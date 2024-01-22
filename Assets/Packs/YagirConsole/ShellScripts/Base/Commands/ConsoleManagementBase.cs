using System.Collections.Generic;
using YagirConsole.Scripts.Base.Shell;

namespace Packs.YagirConsole.ShellScripts.Base.Commands
{
    public class ConsoleManagementBase : ICommandExecutable
    {
        public List<ConsoleCommandData> CommandsList => commands;
        
        protected List<ConsoleCommandData> commands = new List<ConsoleCommandData>();
    }
}