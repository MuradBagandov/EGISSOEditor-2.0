using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Infrastuctures.Enums
{
    internal enum GroupingType
    {
        [Description("Без группировки")]
        NoGrouping,
        [Description("Директория")]
        Location,
        [Description("Изменения")]
        Status
    }

    internal enum SortingType
    {
        [Description("Без сортировки")]
        NoSorting,
        [Description("Имя")]
        ByName,
        [Description("Изменения")]
        ByStatus
    }
}
