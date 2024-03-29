﻿using System.Linq.Expressions;

namespace ObjectRationalMapper.DatabaseQuery;

public interface IDeleteBuilder<T>
{
    public IDeleteBuilder<T> Delete();
    public IDeleteBuilder<T> Where(Expression<Func<T, bool>> expression);
    public IDeleteBuilder<T> And(Expression<Func<T, bool>> expression);
    public IDeleteBuilder<T> Or(Expression<Func<T, bool>> expression);
    public string ToCommand();
}