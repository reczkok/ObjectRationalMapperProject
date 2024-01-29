using System.Linq.Expressions;

namespace ObjectRationalMapper.DatabaseQuery;

public interface IQueryBuilder<T>
{
    public IQueryBuilder<T> Select(Expression<Func<T, object>>[] fields);
    public IQueryBuilder<T> Select();
    public IQueryBuilder<T> Where(Expression<Func<T, bool>> expression);
    public IQueryBuilder<T> Limit(int limit);
    public IQueryBuilder<T> And(Expression<Func<T, bool>> expression);
    public IQueryBuilder<T> Or(Expression<Func<T, bool>> expression);
    public IQueryBuilder<T> OrderBy(Expression<Func<T, object>>[] fields);
    public string ToCommand();
}