﻿using System.Linq.Expressions;
using System.Reflection;
using ObjectRationalMapper.Attributes;

namespace ObjectRationalMapper.DatabaseQuery;

public class DeleteBuilder<T> : IDeleteBuilder<T>
{
    private string _query = string.Empty;
    
    public IDeleteBuilder<T> Delete()
    {
        var type = typeof(T);
        var tableName = type.GetCustomAttribute<TablenameAttribute>()?.Name ?? type.Name;
        var query = $"DELETE FROM {tableName}";
        _query = query;
        return this;
    }

    public IDeleteBuilder<T> Where(Expression<Func<T, bool>> expression)
    {
        if (string.IsNullOrEmpty(_query))
        {
            throw new InvalidOperationException("DeleteFrom must be called before Where");
        }
        var query = $"{_query} WHERE {CustomClassMapper<T>.Visit(expression.Body)}";
        _query = query;
        return this;
    }

    public IDeleteBuilder<T> And(Expression<Func<T, bool>> expression)
    {
        if (string.IsNullOrEmpty(_query) || !_query.Contains("WHERE"))
        {
            throw new InvalidOperationException("Where must be called before And");
        }
        var query = $"{_query} AND {CustomClassMapper<T>.Visit(expression.Body)}";
        _query = query;
        return this;
    }

    public IDeleteBuilder<T> Or(Expression<Func<T, bool>> expression)
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