using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EGISSOEditor_2._0.Services.Structs;

namespace EGISSOEditor_2._0.Services.Interfaces
{
    interface IEGISSOFileEditor<T> where T: class
    {
        bool IsValidateFile(string path);

        bool IsValidateFile(T path);

        Task<bool> IsValidateFileAsync(string path, IProgress<float> progress, CancellationToken cancel);

        Task<bool> IsValidateFileAsync(T path, IProgress<float> progress, CancellationToken cancel);

        void FilesStyleCorrection(IEnumerable<T> file);

        Task FilesStyleCorrectionAsync(IEnumerable<T> file, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel);

        void ValidateFiles(IEnumerable<T> file);

        Task ValidateFilesAsync(IEnumerable<T> file, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel);

        void MergingFiles(IEnumerable<T> file, string mergingFilePath);

        Task MergingFilesAsync(IEnumerable<T> file, string mergingFilePath, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel);
    }
}
