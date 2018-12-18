// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CoreCsvParser.Reflection
{
    public static class ReflectionUtils
    {
        public static PropertyInfo GetProperty<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            var member = GetMemberExpression(expression).Member;
            if (member is PropertyInfo property)
            {
                return property;
            }
            throw new InvalidOperationException($"Member with Name '{member?.Name}' is not a property.");
        }

        private static MemberExpression GetMemberExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            MemberExpression? memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access", nameof(expression));
            }

            return memberExpression;
        }

        public static Action<TEntity, TProperty> CreateSetter<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            var propertyInfo = GetProperty(property);

            var instance = Expression.Parameter(typeof(TEntity), "instance");
            var parameter = Expression.Parameter(typeof(TProperty), "param");

            return Expression.Lambda<Action<TEntity, TProperty>>(
                Expression.Call(instance, propertyInfo.GetSetMethod(), parameter),
                instance, parameter
            ).Compile();
        }

        public static string GetPropertyNameFromExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            var member = GetMemberExpression(expression).Member;
            return member.Name;
        }
    }
}
