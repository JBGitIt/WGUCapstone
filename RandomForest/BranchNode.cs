using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest
{
    public class BranchNode
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

        public bool Numeric { get { return c_BOOLisNumeric; } }
        public int FeatureIndex { get { return c_INTfeatureIndex; } }
        public object Threshold { get { return c_OBJthreshold; } }
        public object NodeValue { get { return c_OBJvalue; } }
        public DecisionTree LeftBranch { get {  return c_OBJleftSubtree; } }
        public DecisionTree RightBranch { get { return c_OBJrightSubtree; } }

        internal bool CheckLeaf()
        {
            return c_OBJvalue is null;
        }

        public BranchNode(bool v_BOOLisNumeric, int v_INTfeatureIndex, object r_OBJthreshold, object r_OBJvalue, DecisionTree r_OBJleft, DecisionTree r_OBJright)
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

        /// <summary>
        /// This is the method used to make predictions.
        /// It is recursive(ish) and will travers it's way down the tree until it hits a leafnode at which point that value will be passed back up the tree.
        /// </summary>
        /// <param name="r_COLLfeatures"></param>
        /// <returns></returns>
        internal object Predict(IEnumerable<object> r_COLLfeatures)
        {
            //if this node has a value then it must be a leaf and we simply return that value to the caller.
            if (c_OBJvalue is not null)
            {
                return c_OBJvalue;
            }
            //otherwise we continue traversing our way down the tree.
            else
            {
                //we check if this is a numeric node
                if (c_BOOLisNumeric)
                {
                    //then compare the value of the feature from r_COLLfeatures that corresponds to the the threshold of this node with said threshold.
                    if (Convert.ToDouble(r_COLLfeatures.ElementAt(c_INTfeatureIndex)) <= Convert.ToDouble(c_OBJthreshold))
                    {
                        //if the value is less than the threshold we continue down the left branch
                        return LeftSubtree.RootNode.Predict(r_COLLfeatures);
                    }
                    else
                    {
                        //if the value is greater than the threshold we continue down the right branch
                        return RightSubtree.RootNode.Predict(r_COLLfeatures);
                    }
                }
                else
                {
                    //if this is not a numeric node then the left branch is taken if the value is equal to our threshold value
                    if (r_COLLfeatures.ElementAt(c_INTfeatureIndex) == c_OBJthreshold)
                    {
                        return LeftSubtree.RootNode.Predict(r_COLLfeatures);
                    }
                    //all other values go down the right branch.
                    else
                    {
                        return RightSubtree.RootNode.Predict(r_COLLfeatures);
                    }
                }
            }
        }
    }
}