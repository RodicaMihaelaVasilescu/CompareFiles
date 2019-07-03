using EnvDTE;
using EnvDTE80;
using FileDiffer.Commands;
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
        protected DTE2 dte;
        protected OleMenuCommandService CommandService;
        protected static Guid CommandSet;
        protected int CommandId;

        public BasicCommand(DTE2 DTE, OleMenuCommandService commandService, Guid commandSet, int commandId)
        {
            dte = DTE;
            CommandService = commandService;
            CommandSet = commandSet;
            CommandId = commandId;

        }


    }
}
