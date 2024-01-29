using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using MySql.Data.MySqlClient;
using ObjectRationalMapper.Attributes;
using ObjectRationalMapper.DatabaseActions;
using ObjectRationalMapper.DatabaseConnection;

namespace ObjectRationalMapper.DatabaseQuery
{
    public class InsertBuilder<T> : IInsertBuilder<T>
    {
        private string _query = string.Empty;
        private List<MemberInfo> _selectedAttributes;

        public IInsertBuilder<T> Insert()
        {
            var tableName = CustomClassMapper<T>.GetHierarchyTableName();
            TableGenerator<T>.GenerateIfNotExists();

            var query = $"INSERT INTO {tableName}";
            _query = query;
            return this;
        }

        public IInsertBuilder<T> Attributes(params Expression<Func<T, object>>[] attributes)
        {
            if (string.IsNullOrEmpty(_query))
            {
                throw new InvalidOperationException("Insert must be called before Values");
            }

            if (attributes == null || attributes.Length == 0)
            {
                return this;
            }

            _selectedAttributes = new List<MemberInfo>();

            foreach (var attribute in attributes)
            {
                _selectedAttributes.Add(CustomClassMapper<T>.GetPropertyInfo(attribute));
            }

            return this;
        }

        public IInsertBuilder<T> Values(T entity)
        {
            if (string.IsNullOrEmpty(_query))
            {
                throw new InvalidOperationException("Insert must be called before Values");
            }

            IEnumerable<MemberInfo> properties;
            IEnumerable<string> propertyValues;

            if (_selectedAttributes != null && _selectedAttributes.Any())
            {
                properties = _selectedAttributes;
                propertyValues = _selectedAttributes.Select(prop => GetPropertyStringValue(entity, prop));
            }
            else
            {
                properties = CustomClassMapper<T>.GetProperties();

                propertyValues = properties.Select(prop => GetPropertyStringValue(entity, prop));
            }
            
            var propertyNames = properties.Select(prop => prop.GetCustomAttribute<FieldAttribute>()?.Name ?? prop.Name);

            var discriminatorValue = CustomClassMapper<T>.GetDiscriminatorValue();
            var discriminator = CustomClassMapper<T>.GetDiscriminator();
            
            
            var values = string.Join(", ", propertyValues.Select(value => $"'{value}'"));
            var columns = string.Join(", ", propertyNames);
            
            columns += $", {discriminator}";
            values += $", '{discriminatorValue}'";
            
            var query = $"{_query} ({columns}) VALUES ({values})";
            _query = query;
            return this;
        }


        public string ToCommand()
        {
            return _query;
        }
        

        private string GetPropertyStringValue(T entity, MemberInfo prop)
        {
            var propertyValue = typeof(T).GetProperty(prop.Name)?.GetValue(entity);
            if (propertyValue is double doubleValue)
            {
                return doubleValue.ToString(CultureInfo.InvariantCulture);
            }
            if (propertyValue is DateTime dateTimeValue)
            {
                return dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return propertyValue?.ToString() ?? string.Empty;
        }
    }
}