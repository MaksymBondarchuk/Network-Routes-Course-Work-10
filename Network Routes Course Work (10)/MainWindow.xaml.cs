using System.Windows;
using Microsoft.Win32;

namespace Network_Routes_Course_Work_10
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Graph Graph { get; set; } = new Graph();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "JavaScript Object Notation File|*.json"
            };

            if (dlg.ShowDialog() == true)
                Graph.LoadFromJson(dlg.FileName);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                DefaultExt = ".json",
                Filter = "JavaScript Object Notation File|*.json"
            };

            if (dlg.ShowDialog() == true)
                Graph.LoadToJson(dlg.FileName);
        }

        private void ButtonBuild_Click(object sender, RoutedEventArgs e)
        {
            Graph.BuildPathes();
        }
    }
}
