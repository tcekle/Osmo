using Microsoft.EntityFrameworkCore;
using Osmo.Common.Database.Attributes;
using System.Reflection;

namespace Osmo.Common.Database.Extensions;

public static class TimeScaleExtensions
{
    /// <summary>
    /// Apply a hyper table to columns marked with a <see cref="HyperTableColumnAttribute"/>
    /// </summary>
    /// <param name="context"></param>
    public static void ApplyHypertables(this DbContext context)
    {
        // Adding timescale extension to the database
        context.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS timescaledb CASCADE;");
        var entityTypes = context.Model.GetEntityTypes();
        foreach (var entityType in entityTypes)
        {
            string hyperTableColumn = string.Empty;
            
            // Apply hypertables if not created
            foreach (var property in entityType.GetProperties())
            {
                if (property.PropertyInfo.GetCustomAttribute(typeof(HyperTableColumnAttribute)) != null)
                {
                    
                    var tableName = entityType.GetTableName();
                    hyperTableColumn = property.GetColumnName();
                    context.Database.ExecuteSqlRaw("SELECT create_hypertable({0}, {1}, if_not_exists => TRUE);", tableName, hyperTableColumn);
                    break;
                }
            }

            if (string.IsNullOrEmpty(hyperTableColumn))
            {
                continue;
            }
            
            // Apply indexes if not created
            foreach (var property in entityType.GetProperties())
            {
                if (property.PropertyInfo.GetCustomAttribute(typeof(IndexWithHyperTableColumnAttribute)) != null)
                {
                    var tableName = entityType.GetTableName();
                    var columnName = property.GetColumnName();
                    string indexName = $"idx_{columnName}_{hyperTableColumn}";
                    string sql = $"CREATE INDEX IF NOT EXISTS {indexName} ON \"{tableName}\" (\"{columnName}\", \"{hyperTableColumn}\" DESC)";
                    context.Database.ExecuteSqlRaw(sql);
                    break;
                }
            }
        }
    }
}