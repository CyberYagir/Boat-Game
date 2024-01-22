using System.Collections.Generic;

namespace YagirConsole.Scripts.Base.Shell
{
    public interface ICommandExecutable
    {
        public List<ConsoleCommandData> CommandsList { get; }
    }
}