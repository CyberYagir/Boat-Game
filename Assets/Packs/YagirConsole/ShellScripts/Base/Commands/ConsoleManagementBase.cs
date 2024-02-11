using System.Collections.Generic;
using Packs.YagirConsole.ShellScripts.Base.Shell;

namespace Packs.YagirConsole.ShellScripts.Base.Commands
{
    public class ConsoleManagementBase : ICommandExecutable
    {
        public List<ConsoleCommandData> CommandsList => commands;
        
        protected List<ConsoleCommandData> commands = new List<ConsoleCommandData>();
    }
}