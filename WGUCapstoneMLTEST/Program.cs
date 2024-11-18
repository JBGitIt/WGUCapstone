// See https://aka.ms/new-console-template for more information
using RandomForest;
using Microsoft.Data.SqlClient;
using System.Data;


//List<object> testDataSet = new List<object>
//{
//    new double[] { 60, 110 },
//    new double[] { 62, 115 },
//    new double[] { 64, 120 },
//    new double[] { 65, 140 },
//    new double[] { 66, 150 }, 
//    new double[] { 70, 160 }
//};

//List<object> classifications = new List<object> { 0, 0, 0, 1, 1, 1};

//List<object> testDataSet2 = new List<object>
//{
//    new string[] {"Red"},
//    new string[] {"Blue"},
//    new string[] {"Green"},
//    new string[] {"Red"},
//    new string[] {"Green"},
//    new string[] {"Blue"},
//};

//List<object> classifications2 = new List<object> { 1, 0, 0, 1, 0, 0 };

//List<object> testDataSet3 = new List<object>
//{
//    new object[] { 25, "USA" },
//    new object[] { 30, "Canada" },
//    new object[] { 45, "USA" },
//    new object[] { 50, "Canada" },
//};

//List<object> classifications3 = new List<object> { 1, 1, 0, 0 };

//object[][] features = new object[][]
//{
//    new object[] { 2, 3, 1, 4, 7, 2, 5, 1, 3, 8, 6, "A" },
//    new object[] { 5, 1, 3, 2, 4, 6, 8, 5, 7, 1, 9, "B" },
//    new object[] { 3, 2, 4, 1, 9, 3, 4, 6, 2, 7, 5, "C" },
//    new object[] { 4, 5, 2, 3, 6, 4, 7, 2, 8, 5, 3, "A" },
//    new object[] { 1, 6, 5, 8, 3, 7, 6, 4, 1, 2, 4, "B" },
//    new object[] { 8, 4, 7, 6, 2, 9, 3, 7, 5, 4, 1, "C" },
//    new object[] { 6, 8, 1, 5, 7, 1, 2, 8, 3, 6, 9, "A" },
//    new object[] { 7, 9, 3, 4, 8, 5, 1, 9, 2, 3, 7, "B" },
//    new object[] { 9, 7, 5, 1, 4, 2, 8, 3, 6, 5, 2, "C" },
//    new object[] { 2, 6, 4, 9, 5, 3, 9, 2, 4, 7, 8, "A" },
//    new object[] { 3, 5, 7, 2, 6, 4, 5, 3, 7, 9, 1, "B" },
//    new object[] { 1, 8, 9, 4, 3, 7, 6, 5, 2, 4, 6, "C" },
//    new object[] { 4, 2, 6, 5, 1, 8, 9, 7, 3, 1, 3, "A" },
//    new object[] { 5, 3, 8, 7, 2, 6, 4, 8, 5, 2, 9, "B" },
//    new object[] { 7, 1, 3, 6, 8, 9, 2, 6, 9, 5, 4, "C" },
//    new object[] { 6, 4, 2, 8, 5, 1, 7, 9, 4, 8, 3, "A" },
//    new object[] { 8, 9, 5, 7, 3, 4, 1, 2, 5, 6, 7, "B" },
//    new object[] { 9, 7, 4, 3, 6, 8, 3, 4, 8, 7, 2, "C" },
//    new object[] { 2, 5, 6, 1, 4, 2, 8, 3, 1, 9, 5, "A" },
//    new object[] { 3, 6, 7, 5, 2, 3, 6, 1, 7, 4, 8, "B" }
//};

//IEnumerable<object> classifications = new List<object>
//{
//    1, 0, 0, 1, 1, 0, 0, 1, 0, 1,
//    1, 0, 0, 1, 0, 1, 1, 0, 1, 0
//};

// Random number generator for test data
Random rand = new Random();

