using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using ObjectRationalMapper;
using ObjectRationalMapper.Attributes;
using ObjectRationalMapper.DatabaseActions;
using ObjectRationalMapper.DatabaseQuery;
using ObjectRationalMapper.DataClass;

Facade facade = new();
facade.ConfigureMySql("localhost", "test", "konrad", "xxxx");

var entityToInsert = new TestClass
{
    Name = "Jakub Januszewski",
    Age = 28,
    Height = 23.5
};
var entityToInsert2 = new TestInheritance
{
    Name = "Jakub2 Januszewski",
    Age = 28,
    Height = 23.5,
    Weight = 100,
    TestInheritanceId = 400,
    TestInheritanceName = "I WORK OMG"
};

/*
var insertBuilder = new InsertBuilder<TestInheritance>();
var insert2 = insertBuilder.Insert().Values(entityToInsert2).ToCommand();
facade.ExecuteInsert(insert2);

var select = new QueryBuilder<TestInheritance>().Select().Where(x => x.Height >= 20.2).ToCommand();
var res = facade.ExtractObjects<TestInheritance>(select);
foreach (var testInheritance in res)
{
    Console.WriteLine(testInheritance.Name);
}*/

facade.ExecuteDropTable<Vehicle>();

var trainData = JsonConvert.DeserializeObject<Train[]>(File.ReadAllText(@"..\..\..\Data\trains.json"));
var trainInsertBuilder = facade.GetInsertBuilder<Train>();

foreach (var train in trainData!) {
    var trainInsertQuery = trainInsertBuilder.Insert().Values(train).ToCommand();
    facade.ExecuteInsert(trainInsertQuery);
}

var carData = JsonConvert.DeserializeObject<Car[]>(File.ReadAllText(@"..\..\..\Data\cars.json"));
var carInsertBuilder = facade.GetInsertBuilder<Car>();

foreach (var car in carData!) {
    var carInsertQuery = carInsertBuilder.Insert().Values(car).ToCommand();
    facade.ExecuteInsert(carInsertQuery);
}

var planeData = JsonConvert.DeserializeObject<Plane[]>(File.ReadAllText(@"..\..\..\Data\planes.json"));
var planeInsertBuilder = facade.GetInsertBuilder<Plane>();

foreach (var plane in planeData!) {
    var planeInsertQuery = planeInsertBuilder.Insert().Values(plane).ToCommand();
    facade.ExecuteInsert(planeInsertQuery);
}

var busData = JsonConvert.DeserializeObject<Bus[]>(File.ReadAllText(@"..\..\..\Data\buses.json"));
var busInsertBuilder = facade.GetInsertBuilder<Bus>();

foreach (var bus in busData!) {
    var busInsertQuery = busInsertBuilder.Insert().Values(bus).ToCommand();
    facade.ExecuteInsert(busInsertQuery);
}

var vehicleSelect = facade.GetSelectBuilder<Car>().Select().Where(x => x.CarEngineType == EngineType.Diesel).ToCommand();
var vehicleRes = facade.ExtractObjects<Car>(vehicleSelect);
Console.WriteLine("------------------------------------------");
foreach (var vehicle in vehicleRes)
{
    Console.WriteLine($"{vehicle.Name}: {vehicle.CarEngineType}");
}