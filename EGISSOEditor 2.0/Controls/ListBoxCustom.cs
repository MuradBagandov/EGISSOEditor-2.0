using EGISSOEditor_2._0.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EGISSOEditor_2._0.Controls
{
    class ListBoxCustom: ListBox
    {
        public static readonly DependencyProperty TestProperty =
         DependencyProperty.Register("Test",
             typeof(int), typeof(ListBoxCustom));
             

        public int Test
        {
            get => (int)GetValue(TestProperty);
            set => SetValue(TestProperty, value);
        }



        public static readonly DependencyProperty BindableSelectedItemsProperty =
         DependencyProperty.Register("BindableSelectedItems",
             typeof(ObservableCollection<object>), typeof(ListBoxCustom));

        public ObservableCollection<object> BindableSelectedItems
        {
            get => (ObservableCollection<object>)GetValue(BindableSelectedItemsProperty);
            set => SetValue(BindableSelectedItemsProperty, value);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            Test = SelectedItems.Count;
            BindableSelectedItems = new ObservableCollection<object>((IEnumerable<object>)SelectedItems);
        }

       
    }
}
