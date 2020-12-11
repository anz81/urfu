using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ext.Utilities.Linq
{
    public static class Queryable
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, SortRule rule)
        {
            if (rule == null) return query;

            Type type = typeof(T);
            ParameterExpression pe = Expression.Parameter(type, "obj");

            System.Reflection.PropertyInfo propInfo = type.GetProperty(rule.Property);

            //var expr = Expression.Property(pe, propInfo);

            var expr = Expression.MakeMemberAccess(pe, propInfo);
            var orderByExpression = Expression.Lambda(expr, pe);

            MethodCallExpression orderByCallExpression = Expression.Call(typeof(System.Linq.Queryable), rule.Direction == SortDirection.Ascending ? "OrderBy" : "OrderByDescending",
                new Type[] { type, propInfo.PropertyType }, query.Expression, orderByExpression);

            return query.Provider.CreateQuery<T>(orderByCallExpression);
        }

        public static IQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> query, SortRule rule, Expression<Func<TSource, TKey>> keySelector)
        {
            if (rule == null)
                return query.OrderBy(keySelector);
            else
                return query.OrderBy(rule);
        }

        public static IQueryable<TSource> OrderByThenBy<TSource, TKey>(this IQueryable<TSource> query, SortRule rule, params Expression<Func<TSource, TKey>>[] keySelector)
        {
            if (rule == null && keySelector.Any())
            {
                var sorted = query.OrderBy(keySelector.First());
                foreach (var expression in keySelector.Skip(1))
                {
                    sorted = sorted.ThenBy(expression);
                }
                return sorted;
            }
            else
                return query.OrderBy(rule);
        }

        // Поддерживает только int и string
        public static IQueryable<T> Where<T>(this IQueryable<T> query, FilterRules rules)
        {
            if (rules == null || rules.Count() == 0) return query;

            Type type = typeof(T);
            ParameterExpression pe = Expression.Parameter(type, "obj");

            Expression expr = Expression.Constant(true);
            foreach (var rule in rules)
            {
                if (string.IsNullOrWhiteSpace(rule.Value)) continue;

                var property = Expression.PropertyOrField(pe, rule.Property);

                if (property.Type == typeof(string))
                {
                    Expression propertyToLower = Expression.Call(property, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                    if (rule.Verb == "Equals")
                    {
                        var equal = Expression.Equal(propertyToLower,
                            Expression.Convert(Expression.Constant(rule.Value.ToLower()), property.Type));
                        expr = Expression.AndAlso(expr, equal);
                    }
                    else
                    {                        
                      
                        var methodContains = Expression.Call(propertyToLower, rule.Verb ?? "Contains", null,
                            Expression.Constant(rule.Value.ToLower()));                      
                        expr = Expression.AndAlso(expr, methodContains);
                    }
                }
                else if (property.Type == typeof(bool))
                {
                    var equal = Expression.Equal(property, Expression.Convert(Expression.Constant(bool.Parse(rule.Value)), property.Type));
                    expr = Expression.AndAlso(expr, equal);
                }
                else if (property.Type == typeof(int) || property.Type == typeof(int?))
                {
                    int val = 0;
                    if (int.TryParse(rule.Value, out val))
                    {
                        var equal = Expression.Equal(property, Expression.Convert(Expression.Constant(val), property.Type));
                        expr = Expression.AndAlso(expr, equal);
                    }
                }
                else if (property.Type.IsEnum)
                {
                    var val = Enum.Parse(property.Type, rule.Value);
                    var equal = Expression.Equal(property, Expression.Convert(Expression.Constant(val), property.Type));
                    expr = Expression.AndAlso(expr, equal);
                }

            }

            var whereExpression = Expression.Lambda<Func<T, bool>>(expr, pe);
            return query.Where(whereExpression);
        }

        // Поддерживает int, string, List<int>, List<string>
        public static IQueryable<T> Where<T>(this IQueryable<T> query, ObjectableFilterRules rules)
        {
            if (rules == null || rules.Count() == 0) return query;

            Type type = typeof(T);
            ParameterExpression pe = Expression.Parameter(type, "obj");

            Expression expr = Expression.Constant(true);
            foreach (var rule in rules)
            {
                if (rule.Value == null) continue;

                var property = Expression.PropertyOrField(pe, rule.Property);

                if (property.Type == typeof(string))
                {

                    Expression propertyToLower = Expression.Call(property, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                    if (rule.Verb == "Equals")
                    {
                        var equal = Expression.Equal(propertyToLower,
                            Expression.Convert(Expression.Constant(rule.Value.ToString().ToLower()), property.Type));
                        expr = Expression.AndAlso(expr, equal);
                    }
                    else
                    {
                        var methodContains = Expression.Call(propertyToLower, rule.Verb ?? "Contains", null,
                            Expression.Constant(rule.Value.ToString().ToLower()));
                        expr = Expression.AndAlso(expr, methodContains);
                    }
                }
                else if (property.Type == typeof(bool) || property.Type == typeof(bool?))
                {
                    bool value;
                    if (bool.TryParse(rule.Value.ToString(), out value))
                    {
                        var equal = Expression.Equal(property, Expression.Convert(Expression.Constant(value), property.Type));
                        expr = Expression.AndAlso(expr, equal);
                    }
                }
                else if (property.Type == typeof(int) || property.Type == typeof(int?))
                {
                    if (rule.Value.GetType() == typeof(int) || rule.Value.GetType() == typeof(int?)
                        || rule.Value.GetType() == typeof(System.Int16) || rule.Value.GetType() == typeof(System.Int32)
                        || rule.Value.GetType() == typeof(System.Int64))
                    {
                        int value;
                        if (Int32.TryParse(rule.Value.ToString(), out value))
                        {
                            var equal = Expression.Equal(property, Expression.Convert(Expression.Constant(value), property.Type));
                            expr = Expression.AndAlso(expr, equal);
                        }
                    }
                }
                else if (property.Type == typeof(List<int>))
                {
                    if (rule.Value.GetType() == typeof(JArray))
                    {
                        Expression exprOr = Expression.Constant(false);
                        var values = (JArray)rule.Value;
                        if (values.Count() != 0)
                        {
                            foreach (var v in values)
                            {
                                int value;
                                if (int.TryParse(v.ToString(), out value))
                                {
                                    var methodContains = Expression.Call(property, rule.Verb ?? "Contains", null,
                                        Expression.Constant(value));
                                    exprOr = Expression.Or(exprOr, methodContains);
                                }
                            }
                            expr = Expression.AndAlso(expr, exprOr);
                        }
                    }
                    else
                    {
                        int value;
                        if (int.TryParse(rule.Value.ToString(), out value))
                        {
                            var methodContains = Expression.Call(property, rule.Verb ?? "Contains", null,
                                Expression.Constant(value));
                            expr = Expression.AndAlso(expr, methodContains);
                        }
                    }
                }
                else if (property.Type == typeof(List<string>))
                {
                    if (rule.Value.GetType() == typeof(JArray))
                    {
                        Expression exprOr = Expression.Constant(false);
                        var values = (JArray)rule.Value;
                        if (values.Count() != 0)
                        {
                            foreach (var v in values)
                            {
                                var value = v.ToString();
                                var methodContains = Expression.Call(property, rule.Verb ?? "Contains", null,
                                    Expression.Constant(value));
                                exprOr = Expression.Or(exprOr, methodContains);
                            }
                            expr = Expression.AndAlso(expr, exprOr);
                        }
                    }
                    else
                    {
                        var value = rule.Value.ToString();
                        var methodContains = Expression.Call(property, rule.Verb ?? "Contains", null,
                            Expression.Constant(value));
                        expr = Expression.AndAlso(expr, methodContains);
                    }
                }
            }

            var whereExpression = Expression.Lambda<Func<T, bool>>(expr, pe);
            return query.Where(whereExpression);
        }
    }
}
