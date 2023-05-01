using MahApps.Metro.Controls;
using Org.BouncyCastle.Bcpg.OpenPgp;
using petFinder.Models;
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
using System.Windows.Shapes;

namespace petFinder
{
    /// <summary>
    /// DetailWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DetailWindow : MetroWindow
    {

        public DetailWindow()
        {
            InitializeComponent();
        }

        public DetailWindow(AminamlInfoDetail pet) : this()
        {
            this.DataContext = pet;
            ImgPet.Source = new BitmapImage(new Uri($"{pet.Ty3Picture}", UriKind.RelativeOrAbsolute));
        }


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
