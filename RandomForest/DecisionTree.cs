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

        /// <summary>
        /// This is a recursive method that will grow the decision tree until it either
        /// reaches it's max depth, each path ends in a leaf, or we don't have enough values to split on.
        /// </summary>
        /// <param name="r_COLLdata">Dataset we are creating our tree from</param>
        /// <param name="r_COLLclassifications">classifications for the rows in our dataset</param>
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
            }

            //Find the best split for this node
            NodeBud l_OBJbranchBud = GetBestSplit(SubsetFeatures(r_COLLdata), r_COLLclassifications);

            //If the splitting function failed we create a leaf
            if (l_OBJbranchBud is null)
            {
                BranchNode l_OBJleaf = GrowLeaf(r_COLLclassifications);

                RootNode = l_OBJleaf;
            }

            //otherwise we produce out BranchNode
            else
            {
                decimal threshold;

                //if the threshold is a numeric value this is a numeric node
                if (decimal.TryParse(l_OBJbranchBud.Threshold.ToString(), out threshold))
                {
                    RootNode = new BranchNode(true, l_OBJbranchBud.FeatureIndex, l_OBJbranchBud.Threshold);
                }

                //otherwise it's not a numeric node
                else
                {
                    RootNode = new BranchNode(false, l_OBJbranchBud.FeatureIndex, l_OBJbranchBud.Threshold);
                }

                //if we have data for the left branch we produce a subtree based on that data
                if (l_OBJbranchBud.LeftSplit.Count > 0)
                {
                    DecisionTree l_OBJleftTree = new DecisionTree(c_INTmaxDepth, c_INTdepth + 1);
                    RootNode.LeftSubtree = l_OBJleftTree;
                    l_OBJleftTree.GrowTreeRF(l_OBJbranchBud.LeftSplit.Select((row, index) => { return r_COLLdata.ElementAt(index); }), l_OBJbranchBud.LeftSplit.Select(row => { return row.Key; }));
                }
                //if not the left branch ends in a leaf
                else
                {
                    DecisionTree l_OBJleftTree = new DecisionTree(c_INTmaxDepth, c_INTdepth + 1);
                    l_OBJleftTree.RootNode = GrowLeaf(r_COLLclassifications);
                    RootNode.LeftSubtree = l_OBJleftTree;
                }

                //if we have data for the right branch we produce a subtree based on that data
                if (l_OBJbranchBud.RightSplit.Count > 0)
                {
                    DecisionTree l_OBJrightTree = new DecisionTree(c_INTmaxDepth, c_INTdepth + 1);
                    RootNode.RightSubtree = l_OBJrightTree;
                    l_OBJrightTree.GrowTreeRF(l_OBJbranchBud.RightSplit.Select((row, index) => { return r_COLLdata.ElementAt(index); }), l_OBJbranchBud.RightSplit.Select(row => { return row.Key; }));
                }
                //otherwise the right branch ends in a leaf
                else
                {
                    DecisionTree l_OBJrightTree = new DecisionTree(c_INTmaxDepth, c_INTdepth + 1);
                    l_OBJrightTree.RootNode = GrowLeaf(r_COLLclassifications);
                    RootNode.RightSubtree = l_OBJrightTree;
                }
            }
        }

        /// <summary>
        /// Method to create a subset of available features. Method is a little clunky as when I designed this library I initially tried to make it handle objects and arrays.
        /// Were I to create further iterations I would likely abandon this methodology and instead force the end user to put their data into DataTable objects to allow better control over data handling within and between methods.
        /// </summary>
        /// <param name="r_COLLdata">Collection of data we're working with</param>
        /// <returns>Subset of <paramref name="r_COLLdata"/> containing only a partial number of the available features</returns>
        private IEnumerable<object> SubsetFeatures(IEnumerable<object> r_COLLdata)
        {
            //First we check if the data is in arrays or objects
            if (r_COLLdata.First() is Array)
            {
                //if we are dealing with arrays, we use the first one to get our list of features
                Array l_COLLfirstArray = (Array)r_COLLdata.First();

                int l_INTnumFeatures = l_COLLfirstArray.Length;

                //We're using roughly 1/3 of the available features at each node
                int l_INTnumFeaturesOutput = l_INTnumFeatures / 3;

                //Random value for selecting features
                Random l_OBJrandom = new Random();

                //list to hold indices of selected features
                List<int> l_COLLindexes = new List<int>();

                //loop through random feature indices until we have the requisite number for our subset.
                while (l_COLLindexes.Count < l_INTnumFeaturesOutput)
                {
                    int l_INTnextRando = l_OBJrandom.Next(l_INTnumFeatures);

                    if (!l_COLLindexes.Contains(l_INTnextRando))
                    {
                        l_COLLindexes.Add(l_INTnextRando);
                    }
                }

                //Build our new data collection including only the selected features.
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
            //if we don't have a collection of arrays we assume it's a collection of objects where the properties = our data features
            else
            {
                //variable to hold the number of features present
                int l_INTnumFeatures = 0;
                //variable to hold the number of features present in our output
                int l_INTnumFeaturesOutput = 0;
                //for randomness
                Random l_OBJrandom = new Random();

                //use the first object in the list to get our list of features
                IEnumerable<List<object>> l_COLLbuffer = r_COLLdata.Select(row =>
                {
                    //using reflection to get a list of properties.
                    PropertyInfo[] props = row.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    List<object> values = new List<object>();

                    foreach (PropertyInfo prop in props)
                    {
                        values.Add(prop.GetValue(row, null));
                    }

                    l_INTnumFeatures = values.Count;

                    //use roughly 1/3 of the available features
                    l_INTnumFeaturesOutput = l_INTnumFeatures / 3;

                    return values;
                }).ToList();

                //collection to hold indexes of features
                List<int> l_COLLindexes = new List<int>();

                //loop through random feature indices until we have the requisite number for our subset.
                while (l_COLLindexes.Count < l_INTnumFeaturesOutput)
                {
                    int l_INTnextRando = l_OBJrandom.Next(l_INTnumFeatures);

                    if (!l_COLLindexes.Contains(l_INTnextRando))
                    {
                        l_COLLindexes.Add(l_INTnextRando);
                    }
                }

                //Build our new data collection including only the selected features.
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
        /// Determines the best way to split for <paramref name="l_COLLdataSet"/> and <paramref name="l_COLLclassifications"/> 
        /// </summary>
        /// <param name="l_COLLdataSet">Data set we're trying to split</param>
        /// <param name="l_COLLclassifications">Values we are trying to analyze</param>
        /// <returns>A NodeBud object representing the best way to split at this node</returns>
        internal NodeBud GetBestSplit(IEnumerable<object> l_COLLdataSet, IEnumerable<object> l_COLLclassifications)
        {
            //object we'll be returning
            NodeBud l_OBJbranchBud = null;

            //if we have no data, we can't perform a split and we'll return null, calling method must handle this
            if (l_COLLdataSet.Count() <= 1)
            {
                return l_OBJbranchBud;
            }

            //things get considerably more clunky beyond this point but I was to far in with to little time to go back and re-factor everything.

            //This collection is for holding the arrays we'll pass to the ProcessArrays() method.
            IEnumerable<object> l_COLLconvertedCollections = new List<object>();

            foreach (object obj in l_COLLdataSet)
            {
                //we figure out if the objects are already arrays, if so we pass them directly into the ProcessArrays() method
                if (obj is IEnumerable enumerable)
                {
                    //result of ProcessArrays is assigned directly to our return value
                    l_OBJbranchBud = ProcessArrays(l_COLLdataSet, l_COLLclassifications);
                    break;
                }
                //Otherwise we convert them.
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

            //if we successfully converted objects to arrays, we pass those arrays to the ProcessArrays() method and assign the result directly to our return value
            if (l_COLLconvertedCollections.Count() > 0)
            {
                l_OBJbranchBud = ProcessArrays(l_COLLconvertedCollections, l_COLLclassifications);
            }

            return l_OBJbranchBud;
        }

        /// <summary>
        /// This is by far the clunkiest method in the whole library.
        /// It takes the collection of data as arrays and calculates the best split.
        /// This method has two main branches, one that deals with numeric data and one that deals with non-numeric data(all treated as categorical data for our purposes)
        /// </summary>
        /// <param name="l_COLLconvertedArrays">data set we're processing</param>
        /// <param name="l_COLLclassifications">the analyzed value for each row of data</param>
        /// <returns>A NodeBud object representing the best way to split at this node</returns>
        private NodeBud ProcessArrays(IEnumerable<object> l_COLLconvertedArrays, IEnumerable<object> l_COLLclassifications)
        {
            //variables for tracking the best feature and the splits associated with it
            int l_INTbestFeatureIndex = -1;
            object l_OBJbestThreshold = null;
            List<KeyValuePair<object, object>> l_COLLleftPairs = new List<KeyValuePair<object, object>>();
            List<KeyValuePair<object, object>> l_COLLrightPairs = new List<KeyValuePair<object, object>>();

            //Initial impurity is set to Infinity to ensure it is greater than whatever our intial calculation is.
            double l_DBLimpurity = Double.PositiveInfinity;

            //we use the first array in the collection to get our feature count and to test if each feature is a numeric feature or not.
            IEnumerable<object> l_OBJfirstCollection = new List<object>();

            //cast our first array to an array of objects, clunky, not elegant, but the overall product functions. Future iterations will have better handling of types and data.
            if (l_COLLconvertedArrays.First() is IEnumerable enumerable)
            {
                l_OBJfirstCollection = enumerable.Cast<object>();
            }

            //Get the count of available features
            int l_INTnumFeatures = l_OBJfirstCollection.Count();

            //loop through each feature to find the best one on which to split
            foreach (int i in Enumerable.Range(0, l_INTnumFeatures))
            {
                //first we determine if this is a numeric feature
                double num = 0;
                if (double.TryParse(l_OBJfirstCollection.First().ToString(), out num))
                {
                    //Get a hashset of all the unique values for this feature
                    HashSet<double?> l_COLLthresholds = new HashSet<double?>(
                        l_COLLconvertedArrays
                        .Select(row => double.TryParse((row as Array)?.GetValue(i)?.ToString(), out num) ? num : (double?)null)
                        .Where(v => v.HasValue)
                    );

                    //check each value to see which one splits the full data set best
                    foreach (double l_DBLthreshold in l_COLLthresholds)
                    {
                        //This series of LINQ statements will split the dataset into values greater than the current threshold, and values less than the current threshold
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

                        //These two if blocks are necessary incase we have a split in which all the values go on one branch
                        if (!l_COLLsplits.ContainsKey(0))
                        {
                            l_COLLsplits[0] = new List<KeyValuePair<object, object>>();
                        }
                        if (!l_COLLsplits.ContainsKey(1))
                        {
                            l_COLLsplits[1] = new List<KeyValuePair<object, object>>();
                        }

                        //Perform the impurity calculations, we have used weighted Gini Impurity for our model.
                        double l_DBLleftImpurity = l_COLLsplits[0].Any() ? CalcGiniImpurity(l_COLLsplits[0].Select(row => { return row.Key; })) : 0;

                        double l_DBLrightImpurity = l_COLLsplits[1].Any() ? CalcGiniImpurity(l_COLLsplits[1].Select(row => { return row.Key; })) : 0;

                        double l_DBLweightedImpurity = (l_COLLsplits[0].Count() * l_DBLleftImpurity + l_COLLsplits[1].Count() * l_DBLrightImpurity) / l_COLLconvertedArrays.Count();

                        //see if this split is better than the current best
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
                //If it's not a numeric feature we treat it as categorical
                else
                {
                    //Get a hashset of all the unique values for the feature
                    HashSet<object?> l_COLLthresholds = new HashSet<object?>(l_COLLconvertedArrays
                        .Select(row => (row as Array)?.GetValue(i))
                        .ToList()
                    );

                    //test each unique value as a threshold
                    foreach (object l_OBJthreshold in l_COLLthresholds)
                    {
                        //this series of LINQ statements will split the dataset into values equal to the threshold and values not equal to the threshold
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

                        //these if blocks are necessary incase all the data flows into one branch.
                        if (!l_COLLsplits.ContainsKey(0))
                        {
                            l_COLLsplits[0] = new List<KeyValuePair<object, object?>>();
                        }
                        if (!l_COLLsplits.ContainsKey(1))
                        {
                            l_COLLsplits[1] = new List<KeyValuePair<object, object?>>();
                        }

                        //Perform the impurity calculations, we have used weighted Gini Impurity for our model.
                        double l_DBLleftImpurity = l_COLLsplits[0].Any() ? CalcGiniImpurity(l_COLLsplits[0].Select(row => { return row.Key; })) : 0;
                        double l_DBLrightImpurity = l_COLLsplits[1].Any() ? CalcGiniImpurity(l_COLLsplits[1].Select(row => { return row.Key; })) : 0;

                        double l_DBLweightedImpurity = (l_COLLsplits[0].Count() * l_DBLleftImpurity + l_COLLsplits[1].Count() * l_DBLrightImpurity) / l_COLLconvertedArrays.Count();

                        //see if this split is better than the current best
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
            //create and return our NodeBud
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