List<object> l_COLLdata = new List<object>();
List<object> l_COLLclassifications = new List<object>();
List<object> l_COLLclassificationAmounts = new List<object>();
List<object> l_COLLdataAmounts = new List<object>();
List<DateTime> l_COLLdates = new List<DateTime>();
//using (SqlConnection l_OBJsqlConn = new SqlConnection("Server=MVFCU-SQL;Database=MVFCUCustom_Dev;Integrated Security=SSPI"))
using(SqlConnection l_OBJsqlConn = new SqlConnection("Server=localhost;Database=WGUCapstone;Integrated Security=SSPI;TrustServerCertificate=True"))
{
    l_OBJsqlConn.Open();

    SqlCommand l_OBJsqlCmd = l_OBJsqlConn.CreateCommand();
    l_OBJsqlCmd.CommandText = "dbo.spSEL_GetData";
    l_OBJsqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

    using (SqlDataReader l_OBJsqlReader = l_OBJsqlCmd.ExecuteReader())
    {
        while (l_OBJsqlReader.Read())
        {
            l_COLLdata.Add(new object[]
            {
                 l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_BUSLOANSPercentChange"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_CPIAUCSLPercentChange"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_DPRIMEPercentChange"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_DPSACBW027SBOGPercentChange"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_EXPGSPercentChange"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_IMPGSPercentChange"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_RHEACBW027SBOGPercentChange"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_TLRESCONPercentChange"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_UNRATEPercentChange"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_GDPPercentChange"))
            });

            l_COLLclassifications.Add(l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T3_GDPPercentChange")));

            l_COLLdates.Add(l_OBJsqlReader.GetDateTime(l_OBJsqlReader.GetOrdinal("T2_Date")));

            l_COLLdataAmounts.Add(new object[]
            {
                 l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_BUSLOANSBillions"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_CPIAUCSL"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_DPRIMEPercent"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_DPSACBW027SBOGBillions"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_EXPGSBillions"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_IMPGSBillions"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_RHEACBW027SBOGBillions"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_TLRESCONBillions"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_UNRATEPercent"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_GDPBillions"))
            });

            l_COLLclassificationAmounts.Add(l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T3_GDPBillions")));
        }
    }

    l_OBJsqlConn.Close();
}

//// Collection for Vehicle instances
//List<Vehicle> vehicles = new List<Vehicle>();

//for (int i = 0; i < 100; i++)
//{
//    int doors = rand.Next(2, 5);  // Random between 2 and 4 doors
//    bool hasBed = rand.NextDouble() > 0.5; // 50% chance of having a bed
//    bool has4x4 = rand.NextDouble() > 0.5; // 50% chance of having 4x4
//    int horsePower = rand.Next(100, 500); // Random horsepower between 100 and 500
//    double price = rand.NextDouble() * (100000 - 15000) + 15000; // Random price between 15000 and 100000

//    vehicles.Add(new Vehicle(doors, hasBed, has4x4, horsePower, price));
//}

//// Collection for VehicleType classifications
//List<object> vehicleTypes = new List<object>();
//foreach (var vehicle in vehicles)
//{
//    if (vehicle.i_INTdoors < 3 && !vehicle.i_BOOLhasBed && !vehicle.i_BOOLhas4x4)
//        vehicleTypes.Add("Sports Car");//VehicleType.SPORTSCAR);
//    else if (vehicle.i_INTdoors > 2 && !vehicle.i_BOOLhasBed && !vehicle.i_BOOLhas4x4)
//        vehicleTypes.Add("Sedan");//VehicleType.SEDAN);
//    else if (vehicle.i_BOOLhasBed && vehicle.i_INTdoors < 3)
//        vehicleTypes.Add("Truck");//VehicleType.TRUCK);
//    else if (vehicle.i_BOOLhasBed && vehicle.i_INTdoors > 2)
//        vehicleTypes.Add("Crew Cab");//VehicleType.CREWCAB);
//    else if (vehicle.i_INTdoors > 2 && !vehicle.i_BOOLhasBed)
//        vehicleTypes.Add("Utility");
//    else if (vehicle.i_INTdoors < 3 && !vehicle.i_BOOLhasBed)
//        vehicleTypes.Add("Offroad");
//}

//Arborist l_OBJarborist = new Arborist(vehicles, vehicleTypes, 1000, 100, 2);

