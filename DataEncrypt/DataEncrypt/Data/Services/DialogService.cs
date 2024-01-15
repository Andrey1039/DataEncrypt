using System;
using Microsoft.Win32;

namespace DataEncrypt.Data.Services
{
    internal interface IDialogService
    {
        string[] OpenFileDialog(string filter, bool multiSelect);
    }

    internal class DialogService : IDialogService
    {
        public string[] OpenFileDialog(string filter, bool multiSelect)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Выберите файлы";
            openFileDialog.Filter = filter;
            openFileDialog.Multiselect = multiSelect;

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileNames;

            return Array.Empty<string>();
        }
    }
}
