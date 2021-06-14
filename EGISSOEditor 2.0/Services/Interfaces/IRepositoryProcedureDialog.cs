using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EGISSOEditor_2._0.Services.Structs;

namespace EGISSOEditor_2._0.Services.Interfaces
{
    internal interface IRepositoryProcedureDialog<T> where T: class
    {
        public IFileRepository<T> Repository { get; set; }

        void Add(string[] files);

        Task AddWithShowProgressAsync(string[] files);

        Task AddAsync(string[] files,IProgress<ProcedureFilesProgess> progress,  CancellationToken cancel);

        void Remove(IEnumerable<T> elements);

        Task RemoveAsync(IEnumerable<T> elements, IProgress<ProcedureFilesProgess> progress, CancellationToken cancel);

        Task RemoveWithShowProgressAsync(IEnumerable<T> elements);

        void Save(IEnumerable<T> elements);

        Task SaveAsync(IEnumerable<T> elements, IProgress<ProcedureFilesProgess> progress, CancellationToken cancel);

        Task SaveWithShowProgressAsync(IEnumerable<T> elements);

        void SaveAs(T element, string newDirectory);
    }
}
