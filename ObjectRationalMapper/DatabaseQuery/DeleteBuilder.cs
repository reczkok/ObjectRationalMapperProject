﻿using System.Linq.Expressions;
using System.Reflection;
using ObjectRationalMapper.Attributes;

namespace ObjectRationalMapper.DatabaseQuery;

public class DeleteBuilder<T> : IDeleteBuilder<T>
{
    private string _query = string.Empty;
    private string _fallbackQuery = string.Empty;
    
    public IDeleteBuilder<T> Delete()
    {
        var tableName = CustomClassMapper<T>.GetHierarchyTableName();
        var query = $"DELETE FROM {tableName}";
        FallbackWhere();
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
        _fallbackQuery = _fallbackQuery.Replace("WHERE", "AND");
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

    private void FallbackWhere()
    {
        var tableName = CustomClassMapper<T>.GetHierarchyTableName();
        var discriminatorValue = CustomClassMapper<T>.GetDiscriminatorValue();
        var discriminator = CustomClassMapper<T>.GetDiscriminator();
        var query = $" WHERE {discriminator} = '{discriminatorValue}'";
        _fallbackQuery = query;
    }

    public string ToCommand()
    {
        return _query + _fallbackQuery;
    }
}