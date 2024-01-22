using System.Linq.Expressions;
using System.Reflection;

namespace ObjectRationalMapper.Attributes;

public static class CustomClassMapper<T>
{
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
    
    public static string GetBinaryOperator(ExpressionType expressionType)
    {
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

    public static string Visit(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression binaryExpression:
            {
                var left = Visit(binaryExpression.Left);
                var right = Visit(binaryExpression.Right);
                return $"{left} {GetBinaryOperator(binaryExpression.NodeType)} {right}";
            }
            case MemberExpression memberExpression:
                return memberExpression.Member.Name;
            case ConstantExpression constantExpression:
                return constantExpression.Value?.ToString() ?? string.Empty;
            case UnaryExpression unaryExpression:
                return Visit(unaryExpression.Operand);
            default:
                throw new NotSupportedException($"Expression type {expression.NodeType} not supported");
        }
    }
}