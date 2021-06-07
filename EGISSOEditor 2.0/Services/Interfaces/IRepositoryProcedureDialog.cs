using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Services.Interfaces
{
    internal interface IRepositoryProcedureDialog<T> where T: class
    {

        void Add(IFileRepository<T> repository, string[] files);

        void Remove(IFileRepository<T> repository, IEnumerable<T> elements);

        void Save(IFileRepository<T> repository, IEnumerable<T> elements);

        void SaveAs(IFileRepository<T> repository, T elements, string newDirectory);
    }
}
