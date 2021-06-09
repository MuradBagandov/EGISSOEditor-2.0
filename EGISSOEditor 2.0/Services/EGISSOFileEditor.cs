using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EGISSOEditor_2._0.Models;
using EGISSOEditor_2._0.Services.Structs;

namespace EGISSOEditor_2._0.Services
{
    internal class EGISSOFileEditor : Interfaces.IEGISSOFileEditor<EGISSOFile>
    {
        public bool IsFileEGISSO(string path)
        {
            throw new NotImplementedException();
        }

        public bool IsFileEGISSO(EGISSOFile path)
        {
            throw new NotImplementedException();
        }

        public void CorrectionErrorsFiles(IEnumerable<EGISSOFile> file)
        {
            throw new NotImplementedException();
        }

        public Task CorrectionErrorsFilesAsync(IEnumerable<EGISSOFile> file, IProgress<EGISSOFilesEditProgess> progress, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        public void FilesStyleCorrection(IEnumerable<EGISSOFile> file)
        {
            throw new NotImplementedException();
        }

        public Task FilesStyleCorrectionAsync(IEnumerable<EGISSOFile> file, IProgress<EGISSOFilesEditProgess> progress, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        public EGISSOFile MergingFiles(IEnumerable<EGISSOFile> file)
        {
            throw new NotImplementedException();
        }

        public Task<EGISSOFile> MergingFilesAsync(IEnumerable<EGISSOFile> file, IProgress<EGISSOFilesEditProgess> progress, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }
    }
}
