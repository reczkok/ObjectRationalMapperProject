using ObjectRationalMapper;
using ObjectRationalMapper.DatabaseQuery;
using ObjectRationalMapper.DataClass;

Facade facade = new();
facade.ConfigureMySql("localhost", "test", "root", "AAAA");

var insertBuilder = new InsertBuilder<TestClass>();
var entityToInsert = new TestClass
{
    Id = 1,
    Name = "Jakub Kowalski",
    Age = 28,
    Height = 168,
    Weight = 63
};
var insert = insertBuilder.Insert().Values(entityToInsert).ToCommand();
facade.ExecuteInsert(insert);

var queryBuilder = new QueryBuilder<TestClass>();
var query = queryBuilder.Select(x => x.Name, x => x.Age).Where(x => x.Id == 1).Limit(10).ToCommand();
var result = facade.ExecuteSelect(query);
Console.WriteLine(result);



