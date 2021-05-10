using System;
using System.Text;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;


namespace FrequencyLookup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker worker;
        bool isLoaded = false;
        Dictionary<string, int> FreqDict = new Dictionary<string, int>();
        string file = "";

        public MainWindow()
        {
            InitializeComponent();
            panelLookup.Visibility = Visibility.Collapsed;
            lblStatus.Content = "";
            isLoaded = true;
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var item in File.ReadAllLines(file))
            {
                var freq = item.Split('\t')[0];
                var term = item.Split('\t')[1];
                if (!FreqDict.ContainsKey(term))
                    FreqDict.Add(term, Convert.ToInt32(freq));
            }
        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (FreqDict.Count > 0)
            {
                panelLookup.Visibility = Visibility.Visible;
                lblStatus.Content = FreqDict.Count + " terms in the file";
                lblFileName.Text = Path.GetFileName(file);
            }
            else
            {
                panelLookup.Visibility = Visibility.Collapsed;
                lblStatus.Content = "problem with the file!";
            }
        }
        private void btnBrows_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";
            if (OFD.ShowDialog() == true)
            {
                btnCopy.IsEnabled = false;
                lblStatus.Content = "loading...";
                FreqDict.Clear();
                file = OFD.FileName;
                worker.RunWorkerAsync();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string[] termList = txtInputList.Text.Split(new[] { '\r', '\n' });
            var output = new StringBuilder();
            foreach (var term in termList)
            {
                if(string.IsNullOrEmpty(term))
                    output.Append("\n");
                else
                {
                    var freq = (FreqDict.ContainsKey(term)) ? FreqDict[term] : 0;
                    output.Append(term + "\t" + freq + "\n");
                }
            }
            txtOutputList.Text = output.ToString();
            btnCopy.IsEnabled = true;
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/aso-mehmudi") { UseShellExecute = true });
        }

        private void chkRTL_Checked(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                if (chkRTL.IsChecked == false)
                {
                    txtInputList.FlowDirection = FlowDirection.LeftToRight;
                    txtOutputList.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    txtInputList.FlowDirection = FlowDirection.RightToLeft;
                    txtOutputList.FlowDirection = FlowDirection.RightToLeft;
                }
            }
        }
        private void Copy(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOutputList.Text))
            {
                Clipboard.SetText(txtOutputList.Text);
                MessageBox.Show("copied!");
            }
        }
    }
}
