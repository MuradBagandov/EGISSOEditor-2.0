using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EGISSOEditor_2._0.Models;
using EGISSOEditor_2._0.Services.Structs;
using OfficeOpenXml;
using OfficeOpenXml.Style;


namespace EGISSOEditor_2._0.Services
{
    internal class EGISSOFileEditor : Interfaces.IEGISSOFileEditor<EGISSOFile>, IDisposable
    {
        private ExcelPackage _patternPackage;
        private bool _disposed;

        public EGISSOFileEditor()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (File.Exists("Resources\\Шаблон.xlsx"))
                _patternPackage = new ExcelPackage(new FileInfo("Resources\\Шаблон.xlsx"));
        }

        public bool IsValidateFile(string path)=> ValidateFile(path, default, default);

        public bool IsValidateFile(EGISSOFile path) => IsValidateFile(path.Directory);
        
        public async Task<bool> IsValidateFileAsync(string path, IProgress<float> progress, CancellationToken cancel)=>
             await Task.Run(() => ValidateFile(path, progress, cancel));

        public async Task<bool> IsValidateFileAsync(EGISSOFile path, IProgress<float> progress, CancellationToken cancel)=>
            await Task.Run(() => ValidateFile(path.Directory, progress, cancel));


        public void ValidateFiles(IEnumerable<EGISSOFile> file)
        {
            throw new NotImplementedException();
        }

