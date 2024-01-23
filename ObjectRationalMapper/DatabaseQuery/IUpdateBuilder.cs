using System.Linq.Expressions;

namespace ObjectRationalMapper.DatabaseQuery;

public interface IUpdateBuilder<T>
{
    public IUpdateBuilder<T> Update();
    public IUpdateBuilder<T> Set(Expression<Func<T, object>> expression);
    public IUpdateBuilder<T> Where(Expression<Func<T, bool>> expression);
    public IUpdateBuilder<T> And(Expression<Func<T, bool>> expression);
    public IUpdateBuilder<T> Or(Expression<Func<T, bool>> expression);
    public string ToCommand();
}