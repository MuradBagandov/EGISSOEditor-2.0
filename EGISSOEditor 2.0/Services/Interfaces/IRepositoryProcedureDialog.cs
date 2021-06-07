using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Services.Interfaces
{
    internal interface IRepositoryProcedureDialog<T> where T: class
    {
        public IFileRepository<T> Repository { get; set; }

        void Add(string[] files);

        void Remove(IEnumerable<T> elements);

        void Save(IEnumerable<T> elements);

        void SaveAs(T elements, string newDirectory);
    }
}