IEnumerable<object> l_COLLtrainData = l_COLLdata.Take(l_COLLdata.Count / 2);
IEnumerable<object> l_COLLtrainClassifications = l_COLLclassifications.Take(l_COLLdata.Count / 2);
IEnumerable<object> l_COLLtrainClassAmounts = l_COLLclassificationAmounts.Take(l_COLLclassificationAmounts.Count / 2);
IEnumerable<object> l_COLLtrainDataAmounts = l_COLLdataAmounts.Take(l_COLLdataAmounts.Count / 2);
List<DateTime> l_COLLtrainDates = l_COLLdates.Take(l_COLLdata.Count / 2).ToList();

IEnumerable<object> l_COLLtestData = l_COLLdata.Skip(l_COLLdata.Count / 2);
IEnumerable<object> l_COLLtestClassifications = l_COLLclassifications.Skip(l_COLLdata.Count / 2);
IEnumerable<object> l_COLLtestClassAmounts = l_COLLclassificationAmounts.Skip(l_COLLclassificationAmounts.Count / 2);
IEnumerable<object> l_COLLtestDataAmounts = l_COLLdataAmounts.Skip(l_COLLdataAmounts.Count / 2);
List<DateTime> l_COLLtestDates = l_COLLdates.Skip(l_COLLdataAmounts.Count / 2).ToList();


Arborist l_OBJarborist = new Arborist(l_COLLtrainData, l_COLLtrainClassifications, 1000, 100, 2);

Console.WriteLine("Planting Forest...");
l_OBJarborist.PlantForestTimeSeries(12, 12);
//l_OBJarborist.PlantForestCategorization(4);

DataTable l_OBJpredictionTable = new DataTable();
l_OBJpredictionTable.Columns.Add("ModelID", Type.GetType("System.Int32"));
l_OBJpredictionTable.Columns.Add("AsOfDate", Type.GetType("System.DateTime"));
l_OBJpredictionTable.Columns.Add("PredictedGDP", Type.GetType("System.Double"));
l_OBJpredictionTable.Columns.Add("RangeHigh", Type.GetType("System.Double"));
l_OBJpredictionTable.Columns.Add("RangeLow", Type.GetType("System.Double"));
l_OBJpredictionTable.Columns.Add("Actual", Type.GetType("System.Double"));
l_OBJpredictionTable.Columns.Add("isCorrect", Type.GetType("System.Boolean"));

Console.WriteLine("\nMaking Predictions...");
while(l_COLLtestData.Count() > 0)
{
    IEnumerable<object> l_COLLpredictionTest = (IEnumerable<object>)l_COLLtestData.First();
    object[] l_COLLresults = l_OBJarborist.Predict(l_COLLpredictionTest);

    double l_DBLprediction = ((double)l_COLLresults[0] / 100) * Convert.ToDouble((decimal)l_COLLtrainClassAmounts.Last()) + Convert.ToDouble((decimal)l_COLLtrainClassAmounts.Last());
    double l_DBLhigh = ((double)l_COLLresults[1] / 100) * Convert.ToDouble((decimal)l_COLLtrainClassAmounts.Last()) + Convert.ToDouble((decimal)l_COLLtrainClassAmounts.Last());
    double l_DBLlow = ((double)l_COLLresults[2] / 100) * Convert.ToDouble((decimal)l_COLLtrainClassAmounts.Last()) + Convert.ToDouble((decimal)l_COLLtrainClassAmounts.Last());

    Dictionary<string, double> l_COLLoutputProcessed = OutputProcessor.GetPredictiveRange(l_DBLhigh, l_DBLlow, l_DBLprediction, (int)l_COLLresults[3], (int)l_COLLresults[4]);

    DataRow l_OBJpredictionRow = l_OBJpredictionTable.NewRow();
    l_OBJpredictionRow[0] = 5;
    l_OBJpredictionRow[1] = l_COLLtestDates.First();
    l_OBJpredictionRow[2] = l_DBLprediction;
    l_OBJpredictionRow[3] = l_DBLhigh;
    l_OBJpredictionRow[4] = l_DBLlow;
    l_OBJpredictionRow[5] = l_COLLtestClassAmounts.First();
    l_OBJpredictionRow[6] = (l_DBLhigh > Convert.ToDouble((decimal)l_COLLtestClassAmounts.First()) && l_DBLlow < Convert.ToDouble((decimal)l_COLLtestClassAmounts.First()));
    l_OBJpredictionTable.Rows.Add(l_OBJpredictionRow);

    l_COLLtrainData = l_COLLtrainData.Append(l_COLLtestData.First());
    l_COLLtestData = l_COLLtestData.Skip(1);

    l_COLLtrainClassifications = l_COLLtrainClassifications.Append(l_COLLtestClassifications.First());
    l_COLLtestClassifications = l_COLLtestClassifications.Skip(1);

    l_COLLtrainClassAmounts = l_COLLtrainClassAmounts.Append(l_COLLtestClassAmounts.First());
    l_COLLtestClassAmounts = l_COLLtestClassAmounts.Skip(1);

    l_COLLtrainDates = l_COLLtrainDates.Append(l_COLLtestDates.First()).ToList();
    l_COLLtestDates = l_COLLtestDates.Skip(1).ToList();

    l_OBJarborist = new Arborist(l_COLLtrainData, l_COLLtrainClassifications, 1000, 100, 2);
    l_OBJarborist.PlantForestTimeSeries(12, 12);
}

