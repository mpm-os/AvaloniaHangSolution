using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AvaloniaHangProject.Views {
    public class TabHeaderInfo : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        private string caption;
        public string Caption {
            get => caption;
            set {
                if (value != caption) {
                    caption = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

