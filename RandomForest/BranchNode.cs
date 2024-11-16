using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest
{
    internal class BranchNode
    {
        private bool c_BOOLisNumeric;
        private int c_INTfeatureIndex;
        private object c_OBJthreshold;
        private object c_OBJvalue;
        private DecisionTree c_OBJleftSubtree;
        private DecisionTree c_OBJrightSubtree;

        internal DecisionTree LeftSubtree { get => c_OBJleftSubtree; set => c_OBJleftSubtree = value; }
        internal DecisionTree RightSubtree { get => c_OBJrightSubtree; set => c_OBJrightSubtree = value; }

        internal object Value { get => c_OBJvalue; }

        internal bool CheckLeaf()
        {
            return c_OBJvalue is null;
        }

        internal BranchNode(bool v_BOOLisNumeric, int v_INTfeatureIndex, object r_OBJthreshold, object r_OBJvalue, DecisionTree r_OBJleft, DecisionTree r_OBJright)
        {
            c_BOOLisNumeric = v_BOOLisNumeric;
            c_INTfeatureIndex = v_INTfeatureIndex;
            c_OBJthreshold = r_OBJthreshold;
            c_OBJvalue = r_OBJvalue;
            c_OBJleftSubtree = r_OBJleft;
            c_OBJrightSubtree = r_OBJright;
        }

        internal BranchNode(bool v_BOOLisNumeric, int v_INTfeatureIndex, object r_OBJthreshold, DecisionTree r_OBJleft, DecisionTree r_OBJright)
        {
            c_BOOLisNumeric = v_BOOLisNumeric;
            c_INTfeatureIndex = v_INTfeatureIndex;
            c_OBJthreshold = r_OBJthreshold;
            c_OBJvalue = null;
            c_OBJleftSubtree = r_OBJleft;
            c_OBJrightSubtree = r_OBJright;
        }

        internal BranchNode(bool v_BOOLisNumeric, int v_INTfeatureIndex, object r_OBJthreshold)
        {
            c_BOOLisNumeric = v_BOOLisNumeric;
            c_INTfeatureIndex = v_INTfeatureIndex;
            c_OBJthreshold = r_OBJthreshold;
            c_OBJvalue = null;
        }

        internal object Predict(IEnumerable<object> r_COLLfeatures)
        {
            if (c_OBJvalue is not null)
            {
                return c_OBJvalue;
            }
            else
            {
                if (c_BOOLisNumeric)
                {
                    if (Convert.ToDouble(r_COLLfeatures.ElementAt(c_INTfeatureIndex)) <= Convert.ToDouble(c_OBJthreshold))
                    {
                        return LeftSubtree.RootNode.Predict(r_COLLfeatures);
                    }
                    else
                    {
                        return RightSubtree.RootNode.Predict(r_COLLfeatures);
                    }
                }
                else
                {
                    if (r_COLLfeatures.ElementAt(c_INTfeatureIndex) == c_OBJthreshold)
                    {
                        return LeftSubtree.RootNode.Predict(r_COLLfeatures);
                    }
                    else
                    {
                        return RightSubtree.RootNode.Predict(r_COLLfeatures);
                    }
                }
            }
        }
    }
}