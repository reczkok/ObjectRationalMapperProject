using System.Linq.Expressions;
using System.Reflection;
using ObjectRationalMapper.Attributes;

namespace ObjectRationalMapper.Attributes;

public static class CustomClassMapper<T>
{
    public static TablenameAttribute? GetTableAttribute()
    {
        var type = typeof(T);
        return type.GetCustomAttribute<TablenameAttribute>();
    }
    
    public static string GetHierarchyTableName(Type? t = null)
    {
        var type = typeof(T);
        if (t != null)
        {
            type = t;
        }
        var parent = type.GetCustomAttribute<TablenameAttribute>()!.ParentClass;
        return parent == null ? type.GetCustomAttribute<TablenameAttribute>()!.Name : GetHierarchyTableName(parent);
    }
    
    public static Type GetParentClass(Type type)
    {
        var parentClass = type.GetCustomAttribute<TablenameAttribute>()!.ParentClass;
        return parentClass == null ? type : GetParentClass(parentClass);
    }
    
    public static Type GetDirectParentClass(Type type)
    {
        var parentClass = type.GetCustomAttribute<TablenameAttribute>()!.ParentClass;
        return parentClass ?? type;
    }

    public static FieldAttribute?[] GetFieldAttributes()
    {
        var type = typeof(T);
        return GetFieldAttributes(type);
    }

    public static string GetDiscriminator()
    {
        return "ORMPROJ_Discriminator";
    }
    
    public static string GetDiscriminatorValue(Type? t = null)
    {
        var type = typeof(T);
        if (t != null)
        {
            type = t;
        }
        var discriminator = type.GetCustomAttribute<TablenameAttribute>()?.Name ?? type.Name;
        return discriminator;
    }
    
    public static FieldAttribute?[] GetFieldAttributes(Type type)
    {
        if(type.GetCustomAttribute<TablenameAttribute>()?.ParentClass == null)
        {
            return type.GetProperties().Select(property => property.GetCustomAttribute<FieldAttribute>()).ToArray();
        }
        var parentClass = GetDirectParentClass(type);
        var parentClassAttributes = parentClass.GetProperties().Select(property => property.GetCustomAttribute<FieldAttribute>()).ToArray();
        var thisClassAttributes = type.GetProperties().Select(property => property.GetCustomAttribute<FieldAttribute>()).ToArray();
        var excludedAttributes = parentClassAttributes.Select(attribute => attribute?.Name).ToArray();
        var thisClassAttributesWithoutParentClassAttributes = thisClassAttributes.Where(attribute => !excludedAttributes.Contains(attribute?.Name)).ToArray();
        return thisClassAttributesWithoutParentClassAttributes;
    }
    
    public static PropertyInfo[] GetProperties(Type t = null)
    {
        var type = typeof(T);
        if (t != null)
        {
            type = t;
        }
        return type.GetProperties();
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
    
    public static MemberInfo GetPropertyInfo(Expression<Func<T, object>> expression)
    {
        switch (expression.Body)
        {
            case MemberExpression memberExpression:
                return memberExpression.Member;
            case UnaryExpression unaryExpression:
                return ((MemberExpression)unaryExpression.Operand).Member;
            default:
                throw new NotSupportedException($"Expression type {expression.NodeType} not supported");
        }
    }
    
    public static string GetBinaryOperator(ExpressionType expressionType, BinaryExpression expression)
    {
        // if(expression.Left is MemberExpression memberExpression)
        // {
        //     var fieldAttribute = memberExpression.Member.GetCustomAttribute<FieldAttribute>();
        //     if (fieldAttribute?.Type == typeof(string))
        //     {
        //         return GetStringBinaryOperator(expressionType);
        //     }
        // }
        
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

                var properties = GetProperties(typeof(T));
                var leftProperty = properties.FirstOrDefault(property => property.GetCustomAttribute<FieldAttribute>()!.Name == left);
                if (leftProperty?.PropertyType.IsEnum == true)
                {
                    var enumValue = Enum.GetName(leftProperty.PropertyType, Convert.ToInt32(right));
                    right = $"'{enumValue}'";
                }
                
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
                if (constantExpression.Value is Enum)
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