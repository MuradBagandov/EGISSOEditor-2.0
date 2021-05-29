using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EGISSOEditor_2._0.Models;
using System.Collections.ObjectModel;

namespace EGISSOEditor_2._0.Services
{
    internal class EGISSOFileRepository : Interfaces.IFileRepository<EGISSOFile>
    {
        public ObservableCollection<EGISSOFile> Items => _items;

        private ObservableCollection<EGISSOFile> _items = new ObservableCollection<EGISSOFile>();

        private static int _id = 0;

        private readonly string _directoryTemplate;
        public EGISSOFileRepository()
        {
            _directoryTemplate = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\temp";
            if (!Directory.Exists(_directoryTemplate))
                Directory.CreateDirectory(_directoryTemplate);
        }

        public bool Add(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("Файл не найден!");

            if (_items.Any(i => i.Directory == path))
                throw new ArgumentException("Данный файл уже добавлен!");

            if (path.IndexOf(_directoryTemplate) != -1)
                throw new ArgumentException("Данный файл нельзя добавить!");

            string name = Path.GetFileName(path);
            string tempPath = _directoryTemplate + "\\" + _id.ToString();

            if (File.Exists(tempPath))
            {
                try { File.Delete(tempPath); }
                catch { throw new Exception("Ошибка добавления файла"); }
            }

            File.Copy(path, tempPath);
            _items.Add(new EGISSOFile(_id++, path, tempPath, name));
            return true;
        }

        public bool Remove(EGISSOFile element)
        {
            File.Delete(element.TemplateDirectory);
            return _items.Remove(element);
        }

        public void RemoveAll()
        {
            foreach (EGISSOFile item in _items)
                File.Delete(item.TemplateDirectory);
            _items.Clear();
        }

        public void Save(EGISSOFile element)
        {
            File.Copy(element.TemplateDirectory, element.Directory, true);
            element.IsFileChanged = false;
        }

        public void SaveAs(EGISSOFile element, string newDirectory)
        {
            if (Items.Any(i => i.Directory == newDirectory || i.TemplateDirectory == newDirectory) || 
                newDirectory.IndexOf(_directoryTemplate) != -1)
                throw new ArgumentException($"{newDirectory} не корректный путь или текущий объект уже добален!");

            element.Directory = newDirectory;
            Save(element);
        }

        public void SaveAll()
        {
            foreach (EGISSOFile item in _items)
                Save(item);
        }

        
    }
}
