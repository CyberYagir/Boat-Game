using System.Collections.Generic;

namespace Packs.YagirConsole.ShellScripts.Base.Shell
{
    public interface ICommandExecutable
    {
        public List<ConsoleCommandData> CommandsList { get; }
    }
}