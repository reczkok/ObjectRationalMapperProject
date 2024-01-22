using System.Linq.Expressions;
using System.Reflection;

namespace ObjectRationalMapper.Attributes;

public static class CustomClassMapper<T>
{
    public static string GetTableName()
    {
        var type = typeof(T);
        return type.GetCustomAttribute<TablenameAttribute>()?.Name ?? type.Name;
    }

    public static FieldAttribute?[] GetFieldNames()
    {
        var type = typeof(T);
        return type.GetProperties().Select(property => property.GetCustomAttribute<FieldAttribute>()).ToArray();
    }

    public static string GetPropertyName(Expression<Func<T, object>> expression)
    {
        switch (expression.Body)
        {
            case MemberExpression memberExpression:
                return memberExpression.Member.GetCustomAttribute<FieldAttribute>()?.Name ?? memberExpression.Member.Name;
            case UnaryExpression unaryExpression:
                return ((MemberExpression)unaryExpression.Operand).Member.GetCustomAttribute<FieldAttribute>()?.Name ?? ((MemberExpression)unaryExpression.Operand).Member.Name;
            default:
                throw new NotSupportedException($"Expression type {expression.NodeType} not supported");
        }
    }
    
    public static string GetBinaryOperator(ExpressionType expressionType, BinaryExpression expression)
    {
        if(expression.Left is MemberExpression memberExpression)
        {
            var fieldAttribute = memberExpression.Member.GetCustomAttribute<FieldAttribute>();
            if (fieldAttribute?.Type == typeof(string))
            {
                return GetStringBinaryOperator(expressionType);
            }
        }
        
        return expressionType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "!=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            _ => throw new ArgumentException("Invalid expression")
        };
    }

    private static string GetStringBinaryOperator(ExpressionType expressionType)
    {
        return expressionType switch
        {
            ExpressionType.Equal => "LIKE",
            ExpressionType.NotEqual => "NOT LIKE",
            _ => throw new ArgumentException("Invalid expression")
        };
    }

    public static string Visit(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression binaryExpression:
            {
                var left = Visit(binaryExpression.Left);
                var right = Visit(binaryExpression.Right);
                return $"{left} {GetBinaryOperator(binaryExpression.NodeType, binaryExpression)} {right}";
            }
            case MemberExpression memberExpression:
                var fieldAttribute = memberExpression.Member.GetCustomAttribute<FieldAttribute>();
                return fieldAttribute?.Name ?? memberExpression.Member.Name;
            case ConstantExpression constantExpression:
                if (constantExpression.Value is string)
                {
                    return $"'{constantExpression.Value}'";
                }
                return constantExpression.Value?.ToString() ?? string.Empty;
            case UnaryExpression unaryExpression:
                return Visit(unaryExpression.Operand);
            default:
                throw new NotSupportedException($"Expression type {expression.NodeType} not supported");
        }
    }
}