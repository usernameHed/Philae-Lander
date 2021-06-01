using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace UnityEssentials.ActionTrigger.editor
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtReflection
    {
        /// <summary>Get the object owner of a field.  This method processes
        /// the '.' separator to get from the object that owns the compound field
        /// to the object that owns the leaf field</summary>
        /// <param name="path">The name of the field, which may contain '.' separators</param>
        /// <param name="obj">the owner of the compound field</param>
        public static object GetParentObject(string path, object obj)
        {
            var fields = path.Split('.');
            if (fields.Length == 1)
                return obj;

            var info = obj.GetType().GetField(
                    fields[0], System.Reflection.BindingFlags.Public 
                        | System.Reflection.BindingFlags.NonPublic 
                        | System.Reflection.BindingFlags.Instance);
            obj = info.GetValue(obj);

            return GetParentObject(string.Join(".", fields, 1, fields.Length - 1), obj);
        }

        /// <summary>Returns a string path from an expression - mostly used to retrieve serialized properties
        /// without hardcoding the field path. Safer, and allows for proper refactoring.</summary>
        public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    me = expr.Body as MemberExpression;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var members = new List<string>();
            while (me != null)
            {
                members.Add(me.Member.Name);
                me = me.Expression as MemberExpression;
            }

            var sb = new StringBuilder();
            for (int i = members.Count - 1; i >= 0; i--)
            {
                sb.Append(members[i]);
                if (i > 0) sb.Append('.');
            }
            return sb.ToString();
        }
    }
}
