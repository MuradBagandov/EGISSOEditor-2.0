using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using EGISSOEditor_2._0.ViewModels.Base;

namespace EGISSOEditor_2._0.Models
{
    internal class EGISSOFile: ViewModel
    {
        public int Id { get;  }

        private string _directory;
        public string Directory {
            get => _directory;
            set 
            { 
                Set(ref _directory, value);
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Location));
            }
        }

        public string TemplateDirectory { get; }

        public string Name => Path.GetFileName(Directory);
        public string Location => Path.GetDirectoryName(Directory);

        public string Status => IsFileChanged == true ? "Изменен" : "Без изменений";

        private bool _isFileChanged;
        public bool IsFileChanged 
        {
            get => _isFileChanged;
            set 
            {
                Set(ref _isFileChanged, value);
                OnPropertyChanged(nameof(Status));
            }
        }

        public EGISSOFile(int id, string directory, string templateDirectory)
        {
            Id = id;
            Directory = directory;
            TemplateDirectory = templateDirectory;
        }

    }
}
