using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Popups;
using PersistenceStoreApp.Annotations;

namespace PersistenceStoreApp
{
    class MainViewModel : INotifyPropertyChanged
    {
        // where we can save files
        private StorageFolder _storageFolder = ApplicationData.Current.LocalFolder;

        private ICommand _saveClearTextCommand;
        private ICommand _loadClearTextCommand;
        private string _clearText;
        private StudentModel _student;
        private ICommand _serializeStudentCommand;
        private ICommand _deserializeStudentCommand;

        public ICommand LoadClearTextCommand
        {
            get { return _loadClearTextCommand; }
            set { _loadClearTextCommand = value; }
        }

        public StudentModel Student
        {
            get { return _student; }
            set
            {
                _student = value;
                OnPropertyChanged("Student");
            }
        }

        public String ClearText
        {
            get { return _clearText; }
            set
            {
                _clearText = value;
                OnPropertyChanged("ClearText");
            }
        }

        public ICommand SerializeStudentCommand
        {
            get { return _serializeStudentCommand; }
            set { _serializeStudentCommand = value; }
        }

        public MainViewModel()
        {
            _saveClearTextCommand = new RelayCommand(SaveClearText);
            _loadClearTextCommand = new RelayCommand(LoadClearText);
            
            _student = new StudentModel();
            _serializeStudentCommand = new RelayCommand(SerializeStudent);
            _deserializeStudentCommand = new RelayCommand(DeserializeStudent);
        }

        private async void LoadClearText()
        {
            //create stram to file
            Stream stream = await _storageFolder.OpenStreamForReadAsync("ClearText.txt");
            //connect streamwriter to stream
            StreamReader streamReader = new StreamReader(stream);
            
            //read textfile
            ClearText = streamReader.ReadToEnd();
            stream.Dispose();
            new MessageDialog(ClearText).ShowAsync();

        }

        private async void SaveClearText()
        {
            //create stram to file
            Stream stream = await _storageFolder.OpenStreamForWriteAsync("ClearText.txt", CreationCollisionOption.ReplaceExisting);
            //connect streamwriter to stream
            StreamWriter streamWriter = new StreamWriter(stream);
            //write text
            streamWriter.Write(ClearText);
            streamWriter.Flush();
            stream.Dispose();
        }


        public ICommand SaveClearTextCommand
        {
            get { return _saveClearTextCommand; }
            set { _saveClearTextCommand = value; }
        }

        public ICommand DeserializeStudentCommand
        {
            get { return _deserializeStudentCommand; }
            set { _deserializeStudentCommand = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void SerializeStudent()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            Stream stream = await storageFolder.OpenStreamForWriteAsync("student.xml", CreationCollisionOption.ReplaceExisting);
            
            DataContractSerializer serializer = new DataContractSerializer(typeof(StudentModel));
            serializer.WriteObject(stream, _student);
        }

        private async void DeserializeStudent()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            Stream stream = await storageFolder.OpenStreamForReadAsync("student.xml");

            DataContractSerializer serializer = new DataContractSerializer(typeof(StudentModel));
            Student = (StudentModel) serializer.ReadObject(stream);


        }
    }
}
