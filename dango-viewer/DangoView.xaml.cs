using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace dango_viewer {
    /// <summary>
    /// DangoView.xaml 的交互逻辑
    /// </summary>
    public partial class DangoView : Window {
        readonly string PATH;
        const string KANJI_REGEX = @"\A.+?(?=\s)";
        const string KANA_REGEX = @"(?<=\s+)\S+?(?=\s)";
        const string MEANING_REGEX = @"\s+.+\s+(.+$)|.+\s+(.+$)";

        List<Dango> dangos = new List<Dango>();
        bool isChanged = false;

        public DangoView(string path) {
            InitializeComponent();
            PATH = path;
        }

        async Task ReadFile(string path) {
            StreamReader sr = new StreamReader(path);
            while (!sr.EndOfStream) {
                var s = sr.ReadLine();

                Dango dango = new Dango();
                dango.Kanji = Regex.Match(s, KANJI_REGEX).Value;
                dango.Kana = Regex.Match(s, KANA_REGEX).Value;
                var meanning = Regex.Match(s, MEANING_REGEX);
                dango.Meaning = meanning.Groups[1].Value == "" ? meanning.Groups[2].Value : meanning.Groups[1].Value;

                if (!dango.GetIsEmpty()) {
                    dangos.Add(dango);
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            await ReadFile(PATH);
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.To = new Thickness(prosses.Margin.Left, -15, prosses.Margin.Right, prosses.Margin.Bottom);
            animation.AccelerationRatio = 0;
            animation.DecelerationRatio = 1;
            prosses.BeginAnimation(MarginProperty, animation);

            mDataGrid.ItemsSource = dangos;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if(isChanged) {
                switch(MessageBox.Show("Save?", "Tips", MessageBoxButton.YesNoCancel)) {
                    case MessageBoxResult.Yes:
                        Save(PATH);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }

            XmlWriter xw = XmlWriter.Create("config.xml");
            xw.WriteElementString("Path", PATH);
            xw.Flush();
            xw.Dispose();
            xw.Close();
        }
        void Save(string path) {
            StreamWriter sw = new StreamWriter(path);
            foreach (Dango dango in dangos) {
                var lineStr = dango.Kanji + " " + dango.Kana + " " + dango.Meaning;
                sw.WriteLine(lineStr);
            }
            sw.Flush();
            sw.Close();
        }

        private void mDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            if (e.EditAction == DataGridEditAction.Commit && e.Column != null) {
                isChanged = true;
            }
        }

        private void mDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
            deleteBtn.IsEnabled = true;
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e) {
            dangos.Remove((Dango)mDataGrid.SelectedCells[0].Item);
            mDataGrid.Items.Refresh();
            isChanged = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            dangos.Insert(0, new Dango());
            mDataGrid.Items.Refresh();
        }
    }
}
