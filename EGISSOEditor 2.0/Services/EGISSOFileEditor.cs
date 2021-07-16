using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            if (!File.Exists("Resources\\Шаблон.xlsx"))
            {
                Directory.CreateDirectory("Resources");
                using (FileStream file = File.Create("Resources\\Шаблон.xlsx"))
                {
                    file.Write(Properties.Resources.Шаблон, 0, Properties.Resources.Шаблон.Count());
                }
            }
            _patternPackage = new ExcelPackage(new FileInfo("Resources\\Шаблон.xlsx"));
        }
           
        public bool IsValidateFile(string path)=> ValidateFile(path, default, default);

        public bool IsValidateFile(EGISSOFile path) => IsValidateFile(path.Directory);
        
        public async Task<bool> IsValidateFileAsync(string path, IProgress<float> progress, CancellationToken cancel)=>
             await Task.Run(() => ValidateFile(path, progress, cancel));

        public async Task<bool> IsValidateFileAsync(EGISSOFile path, IProgress<float> progress, CancellationToken cancel)=>
            await Task.Run(() => ValidateFile(path.Directory, progress, cancel));


        public void ValidateFiles(IEnumerable<EGISSOFile> files) => ValidateFiles(files, default, default);


        public async Task ValidateFilesAsync(IEnumerable<EGISSOFile> files, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel) =>
            await Task.Run(() => ValidateFiles(files, progress, cancel));
            

        public void FilesStyleCorrection(IEnumerable<EGISSOFile> files) => FilesStyleCorrection(files, null, default);


        public async Task FilesStyleCorrectionAsync(IEnumerable<EGISSOFile> files, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel) =>
            await Task.Run(() => FilesStyleCorrection(files, progress, cancel));


        public void MergingFiles(IEnumerable<EGISSOFile> files, string mergingFilePath) =>
            MergingFiles(files, mergingFilePath, default, default);
        

        public async Task MergingFilesAsync(IEnumerable<EGISSOFile> files, string mergingFilePath, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel)=>
            await Task.Run(()=>MergingFiles(files, mergingFilePath, progress, cancel));


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
                ExcelWorksheet sheet = filePackage.Workbook.Worksheets.FirstOrDefault();
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

        private void MergingFiles(IEnumerable<EGISSOFile> files, string mergingFilePath, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel)
        {
            ProcedureElementsProgessReporter reporter = new ProcedureElementsProgessReporter(progress, "Объединение файлов", files.Count());
            
            if (files.Any(i => i.Directory == mergingFilePath))
                throw new ArgumentException("Ошибка объединения файлов!", nameof(mergingFilePath));
            if (!Directory.Exists(Path.GetDirectoryName(mergingFilePath)))
                throw new ArgumentException("Указан несуществующий каталог!", nameof(mergingFilePath));

            using (ExcelPackage mergingFile = new ExcelPackage())
            {
                foreach (var sheet in _patternPackage.Workbook.Worksheets)
                    mergingFile.Workbook.Worksheets.Add(sheet.Name, sheet);

                ExcelWorksheet mainMergingFileSheet = mergingFile.Workbook.Worksheets.FirstOrDefault();
                ExcelWorksheet mainMergedFileSheet;
                int offsetsRow = 7;
                cancel.ThrowIfCancellationRequested();
                foreach (EGISSOFile item in files)
                {
                    reporter.CurrentElementName = item.Name;

                    using (ExcelPackage mergedFile = new ExcelPackage(new FileInfo(item.TemplateDirectory)))
                    {
                        mainMergedFileSheet = mergedFile.Workbook.Worksheets.FirstOrDefault();
                        int CountMergedFileRows = RecountRow(mainMergedFileSheet);
                        if (CountMergedFileRows == 0)
                            continue;
                        mainMergedFileSheet.Cells[7, 1, CountMergedFileRows + 6, 56].Copy(mainMergingFileSheet.Cells[offsetsRow, 1]);
                        offsetsRow += CountMergedFileRows;
                    }
                    cancel.ThrowIfCancellationRequested();
                    reporter.ProcessedElements++;
                }
                mergingFile.SaveAs(new FileInfo(mergingFilePath));
            }
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
                    reporter.CurrentElementProgress = 0.2f;
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
                    range.Style.Fill.BackgroundColor.SetColor(255, 255, 255, 255);

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
                    ExcelRange columnRange, cellPattern;
                    for (int column = 1; column <= 56; column++)
                    {
                        columnRange = mainWorkSheet.Cells[7, column, CountMainWorkSheetRow + 6, column];
                        cellPattern = patternWorkSheet.Cells[7, column];
                        var filecell = mainWorkSheet.Cells[7, column];
                        columnRange.Style.Numberformat.Format = cellPattern.Style.Numberformat.Format;
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
                        columnRange.Style.WrapText = cellPattern.Style.WrapText;
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

        private void ValidateFiles(IEnumerable<EGISSOFile> files, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel)
        {
            ProcedureElementsProgessReporter reporter = new ProcedureElementsProgessReporter(progress, "Проверка файла на ошибки", files.Count());
            int countMainSheetRows = 0;

            foreach (EGISSOFile item in files)
            {
                reporter.CurrentElementName = item.Name;
                using (ExcelPackage fileExcel = new ExcelPackage(new FileInfo(item.TemplateDirectory)))
                {
                    cancel.ThrowIfCancellationRequested();
                    ExcelWorksheet mainSheet = fileExcel.Workbook.Worksheets.FirstOrDefault();
                    countMainSheetRows = RecountRow(mainSheet);
                    var range = mainSheet.Cells[7, 1, countMainSheetRows + 6, 56];
                    range.Style.Fill.SetBackground(ExcelIndexedColor.Indexed1);

                    float procesedvalidateItems = 0;
                    foreach (var validateItem in EGISSOValidationRules.ValidationParametrs)
                    {
                        procesedvalidateItems++;
                        cancel.ThrowIfCancellationRequested();
                        reporter.CurrentElementProgress = procesedvalidateItems / EGISSOValidationRules.ValidationParametrs.Count();
                        ColumnsValidate(mainSheet, validateItem.columns,
                            validateItem.isValidate, validateItem.invalidValueEvent, validateItem.beforeValidationAction);
                    }
                    
                    fileExcel.Save();
                }
                item.IsFileChanged = true;
                reporter.ProcessedElements++;
            }

            void ColumnsValidate(ExcelWorksheet sheet, int[] columns, Predicate<ValidateArgs> isValidate, Predicate<ValidateArgs> invalidValueEvent = null, Action<ExcelRange> beforeValidationAction = null)
            {
                for (int row = 7; row <= countMainSheetRows + 6; row++)
                {
                    for (int i = 0; i < columns.Length; i++)
                    {
                        var currentCell = sheet.Cells[row, columns[i]];
                        beforeValidationAction?.Invoke(currentCell);

                        object value = currentCell.Value;
                        if (value == null)
                        {
                            currentCell.Value = string.Empty;
                            value = currentCell.Value;
                        }

                        ValidateArgs validateArgs = new ValidateArgs(value, currentCell, sheet, row, columns[i]);

                        if (!(isValidate?.Invoke(validateArgs) ?? true))
                        {
                            if (!(invalidValueEvent?.Invoke(validateArgs) ?? false))
                            {
                                currentCell.Style.Fill.SetBackground(ExcelIndexedColor.Indexed29);
                            }
                        }
                    }
                }
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
