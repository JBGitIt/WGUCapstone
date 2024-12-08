using RandomForest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest
{
    public class Arborist
    {
        IEnumerable<object> c_COLLdataSet;
        IEnumerable<object> c_COLLclassifications;

        int c_INTmaxTrees;
        int c_INTmaxTreeDepth;
        int c_INTminSamplesToSplit;
        int c_INTforestID;

        List<DecisionTree> c_COLLforest;

        public List<DecisionTree> Forest { get { return c_COLLforest; } }

        public Arborist(IEnumerable<object> r_COLLdataSet, IEnumerable<object> r_COLLclassifications, int v_INTmaxTrees, int v_INTmaxTreeDepth, int v_INTminSamplesToSplit)
        {
            c_COLLdataSet = r_COLLdataSet;
            c_COLLclassifications = r_COLLclassifications;
            c_INTmaxTrees = v_INTmaxTrees;
            c_INTmaxTreeDepth = v_INTmaxTreeDepth;
            c_INTminSamplesToSplit = v_INTminSamplesToSplit;
            c_COLLforest = new List<DecisionTree>();
        }

        public object[] Predict(IEnumerable<object> r_COLLfeatures)
        {
            Dictionary<object, int> l_COLLresultCounts = new Dictionary<object, int>();
            foreach (DecisionTree l_OBJtree in c_COLLforest)
            {
                object result = l_OBJtree.Predict(r_COLLfeatures);
                if (l_COLLresultCounts.ContainsKey(result))
                {
                    l_COLLresultCounts[result]++;
                }
                else
                {
                    l_COLLresultCounts.Add(result, 1);
                }
            }

            double l_DBLresult = 0;
            double l_DBLhigh = 0;
            double l_DBLlow = Double.PositiveInfinity;
            int l_INTresultsCount = 0;
            int l_INTabovePrediction = 0;
            int l_INTbelowPrediction = 0;

            if (double.TryParse(l_COLLresultCounts.First().Key.ToString(), out l_DBLresult))
            {
                foreach (object l_OBJkey in l_COLLresultCounts.Keys)
                {
                    double l_DBLkey = 0;

                    if (double.TryParse(l_OBJkey.ToString(), out l_DBLkey))
                    {
                        l_DBLresult += l_DBLkey * l_COLLresultCounts[l_OBJkey];
                        l_INTresultsCount += l_COLLresultCounts[l_OBJkey];
                        if(Convert.ToDouble((decimal)l_OBJkey) > l_DBLhigh)
                        {
                            l_DBLhigh = Convert.ToDouble((decimal)l_OBJkey);
                        }
                        if(Convert.ToDouble((decimal)l_OBJkey) < l_DBLlow)
                        {
                            l_DBLlow = Convert.ToDouble((decimal)l_OBJkey);
                        }
                    }
                    else
                    {
                        throw new Exception("How the F...");
                    }
                }

                double l_DBLprediction = l_DBLresult / l_INTresultsCount;

                foreach (object l_OBJkey in l_COLLresultCounts.Keys)
                {
                    double l_DBLkey = 0;

                    if (double.TryParse(l_OBJkey.ToString(), out l_DBLkey))
                    {
                        if (Convert.ToDouble((decimal)l_OBJkey) >= l_DBLprediction)
                        {
                            l_INTabovePrediction += l_COLLresultCounts[l_OBJkey];
                        }
                        if (Convert.ToDouble((decimal)l_OBJkey) < l_DBLprediction)
                        {
                            l_INTbelowPrediction += l_COLLresultCounts[l_OBJkey]; 
                        }
                    }
                    else
                    {
                        throw new Exception("How the F...");
                    }
                }

                return new object[] { l_DBLprediction, l_DBLhigh, l_DBLlow, l_INTabovePrediction, l_INTbelowPrediction };
            }

            else
            {
                object l_OBJcommonResult = null;
                int l_INTcommonResultCount = 0;
                foreach (object l_OBJkey in l_COLLresultCounts.Keys)
                {
                    if (l_COLLresultCounts[l_OBJkey] > l_INTcommonResultCount)
                    {
                        l_INTcommonResultCount = l_COLLresultCounts[l_OBJkey];
                        l_OBJcommonResult = l_OBJkey;
                    }
                }
                return new object[] { l_OBJcommonResult, null, null };
            }
        }

        /// <summary>
        /// Plants a forest to analyze time series data
        /// </summary>
        /// <param name="v_INTwindowWidth">Width of the window of timesteps included in each tree of the forest</param>
        /// <param name="v_INTwindowStep">How many timesteps to move the window forward for each tree</param>
        public void PlantForestTimeSeries(int v_INTwindowWidth, int v_INTwindowStep)
        {
            //always start at timestep 0 and included everything within the first window width
            int l_INTwindowStart = 0;
            int l_INTwindowEnd = v_INTwindowWidth;

            //while the end of the current window is less than or equal to the length of our timeseries
            while (l_INTwindowEnd <= c_COLLdataSet.Count())
            {
                List<object> l_COLLslidingWindowData = new List<object>();
                List<object> l_COLLslidingWindowClassifications = new List<object>();

                //grab the elements that correspond to each step in our current window
                for (int i = l_INTwindowStart; i < l_INTwindowEnd; i++)
                {
                    l_COLLslidingWindowData.Add(c_COLLdataSet.ElementAt(i));
                    l_COLLslidingWindowClassifications.Add(c_COLLclassifications.ElementAt(i));
                }

                //Plant a tree using the selected elements
                c_COLLforest.Add(PlantTree(l_COLLslidingWindowData, l_COLLslidingWindowClassifications));

                if (l_INTwindowEnd + v_INTwindowStep > c_COLLdataSet.Count())
                {
                    v_INTwindowStep = l_INTwindowEnd + v_INTwindowStep - c_COLLdataSet.Count();
                }

                l_INTwindowStart += v_INTwindowStep;
                l_INTwindowEnd += v_INTwindowStep;
            }
        }

        public void PlantForestCategorization(int v_INTsampleRatio)
        {
            int l_INTrowsInSample = c_COLLdataSet.Count() / v_INTsampleRatio;

            while (c_COLLforest.Count() < c_INTmaxTrees)
            {
                Random l_OBJrando = new Random();

                List<object> l_COLLclassifications = new List<object>();
                List<object> l_COLLdata = new List<object>();
                HashSet<int> l_COLLaddedIndices = new HashSet<int>();

                while (l_COLLdata.Count() < l_INTrowsInSample)
                {
                    int l_INTnextIndex = l_OBJrando.Next(c_COLLdataSet.Count() - 1);
                    if (l_COLLaddedIndices.Contains(l_INTnextIndex))
                    {
                        continue;
                    }
                    l_COLLaddedIndices.Add(l_INTnextIndex);
                    l_COLLdata.Add(c_COLLdataSet.ElementAt(l_INTnextIndex));
                    l_COLLclassifications.Add(c_COLLclassifications.ElementAt(l_INTnextIndex));
                }
                c_COLLforest.Add(PlantTree(l_COLLdata, l_COLLclassifications));
                //if(c_COLLforest.Count % 100 == 0 && c_COLLforest.Count != c_INTmaxTrees)
                //{
                //    Console.WriteLine($"{(c_COLLforest.Count() / (double)c_INTmaxTrees).ToString().Replace("0.", "%")}");
                //}
            }
        }

        private DecisionTree PlantTree(IEnumerable<object> r_COLLdataSet, IEnumerable<object> r_COLLclassification)
        {
            DecisionTree l_OBJnewTree = new DecisionTree(c_INTmaxTreeDepth, 0);

            l_OBJnewTree.GrowTreeRF(r_COLLdataSet, r_COLLclassification);

            return l_OBJnewTree;
        }

    }

}