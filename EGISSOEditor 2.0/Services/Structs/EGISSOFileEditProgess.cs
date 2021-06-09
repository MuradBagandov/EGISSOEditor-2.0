using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Services.Structs
{
    public struct EGISSOFilesEditProgess
    {
        public string ProcessName, CurrentFileName;
        public int ProcessedFiles, TotalFiles, CurrentFileProgress, TotalFilesProgress;

        public EGISSOFilesEditProgess(string processName, string currentFileName, int processedFiles, int totalFiles, int currentFileProgress, int totalCurrentFileProgress)
        {
            ProcessName = processName;
            CurrentFileName = currentFileName;
            ProcessedFiles = processedFiles;
            TotalFiles = totalFiles;
            CurrentFileProgress = currentFileProgress;
            TotalFilesProgress = totalCurrentFileProgress;
        }

        public void Change(string processName, string currentFileName, int processedFiles, int totalFiles, int currentFileProgress, int totalCurrentFileProgress)
        {
            ProcessName = processName;
            CurrentFileName = currentFileName;
            ProcessedFiles = processedFiles;
            TotalFiles = totalFiles;
            CurrentFileProgress = currentFileProgress;
            TotalFilesProgress = totalCurrentFileProgress;
        }
    }
}
