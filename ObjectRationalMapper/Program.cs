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
facade.ConfigureMySql("localhost", "test", "konrad", "######");
// var queryBuilder = new QueryBuilder<TestClass>();
// var query = queryBuilder.Select(x => x.Name, x => x.Age).Where(x => x.Id == 1).And(x => x.Name == "Konrad").ToCommand();
// var result = facade.ExecuteSelect(query);
// facade.ConfigureMySql("localhost", "test", "root", "xxxx");
//
// var insertBuilder = new InsertBuilder<TestClass>();
// var entityToInsert = new TestClass
// {
//     Id = 1,
//     Name = "Jakub Januszewski",
//     Age = 28,
//     Height = 23.5
// };
// //you can use insert without specifying attributes (fields that are null will be set to default value becuase of the way that e.g. doubles work - they default to 0)
// var insert = insertBuilder.Insert().Values(entityToInsert).ToCommand();
// facade.ExecuteInsert(insert);
//
// entityToInsert = new TestClass
// {
//     Id = 10,
//     Name = "Jan Burak",
//     Age = 44,
//     Height = 1.80,
// };
// //or you can specify attributes that you want to insert - any other field will be null
// insert = insertBuilder.Insert().Attributes(x => x.Id, x => x.Name, x => x.Age).Values(entityToInsert).ToCommand();
//
// facade.ExecuteInsert(insert);
//
// entityToInsert = new TestClass
// {
//     Id = 20,
//     Name = "Jan AAAA",
//     Age = 26
// };
// //you can also do this - it will insert only fields that are not null - other fields will be set to null
// insert = insertBuilder.Insert().Attributes().Values(entityToInsert).ToCommand();
//
// facade.ExecuteInsert(insert);
//
// //how select works, to add addidtional conditions use .And or .Or
//  var queryBuilder2 = new QueryBuilder<TestClass>();
//  var query2 = queryBuilder2.Select(x => x.Name, x => x.Age, x => x.Height).Where(x => x.Id >= 0).Limit(10).ToCommand();
//  var result2 = facade.ExecuteSelect(query);
//  Console.WriteLine(result);
//  
// //how delete works, to add addidtional conditions use .And or .Or
// Console.WriteLine("Deleting all entries where age is less than 40");
// var deleteBuilder = new DeleteBuilder<TestClass>();
// var delete = deleteBuilder.Delete().Where(x => x.Age < 40).ToCommand();
// facade.ExecuteDelete(delete);
//
// Console.WriteLine("After delete");
// query = queryBuilder.Select(x => x.Name, x => x.Age, x => x.Height).Where(x => x.Id >= 0).Limit(10).ToCommand();
// result = facade.ExecuteSelect(query);
// Console.WriteLine(result);
//
// //how update works, to add addidtional conditions use .And or .Or
// Console.WriteLine("Updating all entries where age is more than 40 to have age 99");
// var updateBuilder = new UpdateBuilder<TestClass>();
// var update = updateBuilder.Update().Set(x => x.Age == 99).Where(x => x.Age > 40).ToCommand();
// facade.ExecuteUpdate(update);
//
// Console.WriteLine("After update");
// query = queryBuilder.Select(x => x.Name, x => x.Age, x => x.Height).Where(x => x.Id >= 0).Limit(10).ToCommand();
// result = facade.ExecuteSelect(query);
// Console.WriteLine(result);

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
var trainInsertBuilder = new InsertBuilder<Train>();

foreach (var train in trainData!) {
    var trainInsertQuery = trainInsertBuilder.Insert().Values(train).ToCommand();
    facade.ExecuteInsert(trainInsertQuery);
}

var carData = JsonConvert.DeserializeObject<Car[]>(File.ReadAllText(@"..\..\..\Data\cars.json"));
var carInsertBuilder = new InsertBuilder<Car>();

foreach (var car in carData!) {
    var carInsertQuery = carInsertBuilder.Insert().Values(car).ToCommand();
    facade.ExecuteInsert(carInsertQuery);
}

var planeData = JsonConvert.DeserializeObject<Plane[]>(File.ReadAllText(@"..\..\..\Data\planes.json"));
var planeInsertBuilder = new InsertBuilder<Plane>();

foreach (var plane in planeData!) {
    var planeInsertQuery = planeInsertBuilder.Insert().Values(plane).ToCommand();
    facade.ExecuteInsert(planeInsertQuery);
}

var busData = JsonConvert.DeserializeObject<Bus[]>(File.ReadAllText(@"..\..\..\Data\buses.json"));
var busInsertBuilder = new InsertBuilder<Bus>();

foreach (var bus in busData!) {
    var busInsertQuery = busInsertBuilder.Insert().Values(bus).ToCommand();
    facade.ExecuteInsert(busInsertQuery);
}

var vehicleSelect = new QueryBuilder<Car>().Select().Where(x => x.CarEngineType == EngineType.Diesel).ToCommand();
var vehicleRes = facade.ExtractObjects<Car>(vehicleSelect);
Console.WriteLine("------------------------------------------");
foreach (var vehicle in vehicleRes)
{
    Console.WriteLine($"{vehicle.Name}: {vehicle.CarEngineType}");
}