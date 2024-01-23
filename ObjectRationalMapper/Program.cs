﻿using ObjectRationalMapper;
using ObjectRationalMapper.DatabaseQuery;
using ObjectRationalMapper.DataClass;

Facade facade = new();
facade.ConfigureMySql("localhost", "test", "root", "xxxx");

var insertBuilder = new InsertBuilder<TestClass>();
var entityToInsert = new TestClass
{
    Id = 1,
    Name = "Jakub Januszewski",
    Age = 28
};
//you can use insert without specifying attributes (fields that are null will be set to default value becuase of the way that e.g. doubles work - they default to 0)
var insert = insertBuilder.Insert().Values(entityToInsert).ToCommand();
facade.ExecuteInsert(insert);

entityToInsert = new TestClass
{
    Id = 10,
    Name = "Jan Burak",
    Age = 44
};
//or you can specify attributes that you want to insert - any other field will be null
insert = insertBuilder.Insert().Attributes(x => x.Id, x => x.Name, x => x.Age).Values(entityToInsert).ToCommand();

facade.ExecuteInsert(insert);

entityToInsert = new TestClass
{
    Id = 20,
    Name = "Jan AAAA",
    Age = 26
};
//you can also do this - it will insert only fields that are not null - other fields will be set to null
insert = insertBuilder.Insert().Attributes().Values(entityToInsert).ToCommand();

facade.ExecuteInsert(insert);

//how select works
var queryBuilder = new QueryBuilder<TestClass>();
var query = queryBuilder.Select(x => x.Name, x => x.Age).Where(x => x.Id >= 0).Limit(10).ToCommand();
var result = facade.ExecuteSelect(query);
Console.WriteLine(result);