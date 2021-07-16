using EGISSOEditor_2._0.Services.Structs;
using Spire.Xls;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Services
{
    internal class ExcelConvertor : Interfaces.IExcelConvertor
    {
        public void XLSToXLSXConvert(string fileName)
        {
            if (!File.Exists(fileName))
                throw new ArgumentException("Указан некорректный путь к файлу", nameof(fileName));
            string fileExtension = Path.GetExtension(fileName);

            if (fileExtension != ".xls" && fileExtension != ".xlsm" && fileExtension != ".xlsb")
                throw new ArgumentException("Некорректное расширение файла", nameof(fileName));

            using (Workbook workbook = new Workbook())
            {
                workbook.LoadFromFile(fileName);
                workbook.SaveToFile(Path.ChangeExtension(fileName, ".xlsx"), ExcelVersion.Version2013);
            }
            File.Delete(fileName);
        }

        public void XLSToXLSXConvert(IEnumerable<string> fileNames)
        {
            foreach (string fileName in fileNames)
                XLSToXLSXConvert(fileName);
        }

        public async Task XLSToXLSXConvertAsync(IEnumerable<string> fileNames, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel)
        {
            await Task.Run(() => XLSToXLSXConvert(fileNames, progress, cancel));
        }

        private void XLSToXLSXConvert(IEnumerable<string> fileNames, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel)
        {
            ProcedureElementsProgessReporter reporter = new ProcedureElementsProgessReporter(progress, "Конвертация файлов из XLS в XLSX", fileNames.Count());
            foreach (string fileName in fileNames)
            {
                cancel.ThrowIfCancellationRequested();
                reporter.CurrentElementName = fileName;
                XLSToXLSXConvert(fileName);
                reporter.ProcessedElements++;
            }
        }
    }
}
