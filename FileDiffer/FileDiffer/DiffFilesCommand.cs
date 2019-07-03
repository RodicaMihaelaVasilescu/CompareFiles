using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using FileDiffer.Commands;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace FileDiffer
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class DiffFilesCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;
        public const int CommandId2 = 0x0101;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("8f4c6076-ae3c-4814-9c63-6c12b165db7c");
        //public static readonly Guid CommandSet2 = new Guid("69295e2b-8adf-477e-9029-ef1bfb58dc1f");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffFilesCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private DiffFilesCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            //CreateCommand (commandService, CommandSet, CommandId);
            //CreateCommand(commandService, CommandSet, CommandId2);
            LoadCommandsAsync(commandService);
        }

        private async Task LoadCommandsAsync(OleMenuCommandService commandService)
        {
            //var dte = (DTE2)await ServiceProvider.GetServiceAsync(typeof(DTE));
            //DiffFileOpenDocuments commandOpenDocs = new DiffFileOpenDocuments(dte, commandService, new Guid("8f4c6076-ae3c-4814-9c63-6c12b165db7c"), 0x0100);
            //DiffFileSolutionExplorer commandSolutionExplorer = new DiffFileSolutionExplorer(dte, commandService, new Guid("69295e2b-8adf-477e-9029-ef1bfb58dc1f"), 0x0100);
        }

        private void CreateCommand(OleMenuCommandService commandService, Guid CommandSet, int CommandId)
        {
            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        // cmd 1 => DiffFileSolutionExplorer
        // cmd 2 => DiffFileOpenDocuments


        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static DiffFilesCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in DiffFilesCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new DiffFilesCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            var dte = (DTE2)await ServiceProvider.GetServiceAsync(typeof(DTE));

            string file1, file2;
            var menuCommand = (MenuCommand)sender;
            bool canFilesBeCompared = menuCommand.CommandID.Guid == CommandSet ?
                CanFilesBeCompared(dte, out file1, out file2) :
                CanFilesBeCompared2(dte, out file1, out file2);

            if (canFilesBeCompared)
            {
                dte.ExecuteCommand("Tools.DiffFiles", $"\"{file1}\" \"{file2}\"");
            }
        }

        public static bool CanFilesBeCompared(DTE2 dte, out string file1, out string file2)
        {
            file1 = null;
            file2 = null;
            var items = GetDocuments(dte);

            if (items.Count() > 1)
            {
                file1 = items.ElementAtOrDefault(0);
                file2 = items.ElementAtOrDefault(1);
            }

            return !string.IsNullOrEmpty(file1) && !string.IsNullOrEmpty(file2);
        }

        public static bool CanFilesBeCompared2(DTE2 dte, out string file1, out string file2)
        {
            var items = GetSelectedFiles(dte);

            file1 = items.ElementAtOrDefault(0);
            file2 = items.ElementAtOrDefault(1);

            if (items.Count() == 1)
            {
                var dialog = new OpenFileDialog();
                dialog.InitialDirectory = Path.GetDirectoryName(file1);
                dialog.ShowDialog();
                file2 = dialog.FileName;
            }

            if (items.Count() >= 3)
            {
                MessageBox.Show("You can compare only 2 files.");
                return false;
            }

            return !string.IsNullOrEmpty(file1) && !string.IsNullOrEmpty(file2);
        }

        public static IEnumerable<string> GetSelectedFiles(DTE2 dte)
        {
            var items = (Array)dte.ToolWindows.SolutionExplorer.SelectedItems;
            return from item in items.Cast<UIHierarchyItem>()
                   let pi = item.Object as ProjectItem
                   select pi.FileNames[1];
        }
        public static IEnumerable<string> GetDocuments(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var allDocuments = dte.Documents;
            List<string> documentNames = new List<string>();
            for (int i = 1; i <= allDocuments.Count; i++)
            {
                documentNames.Add(allDocuments.Item(i).FullName);
            }
            return documentNames;
        }
    }
}
