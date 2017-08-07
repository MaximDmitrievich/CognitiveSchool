using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;
using WebEye.Controls.Wpf;

namespace CognitiveSchool
{
    public delegate void TimerFunction(object sender, EventArgs e);

    public static class Utility
    {
        public static string PickFile()
        {
            string result = null;
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Video files (*.mp4, *.mov, *.wmv)|*.mp4;*.mov;*.wmv";
            bool? isOpened = openDlg.ShowDialog(window);
            if (isOpened.GetValueOrDefault(false))
            {
                result = openDlg.FileName;
            }
            return result;
        }

        public static void SaveFile(FileStream savingFile)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "Video files (*.mp4, *.mov, *.wmv)|*.mp4;*.mov;*.wmv";
            bool? isOpened = saveDlg.ShowDialog(window);
            using (Stream saveFile = new FileStream(saveDlg.FileName, FileMode.Create, FileAccess.ReadWrite))
            {
                savingFile.Seek(0, SeekOrigin.Begin);
                savingFile.CopyTo(saveFile);
            }
            
        }

        public static void InitializeCombobox(ComboBox comboBox, WebCameraControl camera)
        {
            comboBox.ItemsSource = camera.GetVideoCaptureDevices();

            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedItem = comboBox.Items[0];
            }
        }

        public static void InitializeTimer(DispatcherTimer timer, TimerFunction func)
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(func);
            timer.Interval = new TimeSpan(0, 0, 0, 2);
            timer.Start();
        }
    }
}
