using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DataEncrypt.Models
{
    internal class Item : INotifyPropertyChanged
    {
        private readonly string fileName;
        private bool isChecked;

        public Item(string fileName, bool isChecked)
        {
            this.fileName = fileName;
            this.isChecked = isChecked;
        }

        public string FileName
        {
            get { return fileName; }
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set 
            { 
                isChecked = value;
                OnPropertyChanged("IsChecked");
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
