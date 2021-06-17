using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Services.Structs
{
    public struct ProcedureElementsProgessReporter
    {
        public string ProcessName { get; }

        private string _currentElementName;
        public string CurrentElementName 
        { 
            get => _currentElementName; 
            set
            {
                _currentElementName = value;
                Changed();
            } 
        }

        private int _processedElements;
        public int ProcessedElements
        {
            get => _processedElements;
            set
            {
                _processedElements = value;
                Changed();
            }
        }

        public int TotalElements { get; }

        private int _currentElementProgress;
        public int CurrentElementProgress
        {
            get => _currentElementProgress;
            set
            {
                _currentElementProgress = value;
                Changed();
            }
        }
        public float TotalElementsProgress => (float)ProcessedElements / TotalElements + 1f/ TotalElements * CurrentElementProgress;
        public bool IsEndOfProcessed => ProcessedElements >= TotalElements;

        private IProgress<ProcedureElementsProgessReporter> _reporter;

        public ProcedureElementsProgessReporter(IProgress<ProcedureElementsProgessReporter> reporter, string processName, int totalElement)
        {
            _currentElementName = default;
            _processedElements = default;
            _currentElementProgress = default;

            _reporter = reporter;
            ProcessName = processName;
            TotalElements = totalElement;
            Changed();
        }

        private void Changed()
        {
            _reporter?.Report(this);
        }
    }
}
