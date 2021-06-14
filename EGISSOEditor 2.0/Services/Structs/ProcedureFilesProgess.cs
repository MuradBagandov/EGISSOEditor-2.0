using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Services.Structs
{
    public struct ProcedureFilesProgess
    {
        public string ProcessName, CurrentFileName;
        public int ProcessedFiles, TotalFiles;
        public float CurrentFileProgress, TotalFilesProgress;
        public bool IsEndOfProcessed;

        public ProcedureFilesProgess(string processName, string currentFileName, int processedFiles, int totalFiles, float currentFileProgress, float totalCurrentFileProgress)
        {
            ProcessName = processName;
            CurrentFileName = currentFileName;
            ProcessedFiles = processedFiles;
            TotalFiles = totalFiles;
            CurrentFileProgress = currentFileProgress;
            TotalFilesProgress = totalCurrentFileProgress;
            IsEndOfProcessed = false;
        }
    }
}
