using System.Linq.Expressions;
using System.Reflection;
using ObjectRationalMapper.Attributes;

namespace ObjectRationalMapper.DatabaseQuery;

public class QueryBuilder<T> : IQueryBuilder<T>
{
    private string _query = string.Empty;
    
    public IQueryBuilder<T> Select(params Expression<Func<T, object>>[] fields)
    {
        var type = typeof(T);
        var tableName = type.GetCustomAttribute<TablenameAttribute>()?.Name ?? type.Name;
        var query = $"SELECT {string.Join(", ", fields.Select(CustomClassMapper<T>.GetPropertyName))} FROM {tableName}";
        _query = query;
        return this;
    }

    public IQueryBuilder<T> Where(Expression<Func<T, bool>> condition)
    {
        if (string.IsNullOrEmpty(_query))
        {
            throw new InvalidOperationException("Select must be called before Where");
        }
        var query = $"{_query} WHERE {CustomClassMapper<T>.Visit(condition.Body)}";
        _query = query;
        return this;
    }

    public IQueryBuilder<T> Limit(int limit)
    {
        if (string.IsNullOrEmpty(_query))
        {
            throw new InvalidOperationException("Select must be called before Limit");
        }
        var query = $"{_query} LIMIT {limit}";
        _query = query;
        return this;
    }

    public IQueryBuilder<T> And(Expression<Func<T, bool>> expression)
    {
        if (string.IsNullOrEmpty(_query))
        {
            throw new InvalidOperationException("Select must be called before And");
        }
        var query = $"{_query} AND {CustomClassMapper<T>.Visit(expression.Body)}";
        _query = query;
        return this;
    }

    public IQueryBuilder<T> Or(Expression<Func<T, bool>> expression)
    {
        if (string.IsNullOrEmpty(_query))
        {
            throw new InvalidOperationException("Select must be called before Or");
        }
        var query = $"{_query} OR {CustomClassMapper<T>.Visit(expression.Body)}";
        _query = query;
        return this;
    }

    public IQueryBuilder<T> OrderBy(Expression<Func<T, object>>[] fields)
    {
        if (string.IsNullOrEmpty(_query))
        {
            throw new InvalidOperationException("Select must be called before OrderBy");
        }
        var query = $"{_query} ORDER BY {string.Join(", ", fields.Select(CustomClassMapper<T>.GetPropertyName))}";
        _query = query;
        return this;
    }

    public string ToCommand()
    {
        return _query;
    }
}