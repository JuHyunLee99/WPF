using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix01_First.Models
{
    public class Coordinate : ObservableObject
    {
        private double x; private double y; private double z; private double tx; private double ty; private double tz;
        public double X { get { return x; } set { SetProperty(ref x, value); } }
        public double Y { get { return y; } set { SetProperty(ref y, value); } }
        public double Z { get { return z; } set { SetProperty(ref z, value); } }
        public double TX { get { return tx; } set { SetProperty(ref tx, value); } }
        public double TY { get { return ty; } set { SetProperty(ref ty, value); } }
        public double TZ { get { return tz; } set { SetProperty(ref tz, value); } }
    }
}
