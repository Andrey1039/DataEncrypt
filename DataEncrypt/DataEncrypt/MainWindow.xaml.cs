using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Collections;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using DataEncrypt.Encryption;
using System.Windows.Controls;
using System.Collections.Generic;

namespace DataEncrypt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SecretKeyTB.Text = Keys.GenerateKey(false);
        }

        // Выбор файлов для шифрования
        private void SelectFileDialog()
        {           
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Выберите файлы";
            openFileDialog.Filter = "Все файлы (*.*)|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                string[] selectedFilePaths = openFileDialog.FileNames;

                foreach (string filePath in selectedFilePaths)
                {
                    FilesListBox.Items.Add(filePath);
                }
            }
        }

        private void ItemAddBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectFileDialog();
        }

        // Удаление файлов из списка шифрования
        private void DeleteFile()
        {
            IList selectedItems = FilesListBox.SelectedItems;

            if (selectedItems.Count > 0)
                for (int i = selectedItems.Count - 1; i >= 0; i--)
                    FilesListBox.Items.RemoveAt(FilesListBox.Items.IndexOf(selectedItems[i]));
            else
                MessageBox.Show("Ни один элемент не выбран", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ItemDelBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteFile();
        }

        private void GoSiteBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/Andrey1039") { UseShellExecute = true });
        }

        // Добавление файлов через Drag&Drop
        private void DropInfo_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = ((string[])e.Data.GetData(DataFormats.FileDrop));

                foreach (string path in files)
                    if (Directory.Exists(path))
                    {
                        string[] newFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

                        foreach (string newFile in newFiles)
                            FilesListBox.Items.Add(newFile);
                    }
                    else
                    {
                        FilesListBox.Items.Add(path);
                    }
            }

            DropInfo.Visibility = Visibility.Hidden;
        }

        private void DropInfo_DragLeave(object sender, DragEventArgs e)
        {
            DropInfo.Visibility = Visibility.Hidden;
        }

        private void FilesListBox_DragOver(object sender, DragEventArgs e)
        {
            DropInfo.Visibility = Visibility.Visible;
        }

        // Шифрование файлов
        private void EncryptData(List<string> files, byte[] key, bool mode)
        {
            Dispatcher.Invoke(() =>
            {
                Tabs.IsEnabled = false;
                EncryptProcess.Visibility = Visibility.Visible;
                EncryptProcessPB.IsIndeterminate = true;
            });

            int countFiles = files.Count;
            int count = 0;

            foreach (string file in files)
            {
                Dispatcher.Invoke(() =>             
                    EncryptProcessL.Content = $"Выполняется обработка файлов ({++count} из {countFiles})"
                );

                try 
                {
                    if (mode == true)
                        EncryptFile.EncryptFiles(file, key);
                    else
                        EncryptFile.DecryptFiles(file, key);                    
                }
                catch
                {
                    MessageBox.Show($"Невозможно прочитать файл:\n{file}\n\nФайл будет пропущен", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }                              
            }

            Dispatcher.Invoke(() =>
            {
                EncryptProcess.Visibility = Visibility.Hidden;
                EncryptProcessPB.IsIndeterminate = false;
                Tabs.IsEnabled = true;
            });
        }

        // Подготовка к шифрованию файлов
        private void StartEncrypt()
        {
            byte[] key = Encoding.UTF8.GetBytes(SecretKeyTB.Text);
            Array.Resize(ref key, 32);
            
            List<string> files = FilesListBox.Items
                .Cast<String>()
                .ToList();

            bool mode = Convert.ToBoolean(EncryptFileModeBtn.IsChecked);
            var chiper = Task.Factory.StartNew(() => EncryptData(files, key, mode));          
        }

        private void FileStartBtn_Click(object sender, RoutedEventArgs e)
        {
            StartEncrypt();
        }

        // Шифрование текста
        private void EncryptData()
        {
            byte[] key = Encoding.UTF8.GetBytes(SecretKeyTB.Text);
            Array.Resize(ref key, 32);

            byte[] iv = Enumerable.Range(0, 16).Select(x => (byte)x).ToArray();
            List<string> files = FilesListBox.Items.Cast<String>().ToList();

            try
            {
                if (EncryptTextModeBtn.IsChecked == true)
                    ChangeTB.Text = EncryptText.EncryptString(OriginalTB.Text, key);
                else
                    ChangeTB.Text = EncryptText.DecryptString(OriginalTB.Text, key);
            }
            catch
            {
                MessageBox.Show("Непредвиденная ошибка в исходном тексте :(", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextStartBtn_Click(object sender, RoutedEventArgs e)
        {
            EncryptData();
        }

        // Сохранение ключа шифрования
        private void SaveChanges() 
        {
            if (Encoding.UTF8.GetBytes(SecretKeyTB.Text).Length >= 32)
            {
                SecretKeyTB.Text = Keys.SaveNewKey(@"SOFTWARE\\DataEncrypt", Encoding.UTF8.GetBytes(SecretKeyTB.Text));
                PageText.IsEnabled = true;
            }
            else
                MessageBox.Show("Ключ недостаточной длины");
        }

        private void SaveChangesBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }

        // Смена режима ключа (авто/вручную)
        private void GenAutoBoxChange(bool mode)
        {
            if (GenAutoBox.IsChecked == true)
            {
                if (mode)
                {
                    PageText.IsEnabled = false;
                    SecretKeyTB.Text = Keys.GenerateKey(Convert.ToBoolean(GenAutoBox.IsChecked));
                }
                    
                SecretKeyTB.IsEnabled = false;
            }
            else
            {
                SecretKeyTB.IsEnabled = true;
                SecretKeyTB.Text = string.Empty;
            }           
        }

        private void GenAutoBox_Click(object sender, RoutedEventArgs e)
        {
            GenAutoBoxChange(true);
        }

        // Реакция на нопку delete
        private void FilesListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteFile();
                e.Handled = true;
            }
        }

        // Реакция на ctrl+A
        private void FilesListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.A)
            {
                ListBox listBox = (ListBox)sender;
                listBox.SelectAll();
                e.Handled = true;
            }
        }

        private void OriginalTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChangeTB.Text = string.Empty;

            if (!OriginalTB.Text.Equals(string.Empty))
                TextStartBtn.IsEnabled = true;
            else
                TextStartBtn.IsEnabled = false;
        }

        private void SecretKeyTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            byte[] key = Encoding.UTF8.GetBytes(SecretKeyTB.Text);

            if (key.Length < 32)
            {
                SaveChangesBtn.IsEnabled = false;
                PageText.IsEnabled = false;
            }
            else
                SaveChangesBtn.IsEnabled = true;                        
        }

        // Сохранение статуса GenAutoBox
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.checkAutoGenBox = Convert.ToBoolean(GenAutoBox.IsChecked);
            Properties.Settings.Default.Save();
        }

        // Восстановление статуса GenAutoBox
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GenAutoBox.IsChecked = Properties.Settings.Default.checkAutoGenBox;
        }

        private void GenAutoBox_Checked(object sender, RoutedEventArgs e)
        {
            GenAutoBoxChange(false);
        }
    }
}
