using System.Linq.Expressions;
using MySql.Data.MySqlClient;

namespace ObjectRationalMapper.DatabaseQuery;

public interface IInsertBuilder<T>
{
    // insert into values toCommand
    public IInsertBuilder<T> Insert();
    IInsertBuilder<T> Attributes(params Expression<Func<T, object>>[] attributes);
    public IInsertBuilder<T> Values(T entity);
    public string ToCommand();
    public void CreateIfNotExists();
}