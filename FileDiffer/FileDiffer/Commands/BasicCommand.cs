using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDiffer
{
    class BasicCommand
    {
        protected DTE2 Dte;
        protected OleMenuCommandService CommandService;
        protected static Guid CommandSet;
        protected int CommandId;

        public BasicCommand(DTE2 dte, OleMenuCommandService commandService, Guid commandSet, int commandId)
        {
            Dte = dte;
            CommandService = commandService;
            CommandSet = commandSet;
            CommandId = commandId;

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);

        }

        private async void Execute(object sender, EventArgs e)
        {

            string file1, file2;
            var menuCommand = (MenuCommand)sender;;

            if (CanFilesBeCompared(Dte, out file1, out file2))
            {
                dte.ExecuteCommand("Tools.DiffFiles", $"\"{file1}\" \"{file2}\"");
            }
        }

    }
}
