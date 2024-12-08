using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest
{
    /// <summary>
    /// Class to model a decision tree from a collection of data and collection of classifications for each row of said data.
    /// Class is designed such that the data can be submitted as either objects or arrays. Any collection outside of arrays will not have the expected behavior and will fail to form a tree.
    /// Features in the data should be limited to simple types and objects that implement the IComparable Interface for equality.
    /// </summary>
    public class DecisionTree
    {
        private int c_INTdepth = 0;          //Current depth of the tree, default to 0 on instantiation.
        private int c_INTmaxDepth;           //Maximum depth of the tree
        private int c_INTminSamplesSplit = 2;//Minimum number of records which we need to be capable of executing a split, default to 2 on instantiation.

        private BranchNode c_OBJrootNode;
        private IEnumerable<BranchNode> c_COLLbranches;

        public int Depth { get { return c_INTdepth; } }
        public int MaxDepth {  get { return c_INTmaxDepth; } }
        public int MinSamples { get {  return c_INTminSamplesSplit; } }
        public BranchNode Root { get { return c_OBJrootNode; } }

        internal BranchNode RootNode { get => c_OBJrootNode; set => c_OBJrootNode = value; }

        /// <summary>
        /// Creates an instance of the DecisionTree class at initial depth v_INTdepth and with maximum depth v_INTmaxDepth
        /// </summary>
        /// <param name="v_INTmaxDepth">Maximum Depth for the tree</param>
        /// <param name="v_INTdepth">Initial depth of the tree</param>
        internal DecisionTree(int v_INTmaxDepth, int v_INTdepth)
        {
            this.c_INTmaxDepth = v_INTmaxDepth;
            this.c_INTdepth = v_INTdepth;
        }

        internal DecisionTree(int v_INTmaxDepth, int v_INTdepth, BranchNode r_OBJrootNode)
        {
            c_INTmaxDepth = v_INTmaxDepth;
            c_INTdepth = v_INTdepth;
            c_OBJrootNode = r_OBJrootNode;
        }

        public DecisionTree(int v_INTdepth, int v_INTmaxDepth, int v_INTminSamplesSplit, BranchNode r_OBJrootNode) : this(v_INTdepth, v_INTmaxDepth)
        {
            this.c_INTminSamplesSplit = c_INTminSamplesSplit;
            this.c_OBJrootNode = r_OBJrootNode;
        }



        /// <summary>
        /// Recursive method to grow the decision tree until one of three things happens:
        /// 1. We only have one classification remaining.
        /// 2. We don't have enough records to split on (usually 2).
        /// 3. We have reached the maximum depth of the tree.
        /// For outcomes one and two the value of the leaf nodes will be whatever the remaining classification is.
        /// For outcome 3 the value of each leaf will be the most common classification at that node.
        /// </summary>
        /// <param name="r_COLLdata">Dataset we are modeling our tree on. Accepts IEnumerable of objects but those objects should be either arrays or objects</param>
        /// <param name="r_COLLclassifications">Classifications for each record in the dataset</param>
        /// <returns type="BranchNode">BranchNode object representing a node in the tree</returns>
        internal void GrowTree(IEnumerable<object> r_COLLdata, IEnumerable<object> r_COLLclassifications)
        {
            //We need the total count of records in our set and the unique classifications available for those records
            int l_INTsamples = r_COLLdata.Count();
            HashSet<object> l_COLLuniqueClasses = r_COLLclassifications.ToHashSet();

            //If we don't have what we need in order to split or if our tree has reached it's max depth we produce a leafnode
            if (l_COLLuniqueClasses.Count() == 1 || l_INTsamples < c_INTminSamplesSplit || (c_INTmaxDepth > 0 && c_INTdepth >= c_INTmaxDepth))
            {
                BranchNode l_OBJleaf = GrowLeaf(r_COLLclassifications);
                RootNode = l_OBJleaf;
            }
            else
            {
                NodeBud l_OBJbranchBud = GetBestSplit(r_COLLdata, r_COLLclassifications);

                //if (l_OBJbranchBud != null)
                //{
                //    Console.WriteLine();
                //    Console.WriteLine($"Impurity: {l_OBJbranchBud.Impurity}\nFeatureIndex: {l_OBJbranchBud.FeatureIndex}\nThreshold: {l_OBJbranchBud.Threshold}\nLeftSplit: ");

                //    foreach (KeyValuePair<object, object> value in l_OBJbranchBud.LeftSplit)
                //    {
                //        Console.Write($"{(value.Value)}, ");
                //    }

                //    Console.WriteLine("\nRightSplit: ");

                //    foreach (KeyValuePair<object, object> value in l_OBJbranchBud.RightSplit)
                //    {
                //        Console.Write($"{(value.Value)}, ");
                //    }
                //    Console.WriteLine("");
                //}

                if (l_OBJbranchBud is null)
                {
                    BranchNode l_OBJleaf = GrowLeaf(r_COLLclassifications);
                    RootNode = l_OBJleaf;
                    //Console.WriteLine();
                    //Console.WriteLine($"Leaf created with value {l_OBJleaf.Value.ToString()}");
                }
                else
                {
                    double threshold;
                    if (double.TryParse(l_OBJbranchBud.Threshold.ToString(), out threshold))
                    {
                        RootNode = new BranchNode(true, l_OBJbranchBud.FeatureIndex, l_OBJbranchBud.Threshold);

                        DecisionTree l_OBJleftTree = new DecisionTree(c_INTmaxDepth, c_INTdepth + 1);
                        DecisionTree l_OBJrightTree = new DecisionTree(c_INTmaxDepth, c_INTdepth + 1);

                        RootNode.LeftSubtree = l_OBJleftTree;
                        RootNode.RightSubtree = l_OBJrightTree;

                        l_OBJleftTree.GrowTree(l_OBJbranchBud.LeftSplit.Select(row => { return row.Value; }), l_OBJbranchBud.LeftSplit.Select(row => { return row.Key; }));
                        l_OBJrightTree.GrowTree(l_OBJbranchBud.RightSplit.Select(row => { return row.Value; }), l_OBJbranchBud.RightSplit.Select(row => { return row.Key; }));
                    }

                    else
                    {
                        RootNode = new BranchNode(false, l_OBJbranchBud.FeatureIndex, l_OBJbranchBud.Threshold);

                        DecisionTree l_OBJleftTree = new DecisionTree(c_INTmaxDepth, c_INTdepth + 1);
                        DecisionTree l_OBJrightTree = new DecisionTree(c_INTmaxDepth, c_INTdepth + 1);

                        RootNode.LeftSubtree = l_OBJleftTree;
                        RootNode.RightSubtree = l_OBJrightTree;

                        l_OBJleftTree.GrowTree(l_OBJbranchBud.LeftSplit.Select(row => { return row.Value; }), l_OBJbranchBud.LeftSplit.Select(row => { return row.Key; }));
                        l_OBJrightTree.GrowTree(l_OBJbranchBud.RightSplit.Select(row => { return row.Value; }), l_OBJbranchBud.RightSplit.Select(row => { return row.Key; }));
                    }
                }
            }
        }

        internal void GrowTreeRF(IEnumerable<object> r_COLLdata, IEnumerable<object> r_COLLclassifications)
        {
            //We need the total count of records in our set and the unique classifications available for those records
            int l_INTsamples = r_COLLdata.Count();

            HashSet<object> l_COLLuniqueClasses = r_COLLclassifications.ToHashSet();

            //If we don't have what we need in order to split or if our tree has reached it's max depth we produce a leafnode
            if (l_COLLuniqueClasses.Count() == 1 || l_INTsamples < c_INTminSamplesSplit || (c_INTmaxDepth > 0 && c_INTdepth >= c_INTmaxDepth))
            {
                BranchNode l_OBJleaf = GrowLeaf(r_COLLclassifications);

                RootNode = l_OBJleaf;

                //Console.WriteLine(c_INTdepth);
            }

            NodeBud l_OBJbranchBud = GetBestSplit(SubsetFeatures(r_COLLdata), r_COLLclassifications);

            if (l_OBJbranchBud is null)
            {
                BranchNode l_OBJleaf = GrowLeaf(r_COLLclassifications);

                RootNode = l_OBJleaf;

                //Console.WriteLine(c_INTdepth);
            }

            else
            {
                decimal threshold;

                if (decimal.TryParse(l_OBJbranchBud.Threshold.ToString(), out threshold))
                {
                    RootNode = new BranchNode(true, l_OBJbranchBud.FeatureIndex, l_OBJbranchBud.Threshold);
                }

                else
                {
                    RootNode = new BranchNode(false, l_OBJbranchBud.FeatureIndex, l_OBJbranchBud.Threshold);
                }

                if (l_OBJbranchBud.LeftSplit.Count > 0)
                {
                    DecisionTree l_OBJleftTree = new DecisionTree(c_INTmaxDepth, c_INTdepth + 1);
                    RootNode.LeftSubtree = l_OBJleftTree;
                    l_OBJleftTree.GrowTreeRF(l_OBJbranchBud.LeftSplit.Select((row, index) => { return r_COLLdata.ElementAt(index); }), l_OBJbranchBud.LeftSplit.Select(row => { return row.Key; }));
                }
                else
                {
                    DecisionTree l_OBJleftTree = new DecisionTree(c_INTmaxDepth, c_INTdepth + 1);
                    l_OBJleftTree.RootNode = GrowLeaf(r_COLLclassifications);
                    RootNode.LeftSubtree = l_OBJleftTree;
                }

                if (l_OBJbranchBud.RightSplit.Count > 0)
                {
                    DecisionTree l_OBJrightTree = new DecisionTree(c_INTmaxDepth, c_INTdepth + 1);
                    RootNode.RightSubtree = l_OBJrightTree;
                    l_OBJrightTree.GrowTreeRF(l_OBJbranchBud.RightSplit.Select((row, index) => { return r_COLLdata.ElementAt(index); }), l_OBJbranchBud.RightSplit.Select(row => { return row.Key; }));
                }
                else
                {
                    DecisionTree l_OBJrightTree = new DecisionTree(c_INTmaxDepth, c_INTdepth + 1);
                    l_OBJrightTree.RootNode = GrowLeaf(r_COLLclassifications);
                    RootNode.RightSubtree = l_OBJrightTree;
                }
            }
        }

        private IEnumerable<object> SubsetFeatures(IEnumerable<object> r_COLLdata)
        {
            if (r_COLLdata.First() is Array)
            {
                Array l_COLLfirstArray = (Array)r_COLLdata.First();

                int l_INTnumFeatures = l_COLLfirstArray.Length;

                int l_INTnumFeaturesOutput = l_INTnumFeatures / 3;

                Random l_OBJrandom = new Random();

                List<int> l_COLLindexes = new List<int>();

                while (l_COLLindexes.Count < l_INTnumFeaturesOutput)
                {
                    int l_INTnextRando = l_OBJrandom.Next(l_INTnumFeatures);

                    if (!l_COLLindexes.Contains(l_INTnextRando))
                    {
                        l_COLLindexes.Add(l_INTnextRando);
                    }
                }

                return r_COLLdata.Select(row =>
                {
                    Array l_COLLarray = (Array)row;

                    object[] l_COLLnewArray = new object[l_INTnumFeaturesOutput];

                    for (int i = 0; i < l_COLLindexes.Count; i++)
                    {
                        l_COLLnewArray[i] = l_COLLarray.GetValue(l_COLLindexes[i]);
                    }
                    return (object)l_COLLnewArray;
                });
            }
            else
            {
                int l_INTnumFeatures = 0;

                int l_INTnumFeaturesOutput = 0;

                Random l_OBJrandom = new Random();

                IEnumerable<List<object>> l_COLLbuffer = r_COLLdata.Select(row =>
                {
                    PropertyInfo[] props = row.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    List<object> values = new List<object>();

                    foreach (PropertyInfo prop in props)
                    {
                        values.Add(prop.GetValue(row, null));
                    }

                    l_INTnumFeatures = values.Count;

                    l_INTnumFeaturesOutput = l_INTnumFeatures / 2;

                    return values;
                }).ToList();

                List<int> l_COLLindexes = new List<int>();

                while (l_COLLindexes.Count < l_INTnumFeaturesOutput)
                {
                    int l_INTnextRando = l_OBJrandom.Next(l_INTnumFeatures);

                    if (!l_COLLindexes.Contains(l_INTnextRando))
                    {
                        l_COLLindexes.Add(l_INTnextRando);
                    }
                }

                IEnumerable<object> x = l_COLLbuffer.Select(row =>
                {
                    Array l_COLLarray = row.ToArray();

                    object[] l_COLLnewArray = new object[l_INTnumFeaturesOutput];

                    for (int i = 0; i < l_COLLindexes.Count; i++)
                    {
                        l_COLLnewArray[i] = l_COLLarray.GetValue(l_COLLindexes[i]);
                    }

                    return (object)l_COLLnewArray;
                }).ToList();

                return x;
            }
        }

        /// <summary>
        /// Takes a collection of features and returns the Gini Impurity which is 1 - the sum of the proportion of records containing the each feature squared.
        /// 1 - ( (rowsWithFeatureOne/totalRows)^2 + (rowsWithFeatureTwo/totalRows)^2...+ (rowsWithFeatureN/totalRows)^2 )
        /// </summary>
        /// <param name="l_ENUMclassifications"></param>
        /// <returns type="double">double representing Gini Impurity for the collection.</returns>
        internal double CalcGiniImpurity(IEnumerable<object> l_ENUMclassifications)
        {

            double l_DBLimpurity = 1.00;
            int l_INTclassCount = l_ENUMclassifications.Count();

            //The impurity of an empty set is 0
            if (l_INTclassCount == 0)
            {
                l_DBLimpurity = 0;
            }

            //Count the occurences
            Dictionary<object, int> l_DICTclassCounts = l_ENUMclassifications.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

            //Perform the calculation
            foreach (object classification in l_DICTclassCounts.Keys)
            {
                double l_DBLproportion = (double)l_DICTclassCounts[classification] / l_INTclassCount;
                l_DBLimpurity -= l_DBLproportion * l_DBLproportion;
            }

            return l_DBLimpurity;
        }

        /// <summary>
        /// Determines the best way to split at 
        /// </summary>
        /// <param name="l_COLLdataSet"></param>
        /// <param name="l_COLLclassifications"></param>
        /// <returns></returns>
        internal NodeBud GetBestSplit(IEnumerable<object> l_COLLdataSet, IEnumerable<object> l_COLLclassifications)
        {
            NodeBud l_OBJbranchBud = null;

            if (l_COLLdataSet.Count() <= 1)
            {
                return l_OBJbranchBud;
            }

            IEnumerable<object> l_COLLconvertedCollections = new List<object>();

            foreach (object obj in l_COLLdataSet)
            {
                if (obj is IEnumerable enumerable)
                {
                    l_OBJbranchBud = ProcessArrays(l_COLLdataSet, l_COLLclassifications);
                    break;
                }
                else
                {
                    PropertyInfo[] props = obj.GetType().GetProperties();
                    List<object> values = new List<object>();
                    foreach (PropertyInfo prop in props)
                    {
                        values.Add(prop.GetValue(obj, null));
                    }
                    l_COLLconvertedCollections.Append(values);
                }
            }

            if (l_COLLconvertedCollections.Count() > 0)
            {
                l_OBJbranchBud = ProcessArrays(l_COLLconvertedCollections, l_COLLclassifications);
            }

            return l_OBJbranchBud;
        }

        private NodeBud ProcessArrays(IEnumerable<object> l_COLLconvertedArrays, IEnumerable<object> l_COLLclassifications)
        {
            int l_INTbestFeatureIndex = -1;
            object l_OBJbestThreshold = null;
            List<KeyValuePair<object, object>> l_COLLleftPairs = new List<KeyValuePair<object, object>>();
            List<KeyValuePair<object, object>> l_COLLrightPairs = new List<KeyValuePair<object, object>>();

            double l_DBLimpurity = Double.PositiveInfinity;

            IEnumerable<object> l_OBJfirstCollection = new List<object>();

            if (l_COLLconvertedArrays.First() is IEnumerable enumerable)
            {
                l_OBJfirstCollection = enumerable.Cast<object>();
            }

            int l_INTnumFeatures = l_OBJfirstCollection.Count();

            foreach (int i in Enumerable.Range(0, l_INTnumFeatures))
            {
                double num = 0;
                if (double.TryParse(l_OBJfirstCollection.First().ToString(), out num))
                {
                    HashSet<double?> l_COLLthresholds = new HashSet<double?>(
                        l_COLLconvertedArrays
                        .Select(row => double.TryParse((row as Array)?.GetValue(i)?.ToString(), out num) ? num : (double?)null)
                        .Where(v => v.HasValue)
                    );

                    foreach (double l_DBLthreshold in l_COLLthresholds)
                    {
                        Dictionary<int, List<KeyValuePair<object, object>>> l_COLLsplits = l_COLLconvertedArrays
                            .Select((row, index) =>
                            {
                                double? value = null;
                                object key = null;
                                if (row is Array arr && double.TryParse(arr.GetValue(i)?.ToString(), out double parsedValue))
                                {
                                    value = parsedValue;
                                    key = l_COLLclassifications.ElementAt(index);
                                }
                                return value.HasValue
                                    ? (value <= l_DBLthreshold ? (0, new KeyValuePair<object, object>(key, row)) : (1, new KeyValuePair<object, object>(key, row)))
                                    : (-1, new KeyValuePair<object, object>(null, null));
                            })
                            .Where(pair => pair.Item1 >= 0)
                            .GroupBy(pair => pair.Item1)
                            .ToDictionary(g => g.Key, g => g.Select(v => v.Item2).ToList());

                        if (!l_COLLsplits.ContainsKey(0))
                        {
                            l_COLLsplits[0] = new List<KeyValuePair<object, object>>();
                        }
                        if (!l_COLLsplits.ContainsKey(1))
                        {
                            l_COLLsplits[1] = new List<KeyValuePair<object, object>>();
                        }

                        double l_DBLleftImpurity = l_COLLsplits[0].Any() ? CalcGiniImpurity(l_COLLsplits[0].Select(row => { return row.Key; })) : 0;

                        double l_DBLrightImpurity = l_COLLsplits[1].Any() ? CalcGiniImpurity(l_COLLsplits[1].Select(row => { return row.Key; })) : 0;

                        double l_DBLweightedImpurity = (l_COLLsplits[0].Count() * l_DBLleftImpurity + l_COLLsplits[1].Count() * l_DBLrightImpurity) / l_COLLconvertedArrays.Count();

                        if (l_DBLweightedImpurity < l_DBLimpurity)
                        {
                            l_DBLimpurity = l_DBLweightedImpurity;
                            l_INTbestFeatureIndex = i;
                            l_OBJbestThreshold = l_DBLthreshold;
                            l_COLLleftPairs = l_COLLsplits[0];
                            l_COLLrightPairs = l_COLLsplits[1];
                        }
                    }
                }

                else
                {
                    HashSet<object?> l_COLLthresholds = new HashSet<object?>(l_COLLconvertedArrays
                        .Select(row => (row as Array)?.GetValue(i))
                        .ToList()
                    );

                    foreach (object l_OBJthreshold in l_COLLthresholds)
                    {
                        Dictionary<int, List<KeyValuePair<object, object?>>> l_COLLsplits = l_COLLconvertedArrays
                            .Select((row, index) =>
                            {
                                object? value = null;
                                object key = null;
                                if (row is Array arr)
                                {
                                    value = arr.GetValue(i);
                                    key = l_COLLclassifications.ElementAt(index);
                                }
                                return (value == l_OBJthreshold) ? (0, new KeyValuePair<object, object?>(key, row)) : (1, new KeyValuePair<object, object?>(key, row));
                            })
                            .Where(pair => pair.Item1 >= 0)
                            .GroupBy(pair => pair.Item1)
                            .ToDictionary(g => g.Key, g => g.Select(v => v.Item2).ToList());

                        if (!l_COLLsplits.ContainsKey(0))
                        {
                            l_COLLsplits[0] = new List<KeyValuePair<object, object?>>();
                        }
                        if (!l_COLLsplits.ContainsKey(1))
                        {
                            l_COLLsplits[1] = new List<KeyValuePair<object, object?>>();
                        }

                        double l_DBLleftImpurity = l_COLLsplits[0].Any() ? CalcGiniImpurity(l_COLLsplits[0].Select(row => { return row.Key; })) : 0;
                        double l_DBLrightImpurity = l_COLLsplits[1].Any() ? CalcGiniImpurity(l_COLLsplits[1].Select(row => { return row.Key; })) : 0;

                        double l_DBLweightedImpurity = (l_COLLsplits[0].Count() * l_DBLleftImpurity + l_COLLsplits[1].Count() * l_DBLrightImpurity) / l_COLLconvertedArrays.Count();

                        if (l_DBLweightedImpurity < l_DBLimpurity)
                        {
                            l_DBLimpurity = l_DBLweightedImpurity;
                            l_INTbestFeatureIndex = i;
                            l_OBJbestThreshold = l_OBJthreshold;
                            l_COLLleftPairs = l_COLLsplits[0];
                            l_COLLrightPairs = l_COLLsplits[1];
                        }
                    }
                }
            }
            return new NodeBud(l_DBLimpurity, l_INTbestFeatureIndex, l_OBJbestThreshold, l_COLLleftPairs, l_COLLrightPairs);
        }

        private BranchNode GrowLeaf(IEnumerable<object> r_COLLclassifications)
        {
            var l_COLLclassCounts = r_COLLclassifications.GroupBy(x => x).Select(y => new { Classification = y.Key, Count = y.Count() }).ToList();
            int l_INTcommonality = 0;
            object l_OBJclass = null;

            foreach (var obj in l_COLLclassCounts)
            {
                if (obj.Count > l_INTcommonality)
                {
                    l_INTcommonality = obj.Count;
                    l_OBJclass = obj.Classification;
                }
            }
            decimal l_DECout;
            if (decimal.TryParse(l_OBJclass.ToString(), out l_DECout))
            {
                return new BranchNode(true, -1, null, l_OBJclass, null, null);
            }
            else
            {
                return new BranchNode(false, -1, null, l_OBJclass, null, null);
            }
        }

        internal object Predict(IEnumerable<object> r_COLLfeatures)
        {
            return RootNode.Predict(r_COLLfeatures);
        }
    }
}