        public Task ValidateFilesAsync(IEnumerable<EGISSOFile> file, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        public void FilesStyleCorrection(IEnumerable<EGISSOFile> files) => FilesStyleCorrection(files, null, default);


        public async Task FilesStyleCorrectionAsync(IEnumerable<EGISSOFile> files, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel) =>
            await Task.Run(() => FilesStyleCorrection(files, progress, cancel));
        

        public EGISSOFile MergingFiles(IEnumerable<EGISSOFile> file)
        {
            throw new NotImplementedException();
        }

        public Task<EGISSOFile> MergingFilesAsync(IEnumerable<EGISSOFile> file, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        ~EGISSOFileEditor()
        {
            if (!_disposed)
                Dispose();
        }

        public void Dispose()
        {
            _patternPackage.Dispose();
            _disposed = true;
        }

        private bool ValidateFile(string path, IProgress<float> progress, CancellationToken cancel)
        {
            if (_patternPackage == null)
                return false;

            if (!File.Exists(path))
                throw new ArgumentException("Файл не найден");

            string fileExtension = Path.GetExtension(path);
            if (!fileExtension.Equals(".xlsx"))
                throw new ArgumentException("Некорректный тип файла", nameof(path));
         
            int differentes = 0;

            using (ExcelPackage filePackage = new ExcelPackage(new FileInfo(path)))
            {
                var sheet = filePackage.Workbook.Worksheets.FirstOrDefault();
                var patternSheet = _patternPackage.Workbook.Worksheets.FirstOrDefault();
                object cellValue, patternCellValue;

                for (int column = 1; column <= 53; column++)
                {
                    cellValue = sheet.Cells[4, column].Value;
                    patternCellValue = patternSheet.Cells[4, column].Value;

                    if (cellValue == null && patternCellValue == null)
                        continue;

                    if ((cellValue == null && patternCellValue != null) ||
                        (cellValue != null && patternCellValue == null))
                    {
                        differentes++;
                        continue;
                    }


                    if (!cellValue.Equals(patternCellValue))
                        differentes++;
                    progress?.Report(column / 53.0f); 
                    cancel.ThrowIfCancellationRequested();
                }
            }
            progress?.Report(1.0f);
            return differentes < 40;
        }


        private void FilesStyleCorrection(IEnumerable<EGISSOFile> files, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel)
        {
            ProcedureElementsProgessReporter reporter = new ProcedureElementsProgessReporter(progress, "Корректировка шаблона", files.Count());
            ExcelWorkbook itemWorkBook;
            foreach (EGISSOFile item in files)
            {
                reporter.CurrentElementName = item.Name;
                cancel.ThrowIfCancellationRequested();
                using (ExcelPackage filePackage = new ExcelPackage(new FileInfo(item.TemplateDirectory)))
                {
                    reporter.CurrentElementProgress = 0.1f;
                    cancel.ThrowIfCancellationRequested();
                    ExcelWorksheet mainWorkSheet;
                    itemWorkBook = filePackage.Workbook;
                    int CountMainWorkSheetRow;

                    if (filePackage.Workbook.Worksheets.Count == 0)
                        continue;

                    mainWorkSheet = itemWorkBook.Worksheets.FirstOrDefault(i => i.Name == "Формат");
                    if (mainWorkSheet == null)
                        mainWorkSheet = itemWorkBook.Worksheets.FirstOrDefault();

                    CountMainWorkSheetRow = RecountRow(mainWorkSheet);

                    var range = mainWorkSheet.Cells[7, 1, CountMainWorkSheetRow + 6, 56];
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(255, 240, 240, 240);

                    reporter.CurrentElementProgress = 0.4f;
                    cancel.ThrowIfCancellationRequested();
                    filePackage.Save();

                    if (_patternPackage == null)
                        continue;

                    mainWorkSheet.Name = "TempFormat";

                    var newMainWorkSheet = itemWorkBook.Worksheets.Add("Формат", _patternPackage.Workbook.Worksheets[0]);
                    itemWorkBook.Worksheets.MoveToStart("Формат");

                    mainWorkSheet.Cells[7, 1, CountMainWorkSheetRow + 6, 56].Copy(newMainWorkSheet.Cells[7, 1]);
                    itemWorkBook.Worksheets.Delete(mainWorkSheet);
                    mainWorkSheet = newMainWorkSheet;
                    reporter.CurrentElementProgress = 0.6f;
                    var patternWorkSheet = _patternPackage.Workbook.Worksheets.FirstOrDefault();
                    for (int column = 1; column <= 56; column++)
                    {
                        var columnRange = mainWorkSheet.Cells[7, column, CountMainWorkSheetRow + 6, column];
                        var cellPattern = patternWorkSheet.Cells[7, column];
                        columnRange.Style.HorizontalAlignment = cellPattern.Style.HorizontalAlignment;
                        columnRange.Style.VerticalAlignment = cellPattern.Style.VerticalAlignment;
                        columnRange.Style.Font.Bold = cellPattern.Style.Font.Bold;
                        columnRange.Style.Font.Charset = cellPattern.Style.Font.Charset;
                        columnRange.Style.Font.Family = cellPattern.Style.Font.Family;
                        columnRange.Style.Font.Italic = cellPattern.Style.Font.Italic;
                        columnRange.Style.Font.Name = cellPattern.Style.Font.Name;
                        columnRange.Style.Font.Scheme = cellPattern.Style.Font.Scheme;
                        columnRange.Style.Font.Size = cellPattern.Style.Font.Size;
                        columnRange.Style.Font.Strike = cellPattern.Style.Font.Strike;
                        columnRange.Style.Font.UnderLine = cellPattern.Style.Font.UnderLine;
                        columnRange.Style.Font.UnderLineType = cellPattern.Style.Font.UnderLineType;
                        columnRange.Style.Font.VerticalAlign = cellPattern.Style.Font.VerticalAlign;
                        mainWorkSheet.Column(column).Width = patternWorkSheet.Column(column).Width;
                    }
                    reporter.CurrentElementProgress = 0.8f;
                    for (int row = 7; row <= CountMainWorkSheetRow + 6; row++)
                        mainWorkSheet.Row(row).Height = patternWorkSheet.Row(7).Height;

                    filePackage.Save();
                }

                item.IsFileChanged = true;
                reporter.ProcessedElements++;
            }
        }


        /// <summary>
        /// Вычисляет количество строк в файле, без строк заголовков
        /// </summary>
        /// <param name="sheet">Лист Excel</param>
        /// <returns>Количество строк</returns>
        private int RecountRow(ExcelWorksheet sheet)
        {
            int countRows = 0;
            while (true)
            {
                object value = sheet.Cells[countRows + 7, 1].Value;
                if (value == null)
                {
                    if (isRowEmpty(countRows + 7))
                        break;
                }

                countRows++;
            }

            return countRows;

            bool isRowEmpty(int row)
            {
                object value;
                for (int i = 2; i < 10; i++)
                {
                    value = sheet.Cells[row, i].Value;
                    if (value != null)
                        return false;
                }
                return true;
            }
        }
    }
}
