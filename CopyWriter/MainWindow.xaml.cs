using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace CopyWriter
{
    public partial class MainWindow : Window
    {
        ViewModel model = new ViewModel();
        
        public MainWindow()
        {
            InitializeComponent();
            model.Progress = 0;
            this.DataContext = model; 
        }

        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filename = Path.GetFileName(model.Source);
                string destFilePath = Path.Combine(model.Destination, filename);
                await CopyFileAsync(model.Source, destFilePath);
                MessageBox.Show("Complete!");
                model.Source = null;
                model.Destination = null;
                model.Progress = 0;
            }
            catch (Exception)
            {
                MessageBox.Show("Fill in all gapes and check if they are written correctly!");
            }
        }
        private Task CopyFileAsync(string src, string dest)
        {
            return Task.Run(() =>
            {
                using FileStream streamSource = new FileStream(src, FileMode.Open, FileAccess.Read);
                using FileStream streamDest = new FileStream(dest, FileMode.Create, FileAccess.Write);
                byte[] buffer = new byte[1024 * 8];
                int bytes = 0;
                do
                {
                    bytes = streamSource.Read(buffer, 0, buffer.Length);
                    streamDest.Write(buffer, 0, bytes);
                    float procent = streamDest.Length / (streamSource.Length / 100);
                    model.Progress = procent;
                } while (bytes > 0);
            });
        }

        private void OpenSourceBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if(dialog.ShowDialog() == true)
            {
                model.Source = dialog.FileName;
            }
        }

        private void OpenDestBtn_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                model.Destination = dialog.FileName;
            }
        }
    }
    [AddINotifyPropertyChangedInterface]
    class ViewModel
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public float Progress { get; set; }
        public bool IsWaiting => Progress == 0;
    }
}
