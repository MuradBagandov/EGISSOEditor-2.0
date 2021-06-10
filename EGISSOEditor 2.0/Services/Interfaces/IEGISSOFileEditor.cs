using EGISSOEditor_2._0.Services.Structs;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Services.Interfaces
{
    interface IEGISSOFileEditor<T> where T: class
    {
        bool ValidateFile(string path);

        bool ValidateFile(T path);

        void FilesStyleCorrection(IEnumerable<T> file);

        Task FilesStyleCorrectionAsync(IEnumerable<T> file, IProgress<EGISSOFilesEditProgess> progress, CancellationToken cancel);

        void CorrectionErrorsFiles(IEnumerable<T> file);

        Task CorrectionErrorsFilesAsync(IEnumerable<T> file, IProgress<EGISSOFilesEditProgess> progress, CancellationToken cancel);

        T MergingFiles(IEnumerable<T> file);

        Task<T> MergingFilesAsync(IEnumerable<T> file, IProgress<EGISSOFilesEditProgess> progress, CancellationToken cancel);
    }
}
