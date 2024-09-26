using Helix01_First.Models;
using Helix01_First.ViewModels.Base;
using System.Collections.Generic;

namespace Helix01_First.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private List<Coordinate> _head;
        private List<Coordinate>  _prob;

        public List<Coordinate> Head
        {
            get => _head;
            set => SetProperty(ref _head, value);
        }
        public List<Coordinate> Prob
        {
            get => _prob;
            set => SetProperty(ref _prob, value);
        }

        public MainWindowViewModel()
        {
            Head = new List<Coordinate> { new Coordinate { X = 0, Y = 0, Z = 0, TX = 0, TY = 0, TZ = 0 } };
            Prob = new List<Coordinate>();
        }

    }
}