using(SqlConnection l_OBJsqlConnection = new SqlConnection("Server=localhost;Database=WGUCapstone;Integrated Security=SSPI;TrustServerCertificate=True"))
{
    l_OBJsqlConnection.Open();

    SqlCommand l_OBJsqlCommand = l_OBJsqlConnection.CreateCommand();
    l_OBJsqlCommand.CommandType = CommandType.StoredProcedure;
    l_OBJsqlCommand.Parameters.Add(new SqlParameter("@PredictionResults", l_OBJpredictionTable));

    l_OBJsqlCommand.CommandText = "dbo.InsPredictionResults";

    l_OBJsqlCommand.ExecuteNonQuery();

    l_OBJsqlConnection.Close();
}

//double l_DBLprediction = ((double)l_COLLresults[0] / 100) * Convert.ToDouble((decimal)l_COLLtrainClassAmounts.Last()) + Convert.ToDouble((decimal)l_COLLtrainClassAmounts.Last());
//double l_DBLhigh = ((double)l_COLLresults[1] / 100) * Convert.ToDouble((decimal)l_COLLtrainClassAmounts.Last()) + Convert.ToDouble((decimal)l_COLLtrainClassAmounts.Last());
//double l_DBLlow = ((double)l_COLLresults[2] / 100) * Convert.ToDouble((decimal)l_COLLtrainClassAmounts.Last()) + Convert.ToDouble((decimal)l_COLLtrainClassAmounts.Last());
//Dictionary<string, double> l_COLLoutputProcessed = OutputProcessor.GetPredictiveRange(l_DBLhigh, l_DBLlow, l_DBLprediction, (int)l_COLLresults[3], (int)l_COLLresults[4]);


//Console.WriteLine($"Predicted Value: { l_DBLprediction }");
//Console.WriteLine($"High: {l_COLLoutputProcessed["Predictive High"]}");
//Console.WriteLine($"Low: {l_COLLoutputProcessed["Predictive Low"]}");
//Console.WriteLine($"Last: {l_COLLtrainClassAmounts.Last().ToString()}");
//Console.WriteLine($"Actual: {((decimal)l_COLLtestClassifications.First() / 100) * (decimal)l_COLLtrainClassAmounts.Last() + (decimal)l_COLLtrainClassAmounts.Last()}");
//Console.WriteLine($"Date: {l_COLLtestDates.First().ToString()}");

//Console.WriteLine($"{l_OBJarborist.Predict(new List<object> { 4, true, true, 500, 40000 }).ToString()}");

//DecisionTree l_OBJtestTree = l_OBJarborist.PlantTree(0, 200);

//Console.WriteLine();
//Console.WriteLine(l_OBJtestTree.RootNode.Predict(new object[] { 63.0, 130.0 }));
//Console.WriteLine();
//Console.WriteLine(l_OBJtestTree.RootNode.Predict(new object[] { 67.0, 155.0 }));

//Console.WriteLine();
//Console.WriteLine(l_OBJtestTree.RootNode.Predict(new object[] { "Blue" }));
//Console.WriteLine();
//Console.WriteLine(l_OBJtestTree.RootNode.Predict(new object[] { "Red" }));

Console.WriteLine("Done");
Console.ReadLine();