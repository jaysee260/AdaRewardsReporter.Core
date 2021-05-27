using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AdaRewardsReporter.Core.Models;
using ConsoleTables;
using CsvHelper.Configuration.Attributes;

namespace AdaRewardsReporter.Core.Utils
{
    public static class TableHelper
    {
        public static void PrintSummaryToConsole(IEnumerable<RewardsPerEpochSummary> rewardsSummary)
        {
            var adaRewardsToDate = rewardsSummary.Select(x => x.Amount).Sum();
            System.Console.WriteLine($"Total rewards to date: {adaRewardsToDate} ADA\n");
            TableHelper.BuildTableFrom(rewardsSummary).Write();
        }

        public static ConsoleTable BuildTableFrom<T>(IEnumerable<T> values)
        {
            var table = new ConsoleTable();
            // {
            //     ColumnTypes = GetColumnsType<T>().ToArray()
            // };

            var columns = GetColumns<T>();
            var propertyNames = GetPropertyNames<T>();
            table.AddColumn(columns);
            
            foreach (
                var propertyValues
                in values.Select(value => propertyNames.Select(property => GetColumnValue<T>(value, property)))
            ) table.AddRow(propertyValues.ToArray());

            return table;
        }

        private static IEnumerable<Type> GetColumnsType<T>()
        {
            return typeof(T).GetProperties().Select(x => x.PropertyType).ToArray();
        }

        private static IEnumerable<string> GetColumns<T>()
        {
            var columns = typeof(T).GetProperties().Select(GetDisplayName);
            return columns;
        }

        private static IEnumerable<string> GetPropertyNames<T>()
        {
            var propertyNames = typeof(T).GetProperties().Select(x => x.Name);
            return propertyNames;
        }

        private static string GetDisplayName(PropertyInfo prop)
        {
            var attrs = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>();
            var attrs2 = prop.GetCustomAttributes(typeof(NameAttribute), true).Cast<NameAttribute>();
            var name = attrs2.Any() ? attrs2.First().Names.First() : prop.Name;
            var displayName = attrs.Any() ? attrs.First().DisplayName : prop.Name;
            return displayName;
        }

        private static object GetColumnValue<T>(object target, string column)
        {
            return typeof(T).GetProperty(column).GetValue(target, null);
        }
    }
}