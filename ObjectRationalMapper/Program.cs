
using ObjectRationalMapper;
using ObjectRationalMapper.DatabaseQuery;
using ObjectRationalMapper.DataClass;

Facade facade = new();
facade.ConfigureMySql("localhost", "test", "user", "XXXXXXXXX");
var queryBuilder = new QueryBuilder<TestClass>();
var query = queryBuilder.Select(x => x.Name, x => x.Age).Where(x => x.Id == 1).Limit(10).ToCommand();
var result = facade.ExecuteSelect(query);
Console.WriteLine(result);