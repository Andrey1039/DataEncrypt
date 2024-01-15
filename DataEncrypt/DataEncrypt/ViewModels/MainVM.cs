using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;
using DataEncrypt.Models;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using DataEncrypt.Data.Services;
using System.Collections.Generic;
using DataEncrypt.Models.Encryption;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace DataEncrypt.ViewModels
{
    internal class MainVM : INotifyPropertyChanged
    {
        private readonly ObservableCollection<Item> _filePaths;
        private readonly IDialogService _openFilesService;
        private readonly IMessengerService _messengerService;
        private bool _dropInfoVisible;
        private bool _tProcess;
        private bool _tabsEnable;
        private string _startText;
        private string _endText;
        private string _key;

        public MainVM()
        {
            _dropInfoVisible = false;
            _endText = string.Empty;
            _startText = string.Empty;
            _tabsEnable = true;
            _tProcess = false;
            _filePaths = new ObservableCollection<Item>();
            _openFilesService = new DialogService();
            _messengerService = new MessengerService();
            _key = Keys.ReadKey("get");
        }

        // Добавление элементов в список файлов
        private void AddValue(string[] files)
        {
            foreach (string path in files)
                if (Directory.Exists(path))
                {
                    foreach (string newFile in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                    {
                        if (!FilePaths.Any(x => x.FileName == newFile))
                            FilePaths.Add(new Item(newFile, false));
                    }
                }
                else
                    if (!FilePaths.Any(x => x.FileName == path))
                        FilePaths.Add(new Item(path, false));
        }

        // Команда добавления элементов (кнопка)
        private RelayCommand? addItemCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addItemCommand ??= new RelayCommand(_ =>
                    AddValue(_openFilesService.OpenFileDialog("Все файлы (*.*)|*.*", true)));
            }
        }

        // Команда удаления элементов из списка файлов
        private RelayCommand? removeItemCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeItemCommand ??= new RelayCommand(_ =>
                  {
                      List<Item> selectedItems = FilePaths.Where(x => x.IsChecked).ToList();

                      if (selectedItems.Count > 0)
                          for (int i = selectedItems.Count - 1; i >= 0; i--)
                              FilePaths.Remove(selectedItems[i]);
                  });
            }
        }

        // Команда выбора всех элементов
        private RelayCommand? selectAllCommand;
        public RelayCommand SelectAllCommand
        {
            get
            {
                return selectAllCommand ??= new RelayCommand(_ =>
                    FilePaths.Where(x => !x.IsChecked).ToList().ForEach(y => y.IsChecked = true));
            }
        }

        // Обработка файлов
        private bool TransformFile(bool mode)
        {
            byte[] byteKey = Encoding.UTF8.GetBytes(Key);
            Array.Resize(ref byteKey, 32);

            foreach (Item file in FilePaths)
                try
                {
                    if (mode == true)
                        EncryptFile.EncryptFiles(file.FileName, byteKey);
                    else
                        EncryptFile.DecryptFiles(file.FileName, byteKey);
                }
                catch
                {
                    _messengerService.ShowErrorMessageBox(
                            $"Невозможно прочитать файл:\n{file}\n\nФайл будет пропущен");
                }

            return false;
        }

        // Команда шифрования файлов
        private RelayCommand? transformDataCommand;
        public RelayCommand TransformDataCommand
        {
            get
            {
                return transformDataCommand ??= new RelayCommand(mode =>
                  {
                      TProcess = true;
                      Task.Factory.StartNew(() => TProcess = TransformFile((bool)mode));
                  });
            }
        }

        // Добавление элементов в список файлов (Drag&Drop)
        private RelayCommand? dragCommand;
        public RelayCommand DragCommand
        {
            get
            {
                return dragCommand ??= new RelayCommand(param =>
                  {
                      DragEventArgs args = (DragEventArgs)param;

                      if (args.Data.GetDataPresent(DataFormats.FileDrop))
                          AddValue((string[])args.Data.GetData(DataFormats.FileDrop));

                      DropInfoVisible = false;
                  });
            }
        }

        // Команда отображения поля Drag&Drop
        private RelayCommand? dragShowCommand;
        public RelayCommand DragShowCommand
        {
            get 
            {
                return dragShowCommand ??= new RelayCommand(isVisible =>
                    DropInfoVisible = Convert.ToBoolean(isVisible));
            }
        }

        // Команда перехода на сайт разработчика
        private RelayCommand? goSiteCommand;
        public RelayCommand GoSiteCommand
        {
            get
            {
                return goSiteCommand ??= new RelayCommand(_ =>
                      Process.Start(new ProcessStartInfo("https://github.com/Andrey1039") { UseShellExecute = true }));
            }
        }

        // Обработка текста
        private string TransformText(bool mode)
        {
            byte[] byteKey = Encoding.UTF8.GetBytes(Key);
            Array.Resize(ref byteKey, 32);

            try
            {
                if (mode == true)
                    return EncryptText.EncryptString(StartText, byteKey);
                else
                    return EncryptText.DecryptString(StartText, byteKey);
            }
            catch
            {
                _messengerService.ShowErrorMessageBox($"Непредвиденная ошибка в исходном тексте :(");
            }

            return string.Empty;
        }

        // Команда шифрования текста
        private RelayCommand? transformTextCommand;
        public RelayCommand TransformTextCommand
        {
            get
            {
                return transformTextCommand ??= new RelayCommand(mode => EndText = TransformText((bool)mode));
            }
        }

        // Команда сохранения ключа шифрования
        private RelayCommand? saveChangesCommand;
        public RelayCommand SaveChangesCommand
        {
            get
            {
                return saveChangesCommand ??= new RelayCommand(_ =>
                  {
                      Keys.SaveNewKey(Encoding.UTF8.GetBytes(Key));
                      TabsEnable = true;
                  });
            }
        }

        // Команда генерации ключа (авто/вручную)
        private RelayCommand? genAutoCommand;
        public RelayCommand GenAutoCommand
        {
            get
            {
                return genAutoCommand ??= new RelayCommand(state =>
                  {
                      if ((bool)state)
                      {
                          TabsEnable = false;
                          Key = Keys.ReadKey(string.Empty);
                      }
                  });
            }
        }

        // Команда закрытия программы
        private RelayCommand? closingCommand;
        public RelayCommand ClosingCommand
        {
            get
            {
                return closingCommand ??= new RelayCommand(_ => Properties.Settings.Default.Save());
            }
        }

        // Команда очистки исходного текста
        private RelayCommand? clearCommand;
        public RelayCommand ClearCommand
        {
            get
            {
                return clearCommand ??= new RelayCommand(_ => EndText = string.Empty);
            }
        }

        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                OnPropertyChanged("Key");
            }
        }

        public bool TProcess
        {
            get { return _tProcess; }
            set
            {
                _tProcess = value;
                OnPropertyChanged("TProcess");
            }
        }

        public bool DropInfoVisible
        {
            get { return _dropInfoVisible; }
            set
            {
                _dropInfoVisible = value;
                OnPropertyChanged("DropInfoVisible");
            }
        }

        public string StartText
        {
            get { return _startText; }
            set
            {
                _startText = value;
            }
        }

        public string EndText
        {
            get { return _endText; }
            set
            {
                _endText = value;
                OnPropertyChanged("EndText");
            }
        }

        public bool TabsEnable
        {
            get { return _tabsEnable; }
            set
            {
                _tabsEnable = value;
                OnPropertyChanged("TabsEnable");
            }
        }

        public ObservableCollection<Item> FilePaths
        {
            get
            {
                return _filePaths;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}