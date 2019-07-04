using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace FileDiffer.Commands
{
    class CommandController
    {
        #region Public Methods
        public async Task InitializeAsync(AsyncPackage package)
        {
            await DiffOpenFilesCommand.InitializeAsync(package, this);
            await DiffSolutionExplorerFilesCommand.InitializeAsync(package, this);
        }

        public void Execute(object sender, EventArgs e)
        {
            var menuCommand = (MenuCommand)sender;

            switch (menuCommand.CommandID.ID)
            {
                case ConstantsCommandIds.DiffOpenFilesCommandId:
                    {
                        DiffOpenFilesCommand.Instance.Execute(sender, e);
                        break;
                    }
                case ConstantsCommandIds.DiffSolutionExplorerFilesCommandId:
                    {
                        DiffSolutionExplorerFilesCommand.Instance.Execute(sender, e);
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion
    }
}