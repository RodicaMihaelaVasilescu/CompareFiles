using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace FileDiffer.Commands
{

    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class DiffOpenFilesCommand
    {
        #region Properties
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = ConstantsCommandIds.DiffOpenFilesCommandId;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("8f4c6076-ae3c-4814-9c63-6c12b165db7c");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static DiffOpenFilesCommand Instance
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private DiffOpenFilesCommand(AsyncPackage package, OleMenuCommandService commandService, CommandController commandController)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(commandController.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        #endregion

        #region Service Provider

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package, CommandController commandController)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new DiffOpenFilesCommand(package, commandService, commandController);
        }

        #endregion

        #region Private Methods
        public async void Execute(object sender, EventArgs e)
        {
            string file1, file2;
            var menuCommand = (MenuCommand)sender;
            var dte = (DTE2)await ServiceProvider.GetServiceAsync(typeof(DTE));
            if (dte == null)
            {
                return;
            }

            if (CanFilesBeCompared(dte, out file1, out file2))
            {
                dte.ExecuteCommand("Tools.DiffFiles", $"\"{file1}\" \"{file2}\"");
            }
        }

        private bool CanFilesBeCompared(DTE2 dte, out string file1, out string file2)
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
        private IEnumerable<string> GetDocuments(DTE2 dte)
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

        #endregion
    }
}
