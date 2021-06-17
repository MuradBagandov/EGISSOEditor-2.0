using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EGISSOEditor_2._0.Services.Structs;

namespace EGISSOEditor_2._0.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        private static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(
            nameof(Value), typeof(float), typeof(ProgressWindow));

        public float Value
        {
            get => (float)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static readonly DependencyProperty ProcessNameProperty =
            DependencyProperty.Register(
            nameof(ProcessName), typeof(string), typeof(ProgressWindow));

        public string ProcessName
        {
            get => (string)GetValue(ProcessNameProperty);
            set => SetValue(ProcessNameProperty, value);
        }

        private static readonly DependencyProperty ProcessCurrentFileProperty =
            DependencyProperty.Register(
            nameof(ProcessCurrentFile), typeof(string), typeof(ProgressWindow));

        public string ProcessCurrentFile
        {
            get => (string)GetValue(ProcessCurrentFileProperty);
            set => SetValue(ProcessCurrentFileProperty, value);
        }

        private static readonly DependencyProperty ProcessedFilesProperty =
            DependencyProperty.Register(
            nameof(ProcessedFiles), typeof(string), typeof(ProgressWindow));

        public string ProcessedFiles
        {
            get => (string)GetValue(ProcessedFilesProperty);
            set => SetValue(ProcessedFilesProperty, value);
        }

        public bool IsCancel { get; set; }

        private IProgress<ProcedureElementsProgessReporter> _progress;
        public IProgress<ProcedureElementsProgessReporter> Progress => _progress ??= new Progress<ProcedureElementsProgessReporter>((v) =>
        {
            Value = v.TotalElementsProgress;
            ProcessName = v.ProcessName;
            ProcessCurrentFile = v.CurrentElementName;
            ProcessedFiles = $"{v.ProcessedElements} из {v.TotalElements}";

            if (v.IsEndOfProcessed)
                this.Close();
        });

        private CancellationTokenSource _cancellationSource;

        public CancellationToken Cancellation
        {
            get
            {
                if (_cancellationSource == null)
                    _cancellationSource = new CancellationTokenSource();
                return _cancellationSource.Token;
            }
        }

        public ProgressWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _cancellationSource?.Cancel();

            if (!IsCancel)
            {
                ProcessName = "Отмена...";
                progressbar.IsIndeterminate = true;
                e.Cancel = true;
            }
        }
    }
}
