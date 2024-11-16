using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest
{
    internal class Vehicle
    {
        internal int i_INTdoors { get; set; }
        internal bool i_BOOLhasBed { get; set; }
        internal bool i_BOOLhas4x4 { get; set; }
        internal int i_INThorsePower { get; set; }
        internal double i_DBLprice { get; set; }

        internal Vehicle(int v_INTdoors, bool v_BOOLhasBed, bool v_BOOLhas4x4, int v_INThorsePower, double v_DBLprice)
        {
            i_INTdoors = v_INTdoors;
            i_BOOLhasBed = v_BOOLhasBed;
            i_BOOLhas4x4 = v_BOOLhas4x4;
            i_DBLprice = v_DBLprice;
        }
    }
}