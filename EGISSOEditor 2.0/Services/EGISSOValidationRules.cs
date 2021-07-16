using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Services
{
    internal struct ValidateArgs
    {
        public ValidateArgs(object value, ExcelRange cell, ExcelWorksheet sheet, int indexRow, int indexColumn)
        {
            Value = value;
            Cell = cell;
            Sheet = sheet;
            IndexRow = indexRow;
            IndexColumn = indexColumn;
        }

        public object Value { get; private set; }
        public ExcelRange Cell { get; private set; }
        public ExcelWorksheet Sheet { get; private set; }
        public int IndexRow { get; private set; }
        public int IndexColumn { get; private set; }
    }

    internal static class EGISSOValidationRules
    {
        public static List<(int[] columns, Action<ExcelRange> beforeValidationAction, Predicate<ValidateArgs> isValidate, Predicate<ValidateArgs> invalidValueEvent)> ValidationParametrs =>
                new List<(int[] columns, Action<ExcelRange> beforeValidationAction, Predicate<ValidateArgs> isValidate, Predicate<ValidateArgs> invalidValueEvent)>()
                {
                    (new int[]{1}, null, (v) => false, (v) => { v.Cell.Value = (v.IndexRow-6).ToString(); return true; }),
                    (new int[]{2}, null, (v) => v.Value is string str && Regex.IsMatch(str, "9796.00001"), (v) => { v.Cell.Value = "9796.00001"; return true; }),
                    (new int[]{3,17}, SNILSCorrection, (v) => v.Value is string str && CheckSNILS(str), null),
                    (new int[]{4,5,18,19}, null,(v) => v.Value is string str && Regex.IsMatch(str, "^[А-яЁё\\s\\-]{1,100}$"), null),
                    (new int[]{6,20}, null,(v) => v.Value is string str && Regex.IsMatch(str, "(^[А-яЁё\\s\\-]{1,100}$)|^$"), null),
                    (new int[]{7,21}, ReplaceSpaceCorrection,(v) => v.Value is string str && Regex.IsMatch(str, "(^Female$|^Male$)"), null),
                    (new int[]{8, 22, 33, 34, 35}, null, DateValidate, null),
                    (new int[]{9, 23}, null, (v) => v.Value is string str && Regex.IsMatch(str, "([а-яА-ЯёЁ\\-0-9№(][а-яА-ЯёЁ\\-\\s',.0-9()№\"\\\\/]{1,499})|^$"), null),
                    (new int[]{10, 24}, null, (v) => Regex.IsMatch(ConvertObjectDoubleToString(v.Value), "(^\\d{8,11}$)|^$"), null),
                    (new int[]{11, 25},ReplaceSpaceCorrection, (v) => Regex.IsMatch(ConvertObjectDoubleToString(v.Value), "(^\\d{1,3}$)|^$"), null),
                    (new int[]{31, 32}, ReplaceSpaceCorrection, (v) => v.Value is string str && Regex.IsMatch(str, "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$"), null),
                    (new int[]{36}, ReplaceSpaceCorrection, (v) => Regex.IsMatch(ConvertObjectDoubleToString(v.Value), "^0$|^1$"), (v)=> {v.Cell.Value = "0"; return true; }),
                    (new int[]{12,26}, ReplaceSpaceCorrection, (v)=>Regex.IsMatch(ConvertObjectDoubleToString(v.Value), "^[1-8]{1}$"), null),
                    (new int[]{13,27}, ReplaceSpaceCorrection, IdentityDocSeriesValidate, null),
                    (new int[]{14,28}, ReplaceSpaceCorrection, IdentityDocNumberValidate, null),
                    (new int[]{15,29}, ReplaceSpaceCorrection, IdentityDocIssuerDateValidate, null),
                    (new int[]{16,30}, null, IdentityDocIssuerValidate, null),
                };

        private static void SNILSCorrection(ExcelRange v)
        {
            if (v is ExcelRange cell)
            {
                string str = ConvertObjectDoubleToString(cell.Value);
                if (!string.IsNullOrEmpty(str))
                    cell.Value = Regex.Replace(str, @"\D", "");
            }
        }

        private static void ReplaceSpaceCorrection(ExcelRange v)
        {
            if (v is ExcelRange cell && cell.Value is string str)
                cell.Value = str.Replace(" ", "");
        }

        private static string ConvertObjectDoubleToString(object v)
        {
            if (v is double dValue)
                return dValue.ToString();
            else if (v is string strValue)
                return strValue;
            return string.Empty;
        }

        private static bool DateValidate(ValidateArgs a)
        {
            if (a.Value is DateTime)
                return true;
            if (a.Value is string strValue)
                return Regex.IsMatch(strValue, @"((0[1-9]|[12]\d)\.(0[1-9]|1[012])|30\.(0[13-9]|1[012])|31\.(0[13578]|1[02]))\.(19|20)\d\d");
            return false;
        }

        private static bool IdentityDocSeriesValidate(ValidateArgs a)
        {
            object identityDocType = a.Sheet.Cells[a.IndexRow, a.IndexColumn - 1].Value;
            string strIdentityDocType = identityDocType == null ? string.Empty : ConvertObjectDoubleToString(identityDocType);

            if (!(a.Value is string || a.Value is double))
                return false;
            string strValue = ConvertObjectDoubleToString(a.Value);
            string patternValidate = default;
            bool rewriteValue = false;

            switch (strIdentityDocType)
            {
                case "1": patternValidate = "^\\d{4}$"; strValue = strValue.PadLeft(4, '0'); rewriteValue = true; break;
                case "2": patternValidate = "^.{1,20}$"; break;
                case "3": patternValidate = "^\\d{2}$"; break;
                case "4": patternValidate = "^[А-Я]{2}&"; break;
                case "5": patternValidate = "^[IVXLCDM]{1,4}[\\-][А-Я]{2}$"; break;
                case "": patternValidate = "^&"; break;
                default: return true;
            }
            if (rewriteValue)
                a.Cell.Value = strValue;
            return Regex.IsMatch(strValue, patternValidate);
        }

        private static bool IdentityDocNumberValidate(ValidateArgs a)
        {
            object identityDocType = a.Sheet.Cells[a.IndexRow, a.IndexColumn - 2].Value;
            string strIdentityDocType = identityDocType == null ? string.Empty : ConvertObjectDoubleToString(identityDocType);

            if (!(a.Value is string || a.Value is double))
                return false;
            string strValue = ConvertObjectDoubleToString(a.Value);
            string patternValidate = default;
            bool rewriteValue = false;


            switch (strIdentityDocType)
            {
                case "1": patternValidate = "^\\d{6}$"; strValue = strValue.PadLeft(6, '0'); rewriteValue = true; break;
                case "2": patternValidate = "^[0-9а-яА-ЯA-Za-z]{1,25}$"; break;
                case "3": patternValidate = "^\\d{7}$"; strValue = strValue.PadLeft(7, '0'); rewriteValue = true; break;
                case "4": patternValidate = "^\\d{7}&"; strValue = strValue.PadLeft(7, '0'); rewriteValue = true; break;
                case "5": patternValidate = "^\\d{6}$"; strValue = strValue.PadLeft(6, '0'); rewriteValue = true; break;
                case "": patternValidate = "^&"; break;
                default: return true;
            }
            if (rewriteValue)
                a.Cell.Value = strValue;
            return Regex.IsMatch(strValue, patternValidate);
        }

        private static bool IdentityDocIssuerDateValidate(ValidateArgs a)
        {
            object identityDocType = a.Sheet.Cells[a.IndexRow, a.IndexColumn - 3].Value;
            string strIdentityDocType = identityDocType == null ? string.Empty : ConvertObjectDoubleToString(identityDocType);

            bool hasDate = a.Value is DateTime;
            if (!hasDate && a.Value is string strValue)
                hasDate = Regex.IsMatch(strValue, @"((0[1-9]|[12]\d)\.(0[1-9]|1[012])|30\.(0[13-9]|1[012])|31\.(0[13578]|1[02]))\.(19|20)\d\d");

            if (string.IsNullOrEmpty(strIdentityDocType) && hasDate)
                return false;
            if (!string.IsNullOrEmpty(strIdentityDocType) && !hasDate)
                return false;
            return true;
        }

        private static bool IdentityDocIssuerValidate(ValidateArgs a)
        {
            object identityDocType = a.Sheet.Cells[a.IndexRow, a.IndexColumn - 4].Value;
            string strIdentityDocType = identityDocType == null ? string.Empty : ConvertObjectDoubleToString(identityDocType);

            if (a.Value is string strValue)
            {
                return !string.IsNullOrEmpty(strIdentityDocType) && Regex.IsMatch(strValue, "[а-яА-ЯёЁ\\-0-9№(][а-яА-ЯёЁ\\-\\s',.0-9()№\"\\\\/]{1,499}");
            }
            else
            {
                return a.Value == null && string.IsNullOrEmpty(strIdentityDocType);
            }
        }

        private static bool CheckSNILS(string value)
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
