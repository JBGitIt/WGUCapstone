// See https://aka.ms/new-console-template for more information
using RandomForest;
using Microsoft.Data.SqlClient;


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
                 l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T1_BUSLOANSBillions"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T1_CPIAUCSL"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T1_DPRIMEPercent"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T1_DPSACBW027SBOGBillions"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T1_EXPGSBillions"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T1_IMPGSBillions"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T1_RHEACBW027SBOGBillions"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T1_TLRESCONBillions"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T1_UNRATEPercent"))
                ,l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T1_GDPBillions"))
            });

            l_COLLclassifications.Add(l_OBJsqlReader.GetDecimal(l_OBJsqlReader.GetOrdinal("T2_GDPBillions")));
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

IEnumerable<object> l_COLLtestData = l_COLLdata.Skip(l_COLLdata.Count / 2);
IEnumerable<object> l_COLLtestClassifications = l_COLLclassifications.Skip(l_COLLdata.Count / 2);


Arborist l_OBJarborist = new Arborist(l_COLLtrainData, l_COLLtrainClassifications, 1000, 100, 2);

Console.WriteLine("Planting Forest...");
l_OBJarborist.PlantForestTimeSeries(5, 5);
//l_OBJarborist.PlantForestCategorization(4);

IEnumerable<object> l_COLLpredictionTest = (IEnumerable<object>)l_COLLtestData.First();
Console.WriteLine("\nMaking Prediction...");
Console.WriteLine(l_OBJarborist.Predict(l_COLLpredictionTest));
Console.WriteLine(l_COLLtrainClassifications.Last().ToString());
Console.WriteLine(l_COLLtestClassifications.First().ToString());

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


Console.ReadLine();