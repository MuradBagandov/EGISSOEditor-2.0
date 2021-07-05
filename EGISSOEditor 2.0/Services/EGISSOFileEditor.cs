using System;
using System.Collections.Generic;
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


        public void ValidateFiles(IEnumerable<EGISSOFile> files)
        {
            int CountMainSheetRows = 0;

            var validationParametrs =
                new List<(int[] columns, Action<ExcelRange> beforeValidationAction, Predicate<object> isValidate, Predicate<ExcelRange> invalidValueEvent)>()
                {
                    (new int[]{2}, null, (v) => v is string str && Regex.IsMatch(str, "9796.00001"), (v) => { v.Value = "9796.00001"; return true; }),
                    (new int[]{3,17}, SNILSCorrection, (v) => v is string str && CheckSNILS(str), null),
                    (new int[]{4,5,18,19}, null,(v) => v is string str && Regex.IsMatch(str, "^[А-яЁё\\s\\-]{1,100}$"), null),
                    (new int[]{6,20}, null,(v) => v is string str && Regex.IsMatch(str, "(^[А-яЁё\\s\\-]{1,100}$)|^$"), null),
                    (new int[]{7,21}, null,(v) => v is string str && Regex.IsMatch(str, "(^Female$|^Male$)"), null),
                    (new int[]{8, 22, 33, 34, 35}, null, (v) => v is DateTime, null),
                    (new int[]{9, 23}, null, (v) => v is string str && Regex.IsMatch(str, "([а-яА-ЯёЁ\\-0-9№(][а-яА-ЯёЁ\\-\\s',.0-9()№\"\\\\/]{1,499})|^$"), null),
                    (new int[]{10, 24}, null, (v) => Regex.IsMatch(ConvertObjectDoubleToString(v), "(^\\d{8,11}$)|^$"), null),
                    (new int[]{11, 25},null, (v) => Regex.IsMatch(ConvertObjectDoubleToString(v), "(^\\d{1,3}$)|^$"), null),
                    (new int[]{31, 32}, null,(v) => v is string str && Regex.IsMatch(str, "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$"), null),
                    (new int[]{36}, null, (v) => Regex.IsMatch(ConvertObjectDoubleToString(v), "0|1"), (v)=> {v.Value = "0"; return true; })

                };

            foreach (EGISSOFile item in files)
            {
                using (ExcelPackage fileExcel = new ExcelPackage(new FileInfo(item.TemplateDirectory)))
                {
                    ExcelWorksheet mainSheet = fileExcel.Workbook.Worksheets.FirstOrDefault();
                    CountMainSheetRows = RecountRow(mainSheet);
                    var range = mainSheet.Cells[7, 1, CountMainSheetRows + 6, 56];
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(255, 255, 255, 255);


                    foreach(var validateItem in validationParametrs)
                        ColumnsValidate(mainSheet, validateItem.columns, 
                            validateItem.isValidate, validateItem.invalidValueEvent, validateItem.beforeValidationAction);
                    

                    fileExcel.Save();
                }
            }

            void ColumnsValidate(ExcelWorksheet sheet, int[] columns, Predicate<object> isValidate, Predicate<ExcelRange> invalidValueEvent = null, Action<ExcelRange> beforeValidationAction = null)
            {
                for (int row = 7; row <= CountMainSheetRows + 6; row++)
                {
                    for(int i = 0; i< columns.Length; i++)
                    {
                        var currentCell = sheet.Cells[row, columns[i]];
                        beforeValidationAction?.Invoke(currentCell);

                        object value = currentCell.Value;
                        if (value == null)
                        {
                            currentCell.Value = string.Empty;
                            value = currentCell.Value;
                        }

                        if (!(isValidate?.Invoke(value) ?? true))
                        {
                            if (!(invalidValueEvent?.Invoke(currentCell) ?? false))
                            {
                                currentCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                currentCell.Style.Fill.BackgroundColor.SetColor(255, 255, 83, 83);
                            }
                        }
                    }
                }
            }

            void SNILSCorrection(ExcelRange v)
            {
                if (v is ExcelRange cell && cell.Value is string str)
                    cell.Value = Regex.Replace(str, @"\D", "");
            }

            string ConvertObjectDoubleToString(object v)
            {
                if (v is double dValue)
                    return dValue.ToString();
                else if (v is string strValue)
                    return strValue;
                return string.Empty;
            }
        }

        public Task ValidateFilesAsync(IEnumerable<EGISSOFile> files, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

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

        private bool CheckSNILS(string value)
        {
            value = value.Replace("-", "");

            if (Regex.IsMatch(value, @"^\d{9,11}$"))
                value = value.PadLeft(11, '0');
            else
                return false;

            if (int.Parse(value.Substring(0, 9)) < 1001998)
                return false;

            int controlNumber = 0;

            for (int i = 0; i < 9; i++)
                controlNumber += (9 - i) * int.Parse(value[i] + "");

            if (controlNumber > 100) controlNumber %= 101;
            if (controlNumber == 100) controlNumber = 0;
            return controlNumber == int.Parse(value.Substring(9, 2));
        }

    }
}
