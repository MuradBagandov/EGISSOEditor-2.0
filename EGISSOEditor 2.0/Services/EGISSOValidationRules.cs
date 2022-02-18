using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Services
{
    internal struct ValidateRule
    {
        public ValidateRule(Predicate<ValidateArgs> isValidate, params int[] columns)
        {
            IsValidate = isValidate;
            Columns = columns;
            BeforeValidationAction = null;
            CorrectIfValidateIsInvalid = null;
        }

        public int[] Columns { get; private set; }
        public Action<ValidateArgs> BeforeValidationAction { get; set; }
        public Predicate<ValidateArgs> IsValidate{ get; set; }
        public Predicate<ValidateArgs> CorrectIfValidateIsInvalid { get; set; }
    }

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
        public static List<ValidateRule> ValidationRules = new List<ValidateRule>()
        {
            new ValidateRule((a) => true, 1)
            {
                BeforeValidationAction = (a)=>a.Cell.Value = (a.IndexRow-6).ToString()
            },
            new ValidateRule((a) => a.Value is string str && Regex.IsMatch(str, "9796.000001"), 2)
            {
                CorrectIfValidateIsInvalid = (a) => { a.Cell.Value = "9796.000001"; return true; }
            },
            new ValidateRule((a) => a.Value is string str && CheckSNILS(str), 3,17)
            {
                BeforeValidationAction = SNILSCorrection
            },
            new ValidateRule((a) => a.Value is string str && Regex.IsMatch(str, "^[А-яЁё\\s\\-]{1,100}$"), 4,5,18,19)
            {
                BeforeValidationAction = TrimStringCorrection
            },
            new ValidateRule((a) => a.Value is string str && Regex.IsMatch(str, "(^[А-яЁё\\s\\-]{1,100}$)|^$"), 6,20)
            {
                BeforeValidationAction = TrimStringCorrection
            },
            new ValidateRule((a) => a.Value is string str && Regex.IsMatch(str, "(^Female$|^Male$)"), 7,21)
            {
                BeforeValidationAction = (a)=> { ReplaceSpaceCorrection(a); TrimStringCorrection(a); GenderCaseCorrection(a); }
            },
            new ValidateRule(DateValidate, 8, 22, 33, 34, 35),
            new ValidateRule((a) => a.Value is string str && Regex.IsMatch(str, "^([а-яА-ЯёЁ\\-0-9№(][а-яА-ЯёЁ\\-\\s',.0-9()№\"\\\\/]{1,499})$|^$"), 9,23)
            {
                BeforeValidationAction = TrimStringCorrection
            },
            new ValidateRule((a) => Regex.IsMatch(ConvertObjectDoubleToString(a.Value), "(^\\d{8,11}$)|^$"), 10,24),
            new ValidateRule((a) => Regex.IsMatch(ConvertObjectDoubleToString(a.Value), "(^643$)|^$"), 11,25)
            {
                BeforeValidationAction = ReplaceSpaceCorrection
            },
            new ValidateRule((a) => a.Value is string str && Regex.IsMatch(str, "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$"), 31,32)
            {
                BeforeValidationAction = ReplaceSpaceCorrection
            },
            new ValidateRule((a) => Regex.IsMatch(ConvertObjectDoubleToString(a.Value), "^0$|^1$"), 36)
            {
                BeforeValidationAction = ReplaceSpaceCorrection,
                CorrectIfValidateIsInvalid = (a)=> {a.Cell.Value = "0"; return true; }
            },
            new ValidateRule((a)=>Regex.IsMatch(ConvertObjectDoubleToString(a.Value), "^[1-8]{1}$|^$"), 12,26)
            {
                BeforeValidationAction = ReplaceSpaceCorrection
            },
            new ValidateRule(IdentityDocSeriesValidate, 13,27)
            {
                BeforeValidationAction = ReplaceSpaceCorrection
            },
            new ValidateRule(IdentityDocNumberValidate, 14,28)
            {
                BeforeValidationAction = ReplaceSpaceCorrection
            },
            new ValidateRule(IdentityDocIssuerDateValidate, 15,29)
            {
                BeforeValidationAction = ReplaceSpaceCorrection
            },
            new ValidateRule(IdentityDocIssuerValidate, 16, 30)
            {
                BeforeValidationAction = TrimStringCorrection
            },
            new ValidateRule(AmountsValidate, 38,44,50,56)
        };

        private static void SNILSCorrection(ValidateArgs v)
        {
            if (v.Cell is ExcelRange cell)
            {
                string str = ConvertObjectDoubleToString(cell.Value);
                if (!string.IsNullOrEmpty(str))
                    cell.Value = Regex.Replace(str, @"\D", "");
            }
        }

        private static void GenderCaseCorrection(ValidateArgs v)
        {
            if (v.Cell is ExcelRange cell && cell.Value is string str)
                cell.Value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str); ;
        }

        private static void ReplaceSpaceCorrection(ValidateArgs v)
        {
            if (v.Cell is ExcelRange cell && cell.Value is string str)
                cell.Value = str.Replace(" ", "");
        }

        private static void TrimStringCorrection(ValidateArgs v)
        {
            if (v.Cell is ExcelRange cell && cell.Value is string str)
                cell.Value = str.Trim();
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
            if (a.Value is DateTime date)
                return date < DateTime.Now.AddYears(1) && date > new DateTime(1910, 1, 1);
            if (a.Value is string strValue)
                return Regex.IsMatch(strValue, @"^((0[1-9]|[12]\d)\.(0[1-9]|1[012])|30\.(0[13-9]|1[012])|31\.(0[13578]|1[02]))\.(19|20)\d\d$");
            return false;
        }

        private static bool AmountsValidate(ValidateArgs a)
        {
            if (a.Cell.Value is string str && str == "")
                return true;
            return a.Cell.Style.Numberformat.NumFmtID == 2 && a.Cell.Value is double;
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
                case "4": patternValidate = "^[А-Я]{2}$"; break;
                case "5": patternValidate = "^[IVXLCDM]{1,4}[\\-][А-Я]{2}$"; break;
                case "": patternValidate = "^$"; break;
                default: return true;
            }
            if (rewriteValue)
                a.Cell.Value = strValue;
            var result = Regex.IsMatch(strValue, patternValidate);
            return result;
        }

        private static bool IdentityDocNumberValidate(ValidateArgs a)
        {
            object identityDocType = a.Sheet.Cells[a.IndexRow, a.IndexColumn - 2].Value;
            string strIdentityDocType = identityDocType == null ? string.Empty : ConvertObjectDoubleToString(identityDocType);

            if (!(a.Value is string || a.Value is double))
                return false;
            string strValue = ConvertObjectDoubleToString(a.Value);
            bool rewriteValue = false;

            string patternValidate;
            switch (strIdentityDocType)
            {
                case "1": patternValidate = "^\\d{6}$"; strValue = strValue.PadLeft(6, '0'); rewriteValue = true; break;
                case "2": patternValidate = "^[0-9а-яА-ЯA-Za-z]{1,25}$"; break;
                case "3": patternValidate = "^\\d{7}$"; strValue = strValue.PadLeft(7, '0'); rewriteValue = true; break;
                case "4": patternValidate = "^\\d{7}&"; strValue = strValue.PadLeft(7, '0'); rewriteValue = true; break;
                case "5": patternValidate = "^\\d{6}$"; strValue = strValue.PadLeft(6, '0'); rewriteValue = true; break;
                case "": patternValidate = "^$"; break;
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
                hasDate = Regex.IsMatch(strValue, @"^((0[1-9]|[12]\d)\.(0[1-9]|1[012])|30\.(0[13-9]|1[012])|31\.(0[13578]|1[02]))\.(19|20)\d\d$");

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
                if (string.IsNullOrEmpty(strValue))
                    return string.IsNullOrEmpty(strIdentityDocType);
                bool result = Regex.IsMatch(strValue, "^[а-яА-ЯёЁ\\-0-9№(][а-яА-ЯёЁ\\-\\s',.0-9()№\"\\\\/]{1,499}$");
                return !string.IsNullOrEmpty(strIdentityDocType) && result;
            }
            return false;
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
