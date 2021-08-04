using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EGISSOEditor_2._0.Infrastuctures.Enums;

namespace EGISSOEditor_2._0
{
    internal static class ApplicationSettings
    {
        public static SortingType CurrentSortingType
        {
            get => (SortingType)Properties.Settings.Default.SortingType;
            set => Properties.Settings.Default.SortingType = (byte)value;
        }

        public static GroupingType CurrentGroupingType
        {
            get => (GroupingType)Properties.Settings.Default.GroupingType;
            set => Properties.Settings.Default.GroupingType = (byte)value;
        }

        public static bool SortDescending
        {
            get => Properties.Settings.Default.SortDescending;
            set => Properties.Settings.Default.SortDescending = value;
        }

        public static bool ListViewStyle
        {
            get => Properties.Settings.Default.ListViewStyle;
            set => Properties.Settings.Default.ListViewStyle = value;
        }

        public static ThemeType CurrentTheme
        {
            get => (ThemeType)Properties.Settings.Default.Theme;
            set => Properties.Settings.Default.Theme = (byte)value;
        }

        public static Size MainWindowSize
        {
            get => Properties.Settings.Default.MainWindowSize;
            set => Properties.Settings.Default.MainWindowSize = value;
        }

        public static Size MainWindowStartupLocation
        {
            get => Properties.Settings.Default.MainWindowStartupLocation;
            set => Properties.Settings.Default.MainWindowStartupLocation = value;
        }

        public static bool IsMainWindowMaximized
        {
            get => Properties.Settings.Default.MainWindowMaximized;
            set => Properties.Settings.Default.MainWindowMaximized = value;
        }

        static ApplicationSettings()
        {
            Properties.Settings.Default.PropertyChanged += (s,e)=> Properties.Settings.Default.Save();
        }
    }
}
