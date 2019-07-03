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
    class DiffFileOpenDocuments : BasicCommand
    {
        public DiffFileOpenDocuments(DTE2 DTE, OleMenuCommandService commandService, Guid CommandSet, int CommandId)
            : base( DTE, commandService, CommandSet, CommandId)
        {

            //var menuCommandID = new CommandID(CommandSet, CommandId);
            //var menuItem = new MenuCommand(this.Execute, menuCommandID);
            //commandService.AddCommand(menuItem);
        }
        public async void Execute(object sender, EventArgs e)
        {
            string file1, file2;
            var menuCommand = (MenuCommand)sender;

            if (CanFilesBeCompared(dte, out file1, out file2))
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
