using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Xml;
using System.IO;

namespace dango_viewer {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        bool IsOpend = false;

        public MainWindow() {
            InitializeComponent();
        }

        private void Card_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                var index = (paths[0].Length - 3) < 0 ? 1 : (paths[0].Length - 3);
                var extension = paths[0].Substring(index);
                if (paths.Length == 1 && extension == "txt") {
                    e.Effects = DragDropEffects.None;
                }else {
                    e.Effects = DragDropEffects.None;
                }
            } else {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Card_Drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                var index = (paths[0].Length - 3) < 0 ? 1 : (paths[0].Length - 3);
                var extension = paths[0].Substring(index);
                if (paths.Length == 1 && extension == "txt") {
                    if (!IsOpend) {
                        new DangoView(paths[0]).Show();
                        IsOpend = true;
                    }
                    this.Close();
                }
            }
        }

        private void Card_MouseUp(object sender, MouseButtonEventArgs e) {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = "*.txt";
            ofd.Filter = "文本文档|*.txt";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true) {
                new DangoView(ofd.FileName).Show();
                IsOpend = true;
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            if (File.Exists("config.xml")) {
                StreamReader sr = new StreamReader("config.xml");
                XmlReader xr = XmlReader.Create(sr);
                while (xr.Read()) {
                    if(xr.LocalName == "Path") {
                        xr.Read();
                        if (File.Exists(xr.Value)) {
                            new DangoView(xr.Value).Show();
                            this.Close();
                            IsOpend = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
