using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using MySql.Data.MySqlClient;
using ObjectRationalMapper.Attributes;
using ObjectRationalMapper.DatabaseConnection;

namespace ObjectRationalMapper.DatabaseQuery
{
    public class InsertBuilder<T> : IInsertBuilder<T>
    {
        private string _query = string.Empty;
        private List<string> _selectedAttributes;

        public IInsertBuilder<T> Insert()
        {
            var type = typeof(T);
            var tableName = GetTableName(type);
            CreateIfNotExists();

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

            _selectedAttributes = new List<string>();

            foreach (var attribute in attributes)
            {
                _selectedAttributes.Add(CustomClassMapper<T>.GetPropertyName(attribute));
            }

            return this;
        }

        public IInsertBuilder<T> Values(T entity)
        {
            if (string.IsNullOrEmpty(_query))
            {
                throw new InvalidOperationException("Insert must be called before Values");
            }

            IEnumerable<string> propertyNames;
            IEnumerable<string> propertyValues;

            if (_selectedAttributes != null && _selectedAttributes.Any())
            {
                propertyNames = _selectedAttributes;
                propertyValues = _selectedAttributes.Select(propName => GetPropertyStringValue(entity, propName));
            }
            else
            {
                propertyNames = typeof(T).GetProperties()
                    .Where(prop => prop.GetCustomAttribute<FieldAttribute>() != null)
                    .Select(prop => prop.Name);

                propertyValues = propertyNames.Select(propName => GetPropertyStringValue(entity, propName));
            }

            var values = string.Join(", ", propertyValues.Select(value => $"'{value}'"));
            var columns = string.Join(", ", propertyNames);

            var query = $"{_query} ({columns}) VALUES ({values})";
            _query = query;
            return this;
        }


        public string ToCommand()
        {
            return _query;
        }

        public void CreateIfNotExists()
        {
            var connection = Session.GetInstance().GetConnection();
            var type = typeof(T);
            var tableName = GetTableName(type);

            if (!TableExists(connection, tableName))
            {
                var createTableQuery = GenerateCreateTableQuery<T>();
                using (MySqlCommand cmd = new MySqlCommand(createTableQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static bool TableExists(MySqlConnection connection, string tableName)
        {
            using (MySqlCommand cmd = new MySqlCommand($"SHOW TABLES LIKE '{tableName}'", connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    return reader.HasRows;
                }
            }
        }

        private string GenerateCreateTableQuery<TClass>()
        {
            var tableName = GetTableName(typeof(TClass));
            var columns = typeof(TClass).GetProperties()
                .Where(prop => prop.GetCustomAttribute<FieldAttribute>() != null)
                .Select(prop =>
                {
                    var fieldName = prop.GetCustomAttribute<FieldAttribute>().Name ?? prop.Name;
                    var fieldType = prop.GetCustomAttribute<FieldAttribute>().Type;
                    var sqlType = MapToSqlType(fieldType ?? prop.PropertyType);
                    return $"{fieldName} {sqlType}";
                });

            return $"CREATE TABLE {tableName} ({string.Join(", ", columns)});";
        }

        private string MapToSqlType(Type propertyType)
        {
            return propertyType switch
            {
                Type t when t == typeof(int) => "INT",
                Type t when t == typeof(int) => "INT",
                Type t when t == typeof(string) => "VARCHAR(255)",
                Type t when t == typeof(double) => "DECIMAL(10, 2)",
                Type t when t == typeof(DateTime) => "DATETIME",
                Type t when t == typeof(bool) => "BOOLEAN",
                Type t when t == typeof(byte) => "TINYINT",
                Type t when t == typeof(char) => "CHAR",
                _ => throw new NotSupportedException($"Type {propertyType.Name} is not supported"),
            };
        }

        private string GetTableName(Type type)
        {
            return type.GetCustomAttribute<TablenameAttribute>()?.Name ?? type.Name;
        }

        private string GetPropertyStringValue(T entity, string propertyName)
        {
            propertyName = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
            var propertyValue = typeof(T).GetProperty(propertyName)?.GetValue(entity);
            if (propertyValue is double doubleValue)
            {
                return doubleValue.ToString(CultureInfo.InvariantCulture);
            }
            return propertyValue?.ToString() ?? string.Empty;
        }
    }
}