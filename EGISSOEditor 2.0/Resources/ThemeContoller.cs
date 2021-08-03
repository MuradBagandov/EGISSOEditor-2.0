using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EGISSOEditor_2._0
{
    public enum ThemeType
    {
        Light,
        Dark
    }

    internal static class ThemeContoller
    {
        public static ThemeType CurrentTheme 
        {
            get => ApplicationSettings.CurrentTheme;
            set
            {
                ApplicationSettings.CurrentTheme = value;
                ChangeTheme();
            }
        }

        private static void ChangeTheme()
        {
            string themeName = default;
            switch (CurrentTheme)
            {
                case ThemeType.Dark: themeName = "DarkTheme"; break;
                case ThemeType.Light: themeName = "LightTheme"; break;
            }

            Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
            {
                Source = new Uri($"Resources/Styles/{themeName}.xaml", UriKind.Relative)
            };
        }
    }
}
