using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sptask3
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {


        private int currentProgress;
        public int CurrentProgress
        {
            get { return this.currentProgress; }
            set
            {
                currentProgress = value; OnPropertyChanged();
            }
        }


        CancellationTokenSource cancel;
        public MainWindow()
        {
            cancel = new();
            InitializeComponent();
            DataContext = this;
        }

        string selectedFilePath;
        private void fromfile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = " (*.txt)|*.txt|  (*.*)|*.*";
            openFile.InitialDirectory = "C:\\";
            openFile.Title = "Bir File Seçin";
            if (openFile.ShowDialog() == true)
            {
                selectedFilePath = openFile.FileName;
                fromname.Text = selectedFilePath;
            }
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /*if (cancel.IsCancellationRequested)
                    cancel = new CancellationTokenSource();*/


                byte key = byte.Parse(password.Text);
                Thread thread = new Thread(() =>
                {


                    EncryptDecrypt(selectedFilePath, key);
                }); thread.Start();
            }
            catch (FormatException)
            {
                MessageBox.Show("Enter valid number .", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (OverflowException)
            {
                MessageBox.Show("enter num between 0 and 255  .", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            cancel.Cancel();
            MessageBox.Show("Prosses Cancelled ");
        }



        public void EncryptDecrypt(string inputFilePath, byte key)
        {


            byte[] fileBytes = File.ReadAllBytes(inputFilePath);
             
            MessageBox.Show("Crypting  started");
            Thread.Sleep(100);
            for (int i = 0; i < fileBytes.Length; i++)
            {
                if (cancel.IsCancellationRequested)
                {
                    MessageBox.Show("Crypting  ended, token is closed");

                    CurrentProgress = 0;

                    return;
                }
                
                Application.Current.Dispatcher.Invoke(() =>
                                  {
                                      Thread.Sleep(500);


                                      CurrentProgress += 10 * i;

                                  });
                Thread.Sleep(500);
                fileBytes[i] = (byte)(fileBytes[i] ^ key);


            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Crypting  completed");
                Thread.Sleep(1500);
            });
            File.WriteAllBytes(inputFilePath, fileBytes);
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Process completed");
                password.Text = string.Empty;
                fromname.Text = string.Empty;
            });
            CurrentProgress = 0;


        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}