using System.Linq.Expressions;
using System.Reflection;
using ObjectRationalMapper.Attributes;

namespace ObjectRationalMapper.DatabaseQuery;

public class UpdateBuilder<T> : IUpdateBuilder<T>
{
    private string _query = string.Empty;
    
    public IUpdateBuilder<T> Update()
    {
        var type = typeof(T);
        var tableName = type.GetCustomAttribute<TablenameAttribute>()?.Name ?? type.Name;
        var query = $"UPDATE {tableName}";
        _query = query;
        return this;
    }

    public IUpdateBuilder<T> Set(Expression<Func<T, object>> expression)
    {
        if (string.IsNullOrEmpty(_query))
        {
            throw new InvalidOperationException("Update must be called before Set");
        }
        var query = $"{_query} SET {CustomClassMapper<T>.Visit(expression.Body)}";
        _query = query;
        return this;
    }

    public IUpdateBuilder<T> Where(Expression<Func<T, bool>> expression)
    {
        if (string.IsNullOrEmpty(_query) || !_query.Contains("SET"))
        {
            throw new InvalidOperationException("Set must be called before Where");
        }
        var query = $"{_query} WHERE {CustomClassMapper<T>.Visit(expression.Body)}";
        _query = query;
        return this;
    }

    public IUpdateBuilder<T> And(Expression<Func<T, bool>> expression)
    {
        if (string.IsNullOrEmpty(_query) || !_query.Contains("WHERE"))
        {
            throw new InvalidOperationException("Where must be called before And");
        }
        var query = $"{_query} AND {CustomClassMapper<T>.Visit(expression.Body)}";
        _query = query;
        return this;
    }

    public IUpdateBuilder<T> Or(Expression<Func<T, bool>> expression)
    {
        if (string.IsNullOrEmpty(_query) || !_query.Contains("WHERE"))
        {
            throw new InvalidOperationException("Where must be called before Or");
        }
        var query = $"{_query} OR {CustomClassMapper<T>.Visit(expression.Body)}";
        _query = query;
        return this;
    }

    public string ToCommand()
    {
        return _query;
    }
}