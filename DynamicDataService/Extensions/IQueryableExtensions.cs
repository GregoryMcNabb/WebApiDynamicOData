using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataService.Extensions
{
    internal static class IQueryableExtension
    {
        public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, string selector, params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            LambdaExpression lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(typeof(TSource), typeof(TResult), selector, values);
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    typeof(Queryable), "Select",
                    new Type[] { typeof(TSource), typeof(TResult) },
                    source.Expression, Expression.Quote(lambda)));
        }
        public static IQueryable<TResult> SelectMany<TSource, TResult>(this IQueryable<TSource> source, string selector, params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            LambdaExpression lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(typeof(TSource), typeof(IEnumerable<TResult>), selector, values);
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    typeof(Queryable), "SelectMany",
                    new Type[] { typeof(TSource), typeof(TResult) },
                    source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable<T> DynamicWhere<T>(this IQueryable<T> query, PropertyInfo[] keys, string keyParameters)
        {
            if (string.IsNullOrWhiteSpace(keyParameters))
                return query;
            return DynamicFilter(query, keys, keyParameters);
        }

        private static IQueryable<T> DynamicFilter<T>(IQueryable<T> query, PropertyInfo[] keys, string keyParameters)
        {
            if (!keyParameters.Contains('='))
                return query.Where($"{keys[0].Name}.ToString() == @0", new[] { keyParameters });

            var keyValues = keyParameters.Split(',').Select(k => k.Split('=')).ToDictionary(prop => prop[0], val => val[1]);
            var parameters = keyValues.Select(kv => kv.Value).ToArray();
            int i = 0;
            var expression = string.Join(" AND ", keyValues.Select(kv => $"{kv.Key}.ToString() == @{i++}"));

            return query.Where(expression, parameters);
        }

        public static IQueryable<T> DynamicWhere<T>(this IQueryable<T> query, PropertyInfo[] keys, string[] keyParameters)
        {
            if (keyParameters.Count() <= 0)
                return query;

            string expression = "";
            for (int i = 0; i < keys.Count(); i++)
            {
                if (i > 0)
                    expression += " AND ";
                expression += $"{keys[i].Name}.ToString() == @{i}";
            }
            return query.Where(expression, keyParameters);
        }

        public static async Task<T> DynamicFindAsync<T>(this IQueryable<T> query, PropertyInfo[] keys, string keyParameters)
        {
            var keyParams = keyParameters.Split(',');
            string expression = "";
            for (int i = 0; i < keys.Count(); i++)
            {
                if (i > 0)
                    expression += " AND ";
                expression += $"{keys[i].Name}.ToString() == @{i}";
            }
            return await query.Where(expression, keyParams).SingleOrDefaultAsync();
        }

        public static T DynamicFind<T>(this IQueryable<T> query, PropertyInfo[] keys, string keyParameters)
        {
            var keyParams = keyParameters.Split(',');
            string expression = "";
            for (int i = 0; i < keys.Count(); i++)
            {
                if (i > 0)
                    expression += " AND ";
                expression += $"{keys[i].Name}.ToString() == @{i}";
            }
            return query.Where(expression, keyParams).SingleOrDefault();
        }
    }
}
