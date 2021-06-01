using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using System.Reflection;

namespace UnityEssentials.Spline.Extensions.Editor
{
    /// <summary>
    /// A convenience base class for making inspector editors.
    /// </summary>
    /// <typeparam name="T">The class we're editing</typeparam>
    public class BaseEditor<T> : UnityEditor.Editor where T : class
    {
        protected T _target { get { return target as T; } }
        protected T _targetIndex(int index) { return (targets[index] as T); }

        protected SerializedProperty FindAndExcludeProperty<TValue>(Expression<Func<T, TValue>> expr)
        {
            SerializedProperty p = FindProperty(expr);
            ExcludeProperty(p.name);
            return p;
        }

        protected SerializedProperty FindProperty<TValue>(Expression<Func<T, TValue>> expr)
        {
            return serializedObject.FindProperty(FieldPath(expr));
        }

        protected string FieldPath<TValue>(Expression<Func<T, TValue>> expr)
        {
            return ExtReflection.GetFieldPath(expr);
        }

        protected virtual List<string> GetExcludedPropertiesInInspector()
        {
            var excluded = new List<string>() { "m_Script" };
            if (mAdditionalExcluded != null)
                excluded.AddRange(mAdditionalExcluded);
            return excluded;
        }

        List<string> mAdditionalExcluded;
        protected void ExcludeProperty(string propertyName)
        {
            if (mAdditionalExcluded == null)
                mAdditionalExcluded = new List<string>();
            mAdditionalExcluded.Add(propertyName);
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            DrawRemainingPropertiesInInspector();
        }

        protected virtual void BeginInspector()
        {
            mAdditionalExcluded = null;
            serializedObject.Update();
        }

        protected virtual void DrawPropertyInInspector(SerializedProperty p)
        {
            List<string> excluded = GetExcludedPropertiesInInspector();
            if (!excluded.Contains(p.name))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(p);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                ExcludeProperty(p.name);
            }
        }

        protected virtual void DrawRemainingPropertiesInInspector()
        {
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, GetExcludedPropertiesInInspector().ToArray());
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        #region only this class
        protected virtual void DrawOnlyThisKindOfClass<V>() where V : Component
        {
            DrawPropertiesExcluding(serializedObject, GetVariablesOfThisClass<V>());
        }

        private string[] GetVariablesOfThisClass<V>()
        {
            List<string> variables = new List<string>();
            //BindingFlags bindingFlagsLocal = BindingFlags.DeclaredOnly | // This flag excludes inherited variables.
            //                            BindingFlags.Public |
            //                            BindingFlags.NonPublic |
            //                            BindingFlags.Instance |
            //                            BindingFlags.Static;
            BindingFlags bindingFlagsTotal = BindingFlags.Public |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Instance |
                                        BindingFlags.Static;
            Type typeGlobal = _target.GetType();
            Type thisOne = typeof(V);

            //FieldInfo[] localFields = type.GetFields(bindingFlagsLocal);
            FieldInfo[] totalOfTarget = typeGlobal.GetFields(bindingFlagsTotal);
            FieldInfo[] totalOfCurrentClass = thisOne.GetFields(bindingFlagsTotal);


            //for (int i = 0; i < totalOfTarget.Length; i++)
            //{
            //    FieldInfo field = totalOfTarget[i];
            //    if (!ArrayContain(totalOfCurrentClass, field))
            //    {
            //        variables.Add(field.Name);
            //    }
            //}


            //keep everything!
            for (int i = 0; i < totalOfCurrentClass.Length; i++)
            {
                FieldInfo field = totalOfCurrentClass[i];
                variables.Add(field.Name);
            }
            
            return variables.ToArray();
        }

        #endregion

        #region onlyClassCurrent
        protected virtual void DrawOnlyClassProperties(bool appendExcludedProperty)
        {
            string[] notChildVariable = GetVariablesOfChildClass();
            string[] toExclude = GetExcludedPropertiesInInspector().ToArray();

            if (appendExcludedProperty)
            {
                DrawPropertiesExcluding(serializedObject, ArrayCombine(notChildVariable, toExclude));
            }
            else
            {
                DrawPropertiesExcluding(serializedObject, notChildVariable);
            }
        }

        private string[] GetVariablesOfChildClass()
        {
            List<string> variables = new List<string>();
            BindingFlags bindingFlagsLocal = BindingFlags.DeclaredOnly | // This flag excludes inherited variables.
                                        BindingFlags.Public |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Instance |
                                        BindingFlags.Static;
            BindingFlags bindingFlagsTotal = BindingFlags.Public |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Instance |
                                        BindingFlags.Static;
            Type type = _target.GetType();
            FieldInfo[] localFields = type.GetFields(bindingFlagsLocal);
            FieldInfo[] totalFields = type.GetFields(bindingFlagsTotal);

            for (int i = 0; i < totalFields.Length; i++)
            {
                FieldInfo field = totalFields[i];
                if (!ArrayContain(localFields, field))
                {
                    variables.Add(field.Name);
                }
            }
            return variables.ToArray();
        }

        private static bool ArrayContain<U>(U[] array, U item)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (EqualityComparer<U>.Default.Equals(array[i], item))
                {
                    return (true);
                }
            }
            return (false);
        }

        private static U[] ArrayCombine<U>(U[] array, U[] array2)
        {
            U[] total = new U[array.Length + array2.Length];
            int i = 0;
            for (i = 0; i < array.Length; i++)
            {
                total[i] = array[i];
            }
            for (int k = 0; k < array2.Length; k++)
            {
                total[i + k] = array2[k];
            }
            return (total);
        }
        #endregion

    }
}
