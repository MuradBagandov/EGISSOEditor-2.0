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
    internal class EGISSOFileEditor : Interfaces.IEGISSOFileEditor<EGISSOFile>
    {
        public EGISSOFileEditor()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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

        public Task ValidateFilesAsync(IEnumerable<EGISSOFile> file, IProgress<ProcedureFilesProgess> progress, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        public void FilesStyleCorrection(IEnumerable<EGISSOFile> file)
        {
            throw new NotImplementedException();
        }

        public Task FilesStyleCorrectionAsync(IEnumerable<EGISSOFile> file, IProgress<ProcedureFilesProgess> progress, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        public EGISSOFile MergingFiles(IEnumerable<EGISSOFile> file)
        {
            throw new NotImplementedException();
        }

        public Task<EGISSOFile> MergingFilesAsync(IEnumerable<EGISSOFile> file, IProgress<ProcedureFilesProgess> progress, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        private bool ValidateFile(string path, IProgress<float> progress, CancellationToken cancel)
        {
            if (!File.Exists("Resources\\Шаблон.xlsx"))
                return false;

            if (!File.Exists(path))
                throw new ArgumentException("Файл не найден");

            string fileExtension = Path.GetExtension(path);
            if (!fileExtension.Equals(".xlsx"))
                throw new ArgumentException("Некорректный тип файла", nameof(path));

            FileInfo filePattern = new FileInfo("Resources\\Шаблон.xlsx");
            FileInfo file = new FileInfo(path);
            int differentes = 0;

            using (ExcelPackage filePackage = new ExcelPackage(file))
            {
                using (ExcelPackage patternPackage = new ExcelPackage(filePattern))
                {
                    var sheet = filePackage.Workbook.Worksheets.FirstOrDefault();
                    var patternSheet = patternPackage.Workbook.Worksheets.FirstOrDefault();
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
            }
            progress?.Report(1.0f);
            return differentes < 40;
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
