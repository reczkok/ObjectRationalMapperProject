using System.Linq.Expressions;
using MySql.Data.MySqlClient;

namespace ObjectRationalMapper.DatabaseQuery;

public interface IInsertBuilder<T>
{
    // insert into values toCommand
    public IInsertBuilder<T> Insert();
    public IInsertBuilder<T> Values(T entity);
    public string ToCommand();
    public void CreateIfNotExists();
}