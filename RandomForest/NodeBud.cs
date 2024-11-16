using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest
{
    internal class NodeBud
    {
        private double c_DBLimpurity;
        private int c_OBJbestFeatureIndex;
        private object c_OBJbestThreshold;
        private List<KeyValuePair<object, object>> c_COLLleftPairs;
        private List<KeyValuePair<object, object>> c_COLLrightPairs;

        internal double Impurity { get => c_DBLimpurity; set => c_DBLimpurity = value; }
        internal int FeatureIndex { get => c_OBJbestFeatureIndex; set => c_OBJbestFeatureIndex = value; }
        internal object Threshold { get => c_OBJbestThreshold; set => c_OBJbestThreshold = value; }
        internal List<KeyValuePair<object, object>> LeftSplit { get => c_COLLleftPairs; set => c_COLLleftPairs = value; }
        internal List<KeyValuePair<object, object>> RightSplit { get => c_COLLrightPairs; set => c_COLLrightPairs = value; }

        internal NodeBud(double v_DBLimpurity, int v_INTbestFeatureIndex, object r_OBJbestThreshold, List<KeyValuePair<object, object>> r_COLLleftPairs, List<KeyValuePair<object, object>> r_COLLrightPairs)
        {
            this.c_DBLimpurity = v_DBLimpurity;
            this.c_OBJbestFeatureIndex = v_INTbestFeatureIndex;
            this.c_OBJbestThreshold = r_OBJbestThreshold;
            this.c_COLLleftPairs = r_COLLleftPairs;
            this.c_COLLrightPairs = r_COLLrightPairs;
        }
    }
}