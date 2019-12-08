using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotiTube
{
    class SongListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Song> _songListObservable { get; set; }
        public ObservableCollection<Song> songListObservable
        {
            get { return _songListObservable; }
            set
            {
                _songListObservable = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("_songListObservable"));
                }
            }
        }

        public SongListViewModel() {
            songListObservable = new ObservableCollection<Song>();
        }
    }
}
