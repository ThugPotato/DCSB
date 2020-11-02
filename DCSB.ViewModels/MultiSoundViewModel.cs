using DCSB.Business;
using DCSB.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace DCSB.ViewModels
{
    /// <summary>
    /// Viewmodel for handling insertion of multiple sounds at once
    /// </summary>
    public class MultiSoundViewModel : ViewModelBase
    {
        private ObservableCollection<Sound> _newSelectedSounds;
        private Sound _selectedSound;

        private OpenFileManager _openFileManager;

        public MultiSoundViewModel(ApplicationStateModel applicationStateModel, ConfigurationModel configurationModel)
        {
            ApplicationStateModel = applicationStateModel;
            ConfigurationModel = configurationModel;
            
            _openFileManager = new OpenFileManager();
            _newSelectedSounds = new ObservableCollection<Sound>();
        }

        public ApplicationStateModel ApplicationStateModel { get; }

        public ConfigurationModel ConfigurationModel { get; }

        public ObservableCollection<Sound> NewSelectedSounds
        {
            get { return _newSelectedSounds; }
            set
            {
                _newSelectedSounds = value;
                this.Set(ref _newSelectedSounds, value);
            }
        }

        public Sound SelectedSound
        {
            get { return _selectedSound; }
            set
            {
                _selectedSound = value;
                this.Set(ref _selectedSound, value);
            }
        }

        public ICommand AddSoundsCommand
        {
            get { return new RelayCommand(openSoundFileDialog); }
        }

        public ICommand ConfirmSoundsCommand
        {
            get { return new RelayCommand(confirmMultiSoundChanges); }
        }

        public ICommand RemoveSoundCommand
        {
            get { return new RelayCommand(removeSound, ConfigurationModel.AreSoundsEnabled); }
        }

        private void openSoundFileDialog()
        {
            string[] result = _openFileManager.OpenSoundFiles();
            if (result != null)
            {
                this.NewSelectedSounds.Clear();

                foreach (string file in result)
                {
                    this.NewSelectedSounds.Add(new Sound
                    {
                        Files = new ObservableCollection<string>
                        {
                            file
                        },
                        Name = Path.GetFileNameWithoutExtension(file)
                    });
                }
            }
        }

        /// <summary>
        /// Process the current ViewModel's sounds to the Configuration - SelectedPreset - SoundCollection
        /// </summary>
        private void confirmMultiSoundChanges()
        {
            foreach(Sound newSound in this.NewSelectedSounds)
            {
                ConfigurationModel.SelectedPreset.SoundCollection.Add(newSound);
            }

            ApplicationStateModel.MultiSoundOpened = false;
        }

        private void removeSound()
        {
            this.NewSelectedSounds.Remove(this.SelectedSound);
        }
    }
}


