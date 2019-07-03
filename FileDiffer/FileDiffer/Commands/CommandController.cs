using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDiffer.Commands
{
    class CommandController
    {
        //public async void LaunchCommandAsync(DTE2 dte, OleMenuCommandService commandService, Guid CommandSet, int CommandId)
        //{
        //    var menuCommandID = new CommandID(CommandSet, CommandId);
        //    OleMenuCommand menuItem = null;

        //    switch (CommandId)
        //    {
        //        case 0x0100:
        //            {
        //                var diffFileOpenDocumentsClass = new DiffFileOpenDocuments(dte, commandService, CommandSet, CommandId);
        //                menuItem = new OleMenuCommand(diffFileOpenDocumentsClass.Execute, menuCommandID);
        //                break;
        //            }
        //        case 0x0101:
        //            {
        //                var diffFileSolutionExplorerClass = new DiffFileOpenDocuments(dte, commandService, CommandSet, CommandId);
        //                menuItem = new OleMenuCommand(diffFileSolutionExplorerClass.Execute, menuCommandID);
        //                break;
        //            }
        //    }
        //    commandService.AddCommand(menuItem);
        //}


        private void DiffFileSolutionExplorer(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DiffFileOpenDocuments(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public async System.Threading.Tasks.Task InitializeAsync(AsyncPackage package)
        {
            DiffOpenDocuments.InitializeAsync(package, this);
        }

        internal void LaunchCommandAsync(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}