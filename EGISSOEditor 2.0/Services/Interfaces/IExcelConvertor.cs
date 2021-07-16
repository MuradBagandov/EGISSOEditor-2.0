using EGISSOEditor_2._0.Services.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Services.Interfaces
{
    internal interface IExcelConvertor
    {
        void XLSToXLSXConvert(string fileName);
        void XLSToXLSXConvert(IEnumerable<string> fileNames);
        Task XLSToXLSXConvertAsync(IEnumerable<string> fileNames, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel);
    }
}
