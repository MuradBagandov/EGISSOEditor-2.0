using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EGISSOEditor_2._0
{
    public enum ThemeTypes
    {
        Light,
        Dark
    }

    internal static class ThemeContoller
    {
        private static ThemeTypes _currentTheme = ThemeTypes.Dark;
        public static ThemeTypes CurrentTheme 
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                ChangeTheme();
            }
        }

        private static void ChangeTheme()
        {
            string themeName = default;
            switch (CurrentTheme)
            {
                case ThemeTypes.Dark: themeName = "DarkTheme"; break;
                case ThemeTypes.Light: themeName = "LightTheme"; break;

            }

            Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
            {
                Source = new Uri($"Resources/Styles/{themeName}.xaml", UriKind.Relative)
            };

            Properties.Settings.Default.Theme = (byte)CurrentTheme;
            Properties.Settings.Default.Save();
        }
    }
}
