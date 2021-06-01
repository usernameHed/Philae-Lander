using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Text;
using System.IO;

namespace UnityEssentials.Spline.Extensions.Editor
{
    /// <summary>
    /// Extension class for SerializedProperties
    /// See also: http://answers.unity3d.com/questions/627090/convert-serializedproperty-to-custom-class.html
    /// 
    /// use:
    ///  // Get a serialized object
    /// var serializedObject = new UnityEditor.SerializedObject(target);
    ///
    /// Set the property debug to true
    /// serializedObject.FindProperty("debug").SetValue<bool>(true);
    ///
    /// Get the property value of debug
    /// bool debugValue = serializedObject.FindProperty("debug").GetValue<bool>();
    /// </summary>
    public static class ExtSerializedProperties
    {
        #region basic
        public static void UpdateEditor(this UnityEditor.Editor editor)
        {
            if (editor == null || editor.serializedObject == null)
            {
                return;
            }
            editor.serializedObject.Update();
        }

        public static void ApplyModification(this UnityEditor.Editor editor, bool withUndo = true)
        {
            if (editor == null || editor.serializedObject == null)
            {
                return;
            }
            if (withUndo)
            {
                editor.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                editor.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        public static SerializedProperty GetPropertie(this UnityEditor.Editor editor, string propertie)
        {
            return (editor.serializedObject.FindProperty(propertie));
        }

        //public static void SetNonSerializePropertie(this object current, string propertie, object value, BindingFlags binding = BindingFlags.NonPublic | BindingFlags.Instance)
        //{
        //    FieldInfo field = typeof(SplineControllerStick).GetField(propertie, binding);
        //    field?.SetValue(current, value);
        //}

        /// <summary>
        /// use this if you want to acces a private class B inside class A.
        /// this class B will be converted to SerializeObject,
        /// then you can access to property inside that class B
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static SerializedObject ToSerializeObject<T>(this SerializedProperty property)
            where T : UnityEngine.Object
        {
            return (new SerializedObject(property.GetValue<T>()));
        }

        public static SerializedProperty GetPropertie(this SerializedProperty editor, string propertie)
        {
            return (editor.FindPropertyRelative(propertie));
        }

        public static SerializedProperty GetPropertie(this SerializedObject serializedObject, string property)
        {
            return (serializedObject.FindProperty(property));
        }

        public static T GetValue<T>(this UnityEditor.Editor editor, string propertieName)
        {
            SerializedProperty property = editor.GetPropertie(propertieName);
            return (property.GetValue<T>());
        }

        public static bool SetValue<T>(this UnityEditor.Editor editor, string propertieName, T value)
        {
            SerializedProperty property = editor.GetPropertie(propertieName);
            return (property.SetValue(value));
        }

        /// <summary>
        /// Get the object the serialized property of a Component holds by using reflection
        /// </summary>
        /// <typeparam name="T">The object type that the property contains</typeparam>
        /// <param name="property"></param>
        /// <returns>Returns the object type T if it is the type the property actually contains</returns>
        public static T GetValue<T>(this SerializedProperty property)
        {
            if (property == null)
            {
                return (default(T));
            }
            UnityEngine.Object tar = property.serializedObject.targetObject;
            object obj = tar as Component;
            if (obj != null)
            {
                return (GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootComponent(property)));
            }
            obj = tar as ScriptableObject;
            if (obj != null)
            {
                return (GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootScriptableObject(property)));
            }
            return (default(T));
        }

        /// <summary>
        /// Get the object the serialized property of a Component holds by using reflection
        /// </summary>
        /// <typeparam name="T">The object type that the property contains</typeparam>
        /// <param name="property"></param>
        /// <returns>Returns the object type T if it is the type the property actually contains</returns>
        public static bool SetValue<T>(this SerializedProperty property, T value)
        {
            if (property == null)
            {
                return (false);
            }
            UnityEngine.Object tar = property.serializedObject.targetObject;
            object obj = tar as Component;
            if (obj != null)
            {
                bool setValue = SetValueOnComponent<T>(property, value);
                return (setValue);
            }
            obj = tar as ScriptableObject;
            if (obj != null)
            {
                return (SetValueOnScriptableObject<T>(property, value));
            }
            return (false);
        }

        /// <summary>
        /// Get the object the serialized property of a Component holds by using reflection
        /// </summary>
        /// <typeparam name="T">The object type that the property contains</typeparam>
        /// <param name="property"></param>
        /// <returns>Returns the object type T if it is the type the property actually contains</returns>
        public static T GetValueFromComponents<T>(this SerializedProperty property)
        {
            return (GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootComponent(property)));
        }

        /// <summary>
        /// Get the object the serialized property of a ScriptableObject holds by using reflection
        /// </summary>
        /// <typeparam name="T">The object type that the property contains</typeparam>
        /// <param name="property"></param>
        /// <returns>Returns the object type T if it is the type the property actually contains</returns>
        private static T GetValueFromScriptableObject<T>(this SerializedProperty property)
        {
            return GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootScriptableObject(property));
        }

        /// <summary>
        /// Set the value of a field of the property with the type T on a Component
        /// </summary>
        /// <typeparam name="T">The type of the field that is set</typeparam>
        /// <param name="property">The serialized property that should be set</param>
        /// <param name="value">The new value for the specified property</param>
        /// <returns>Returns if the operation was successful or failed</returns>
        private static bool SetValueOnComponent<T>(this SerializedProperty property, T value)
        {
            object obj = GetSerializedPropertyRootComponent(property);
            //Iterate to parent object of the value, necessary if it is a nested object
            string[] fieldStructure = property.propertyPath.Split('.');
            for (int i = 0; i < fieldStructure.Length - 1; i++)
            {
                obj = GetFieldOrPropertyValue<object>(fieldStructure[i], obj);
            }
            string fieldName = fieldStructure.Last();

            return SetFieldOrPropertyValue(fieldName, obj, value);

        }

        /// <summary>
        /// Set the value of a field of the property with the type T on a ScriptableObject
        /// </summary>
        /// <typeparam name="T">The type of the field that is set</typeparam>
        /// <param name="property">The serialized property that should be set</param>
        /// <param name="value">The new value for the specified property</param>
        /// <returns>Returns if the operation was successful or failed</returns>
        private static bool SetValueOnScriptableObject<T>(this SerializedProperty property, T value)
        {

            object obj = GetSerializedPropertyRootScriptableObject(property);
            //Iterate to parent object of the value, necessary if it is a nested object
            string[] fieldStructure = property.propertyPath.Split('.');
            for (int i = 0; i < fieldStructure.Length - 1; i++)
            {
                obj = GetFieldOrPropertyValue<object>(fieldStructure[i], obj);
            }
            string fieldName = fieldStructure.Last();

            return SetFieldOrPropertyValue(fieldName, obj, value);

        }

        /// <summary>
        /// Get the component of a serialized property
        /// </summary>
        /// <param name="property">The property that is part of the component</param>
        /// <returns>The root component of the property</returns>
        public static Component GetSerializedPropertyRootComponent(SerializedProperty property)
        {
            return (Component)property.serializedObject.targetObject;
        }

        /// <summary>
        /// Get the scriptable object of a serialized property
        /// </summary>
        /// <param name="property">The property that is part of the scriptable object</param>
        /// <returns>The root scriptable object of the property</returns>
        public static ScriptableObject GetSerializedPropertyRootScriptableObject(SerializedProperty property)
        {
            return (ScriptableObject)property.serializedObject.targetObject;
        }

        /// <summary>
        /// Iterates through objects to handle objects that are nested in the root object
        /// </summary>
        /// <typeparam name="T">The type of the nested object</typeparam>
        /// <param name="path">Path to the object through other properties e.g. PlayerInformation.Health</param>
        /// <param name="obj">The root object from which this path leads to the property</param>
        /// <param name="includeAllBases">Include base classes and interfaces as well</param>
        /// <returns>Returns the nested object casted to the type T</returns>
        public static T GetNestedObject<T>(string path, object obj, bool includeAllBases = false)
        {
            foreach (string part in path.Split('.'))
            {
                obj = GetFieldOrPropertyValue<object>(part, obj, includeAllBases);
            }
            return (T)obj;
        }

        public static T GetFieldOrPropertyValue<T>(string fieldName, object obj, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null) return (T)field.GetValue(obj);

            PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
            if (property != null) return (T)property.GetValue(obj, null);

            if (includeAllBases)
            {

                foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if (field != null) return (T)field.GetValue(obj);

                    property = type.GetProperty(fieldName, bindings);
                    if (property != null) return (T)property.GetValue(obj, null);
                }
            }

            return default(T);
        }

        public static bool SetFieldOrPropertyValue(string fieldName, object obj, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                field.SetValue(obj, value);
                return true;
            }

            PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
            if (property != null)
            {
                property.SetValue(obj, value, null);
                return true;
            }

            if (includeAllBases)
            {
                foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if (field != null)
                    {
                        field.SetValue(obj, value);
                        return true;
                    }

                    property = type.GetProperty(fieldName, bindings);
                    if (property != null)
                    {
                        property.SetValue(obj, value, null);
                        return true;
                    }
                }
            }
            return false;
        }

        public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type, bool includeSelf = false)
        {
            List<Type> allTypes = new List<Type>();

            if (includeSelf) allTypes.Add(type);

            if (type.BaseType == typeof(object))
            {
                allTypes.AddRange(type.GetInterfaces());
            }
            else
            {
                allTypes.AddRange(
                        Enumerable
                        .Repeat(type.BaseType, 1)
                        .Concat(type.GetInterfaces())
                        .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                        .Distinct());
            }

            return allTypes;
        }

        public static List<int> GetListInt(SerializedProperty listProp)
        {
            List<int> newList = new List<int>(listProp.arraySize);
            for (int i = 0; i < listProp.arraySize; i++)
            {
                newList.Add(listProp.GetArrayElementAtIndex(i).intValue);
            }
            return (newList);
        }

        public static void SetListInt(SerializedProperty listprop, List<int> datas)
        {
            listprop.arraySize = datas.Count;
            for (int i = 0; i < listprop.arraySize; i++)
            {
                listprop.GetArrayElementAtIndex(i).intValue = datas[i];
            }
        }

        public static void SetListComponents<T>(SerializedProperty listprop, List<T> datas) where T : Component
        {
            listprop.arraySize = datas.Count;
            for (int i = 0; i < listprop.arraySize; i++)
            {
                listprop.GetArrayElementAtIndex(i).objectReferenceValue = datas[i];
            }
        }



        /// <summary>
        /// from a given SerializePorperty Component reference, try to fill it if null
        /// usage:
        /// ExtSerializedProperties.SetObjectReferenceValueIfEmpty<MeshExploder>(this.GetPropertie("_meshExploder"), _unit.transform);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference">SerializePorperty Component reference</param>
        /// <param name="whereToSearch">Transform where to search in children</param>
        public static bool SetObjectReferenceValueIfEmpty<T>(SerializedProperty reference, Transform whereToSearch, bool searchOnChilds = true) where T : Component
        {
            if (reference.objectReferenceValue == null)
            {
                T component = (searchOnChilds) ? whereToSearch.GetExtComponentInChildrens<T>(depth: 99, startWithOurSelf: true) : whereToSearch.GetComponent<T>();
                if (component != null)
                {
                    Debug.Log("set " + component.name);
                    reference.objectReferenceValue = component;
                    return (true);
                }
            }
            return (false);
        }

        public static void SetObjectReferenceValueIfEmptyFromNames<T>(SerializedProperty reference, Transform whereToSearch, params string[] nameOrPartialName) where T : Component
        {
            if (reference.objectReferenceValue == null)
            {
                Transform[] allChilds = whereToSearch.GetExtComponentsInChildrens<Transform>(depth: 99, startWithOurSelf: true);
                for (int i = 0; i < allChilds.Length; i++)
                {
                    for (int k = 0; i < nameOrPartialName.Length; k++)
                    {
                        if (allChilds[i].name.Contains(nameOrPartialName[k]))
                        {
                            T component = allChilds[i].GetComponent<T>();
                            if (component != null)
                            {
                                Debug.Log("set " + component.name);
                                reference.objectReferenceValue = component;
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static SerializedProperty GetParent(this SerializedProperty property)
        {
            string path = property.propertyPath;
            if (path.EndsWith("]"))
            {
                for (int i = 0; i < path.Length; i++)
                {
                    if (path[path.Length - 1] != 'A')
                    {
                        path.Pop(path.Length - 1, out path);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            string[] pathSplit = path.Split('.');
            System.Array.Resize(ref pathSplit, pathSplit.Length - 1);
            string parentPath = pathSplit.Concat(".");
            return property.serializedObject.FindProperty(parentPath);
        }
        #endregion

        #region helper array
        public static void ApplyArrayToSerializeArray<T>(SerializedProperty array, T[] components) where T : UnityEngine.Object
        {
            array.arraySize = components.Length;
            for (int i = 0; i < array.arraySize; i++)
            {
                array.GetArrayElementAtIndex(i).objectReferenceValue = components[i];
            }
        }

        public static void RemoveAt(this SerializedProperty array, int index)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            if (index < 0 || index >= array.arraySize)
                throw new IndexOutOfRangeException();

            array.GetArrayElementAtIndex(index).SetPropertyValue(null);
            array.DeleteArrayElementAtIndex(index);
        }

        public static void RemoveObject(this SerializedProperty array, UnityEngine.Object toRemove)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            for (int i = 0; i < array.arraySize; i++)
            {
                if (array.GetArrayElementAtIndex(i).objectReferenceValue == toRemove)
                {
                    array.GetArrayElementAtIndex(i).SetPropertyValue(null);
                    array.DeleteArrayElementAtIndex(i);
                    return;
                }
            }
        }
        public static void RemoveAllObject(this SerializedProperty array, UnityEngine.Object toRemove)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            for (int i = array.arraySize - 1; i >= 0; i--)
            {
                if (array.GetArrayElementAtIndex(i).objectReferenceValue == toRemove)
                {
                    array.GetArrayElementAtIndex(i).SetPropertyValue(null);
                    array.DeleteArrayElementAtIndex(i);
                }
            }
        }

        public static void AddObject(this SerializedProperty array, UnityEngine.Object toAdd)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            array.arraySize++;
            array.GetArrayElementAtIndex(array.arraySize - 1).objectReferenceValue = toAdd;
        }

        public static void AddString(this SerializedProperty array, string toAdd)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            array.arraySize++;
            array.GetArrayElementAtIndex(array.arraySize - 1).stringValue = toAdd;
        }

        public static bool AddObjectIfNoContain(this SerializedProperty array, UnityEngine.Object toAdd)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            for (int i = 0; i < array.arraySize; i++)
            {
                if (array.GetArrayElementAtIndex(i).objectReferenceValue == toAdd)
                {
                    return (false);
                }
            }
            array.arraySize++;
            array.GetArrayElementAtIndex(array.arraySize - 1).objectReferenceValue = toAdd;
            return (true);
        }

        public static bool ContainsObject(this SerializedProperty array, UnityEngine.Object item)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            for (int i = 0; i < array.arraySize; i++)
            {
                if (array.GetArrayElementAtIndex(i).objectReferenceValue == item)
                {
                    return (true);
                }
            }
            return (false);
        }

        public static bool ContainsString(this SerializedProperty array, string item)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            for (int i = 0; i < array.arraySize; i++)
            {
                if (array.GetArrayElementAtIndex(i).stringValue == item)
                {
                    return (true);
                }
            }
            return (false);
        }

        public static void InsertAtIndex(this SerializedProperty array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            if (!array.isArray)
            {
                throw new ArgumentException("Property is not an array");
            }

            if (index < 0 || (index >= array.arraySize && array.arraySize > 0))
            {
                throw new IndexOutOfRangeException();
            }

            array.InsertArrayElementAtIndex(index);
            array.serializedObject.ApplyModifiedProperties();
        }

        public static int Dupplicate(this SerializedProperty array, int indexToDuplicate, out bool firstItem)
        {
            firstItem = false;
            if (indexToDuplicate < 0 || indexToDuplicate >= array.arraySize)
            {
                firstItem = true;
                array.arraySize++;
                return (0);
            }
            SerializedProperty currentCopy = array.GetArrayElementAtIndex(indexToDuplicate);
            currentCopy.DuplicateCommand();
            return (indexToDuplicate + 1);
        }

        /// <summary>
        /// return true if the 2 list have the same content
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool AreListObjectHaveSameContent(SerializedProperty first, SerializedProperty second)
        {
            if (first.arraySize != second.arraySize)
            {
                return (false);
            }
            for (int i = 0; i < first.arraySize; i++)
            {
                if (!Equals(first.GetArrayElementAtIndex(i).objectReferenceValue, second.GetArrayElementAtIndex(i).objectReferenceValue))
                {
                    return (false);
                }
            }
            return (true);
        }
        public static bool AreListObjectHaveSameContent(SerializedProperty first, List<UnityEngine.Object> second)
        {
            if (first.arraySize != second.Count)
            {
                return (false);
            }
            for (int i = 0; i < first.arraySize; i++)
            {
                if (!Equals(first.GetArrayElementAtIndex(i).objectReferenceValue, second[i]))
                {
                    return (false);
                }
            }
            return (true);
        }

        public static List<UnityEngine.Object> ToObjectList(this SerializedProperty list)
        {
            List<UnityEngine.Object> listObject = new List<UnityEngine.Object>(list.arraySize);
            for (int i = 0; i < list.arraySize; i++)
            {
                listObject.Add(list.GetArrayElementAtIndex(i).objectReferenceValue);
            }
            return (listObject);
        }

        public static bool IsThereNullInList(this SerializedProperty list)
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                if (IsTruelyNull(list.GetArrayElementAtIndex(i).objectReferenceValue))
                {
                    return (true);
                }
            }
            return (false);
        }

        public static bool IsTruelyNull(this object aRef)
        {
            return aRef != null && aRef.Equals(null);
        }

        public static void SetPropertyValue(this SerializedProperty property, object value)
        {
            switch (property.propertyType)
            {

                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = value as AnimationCurve;
                    break;

                case SerializedPropertyType.ArraySize:
                    property.intValue = Convert.ToInt32(value);
                    break;

                case SerializedPropertyType.Boolean:
                    property.boolValue = Convert.ToBoolean(value);
                    break;

                case SerializedPropertyType.Bounds:
                    property.boundsValue = (value == null)
                            ? new Bounds()
                            : (Bounds)value;
                    break;

                case SerializedPropertyType.Character:
                    property.intValue = Convert.ToInt32(value);
                    break;

                case SerializedPropertyType.Color:
                    property.colorValue = (value == null)
                            ? new Color()
                            : (Color)value;
                    break;

                case SerializedPropertyType.Float:
                    property.floatValue = Convert.ToSingle(value);
                    break;

                case SerializedPropertyType.Integer:
                    property.intValue = Convert.ToInt32(value);
                    break;

                case SerializedPropertyType.LayerMask:
                    property.intValue = (value is LayerMask) ? ((LayerMask)value).value : Convert.ToInt32(value);
                    break;

                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = value as UnityEngine.Object;
                    break;

                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = (value == null)
                            ? Quaternion.identity
                            : (Quaternion)value;
                    break;

                case SerializedPropertyType.Rect:
                    property.rectValue = (value == null)
                            ? new Rect()
                            : (Rect)value;
                    break;

                case SerializedPropertyType.String:
                    property.stringValue = value as string;
                    break;

                case SerializedPropertyType.Vector2:
                    property.vector2Value = (value == null)
                            ? Vector2.zero
                            : (Vector2)value;
                    break;

                case SerializedPropertyType.Vector3:
                    property.vector3Value = (value == null)
                            ? Vector3.zero
                            : (Vector3)value;
                    break;

                case SerializedPropertyType.Vector4:
                    property.vector4Value = (value == null)
                            ? Vector4.zero
                            : (Vector4)value;
                    break;

            }
        }
        #endregion

        #region newFuncions

        /// <summary>
        /// from a given SerializePorperty Component reference, try to fill it if null
        /// usage:
        /// ExtSerializedProperties.SetObjectReferenceValueIfEmpty<MeshExploder>(this.GetPropertie("_meshExploder"), _unit.transform);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference">SerializePorperty Component reference</param>
        /// <param name="whereToSearch">Transform where to search in children</param>
        public static T SetObjectReferenceValueIfEmpty<T>(SerializedProperty reference, Transform whereToSearch) where T : Component
        {
            if (reference.objectReferenceValue == null)
            {
                T component = whereToSearch.GetExtComponentInChildrens<T>(depth: 99, startWithOurSelf: true);
                if (component != null)
                {
                    reference.objectReferenceValue = component;
                    return (component);
                }
                component = whereToSearch.GetComponentInParent<T>();
                if (component != null)
                {
                    reference.objectReferenceValue = component;
                    return (component);
                }
            }
            return (null);
        }

        public static void SetObjectReferenceArrayIfEmpty<T>(SerializedProperty reference, Transform whereToSearch) where T : Component
        {
            T[] component = whereToSearch.GetExtComponentsInChildrens<T>(depth: 99, startWithOurSelf: true);
            reference.arraySize = component.Length;


            for (int i = 0; i < reference.arraySize; i++)
            {
                if (reference.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    if (component[i] != null)
                    {
                        Debug.Log("Set " + component[i].name);
                        reference.GetArrayElementAtIndex(i).objectReferenceValue = component[i];
                    }
                }
            }
        }

        /// <summary>
        /// for a given SerializedProperty component, search on every childs if one with that component contain a special name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <param name="whereToSearch"></param>
        /// <param name="nameOrPartialName"></param>
        public static void SetObjectReferenceValueIfEmptyFromNamesInChilds<T>(SerializedProperty reference, Transform whereToSearch, params string[] nameOrPartialName) where T : Component
        {
            if (reference.objectReferenceValue != null)
            {
                return;
            }

            Transform[] allChilds = whereToSearch.GetExtComponentsInChildrens<Transform>(depth: 99, startWithOurSelf: true);
            for (int i = 0; i < allChilds.Length; i++)
            {
                for (int k = 0; k < nameOrPartialName.Length; k++)
                {
                    if (allChilds[i].name.Contains(nameOrPartialName[k]))
                    {
                        T component = allChilds[i].GetComponent<T>();
                        if (component != null)
                        {
                            Debug.Log("Set " + component.name);
                            reference.objectReferenceValue = component;
                            return;
                        }
                    }
                }
            }
        }

        public static void SetObjectReferenceValueIfEmptyFromPrefabName<T>(SerializedProperty reference) where T : Component
        {
            if (reference.objectReferenceValue != null)
            {
                return;
            }
            T component = GetAssetByGenericType<T>("Assets/", "*.prefab");
            if (component)
            {
                reference.objectReferenceValue = component;
                Debug.Log("Set " + component.name);
            }
        }

        /// <summary>
        /// find the first asset of type T
        /// </summary>
        /// <typeparam name="T">Type to find</typeparam>
        /// <param name="directory">directory where to search the asset</param>
        /// <returns>first asset found with type T</returns>
        public static T GetAssetByGenericType<T>(string directory = "Assets/", string searchExtension = "*.asset") where T : UnityEngine.Object
        {
            List<T> assets = GetAssetsByGenericType<T>(directory, searchExtension);
            if (assets.Count == 0)
            {
                return (null);
            }
            return (assets[0]);
        }

        /// <summary>
        /// find all asset of type T
        /// </summary>
        /// <typeparam name="T">Type to find</typeparam>
        /// <returns>all asset found with type T, return an list<Object>, you have to cast them to use them, see GetAssetsByRaceScriptableObjects for an exemple</returns>
        public static List<T> GetAssetsByGenericType<T>(string directory = "Assets/", string searchExtension = "*.asset") where T : UnityEngine.Object
        {
            List<T> AllAssetFound = new List<T>();

            DirectoryInfo dir = new DirectoryInfo(directory);
            FileInfo[] infoAssets = dir.GetFiles(searchExtension, SearchOption.AllDirectories);

            System.Type typeToSearch = typeof(T);

            for (int i = 0; i < infoAssets.Length; i++)
            {
                string relativePath = ConvertAbsoluteToRelativePath(infoAssets[i].FullName);

                System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(relativePath);
                if (assetType == null)
                {
                    continue;
                }
                T asset = (T)AssetDatabase.LoadAssetAtPath(relativePath, typeToSearch);
                if (asset)
                {
                    AllAssetFound.Add(asset);
                }
            }
            return (AllAssetFound);
        }


        #endregion

        #region getChilds
        /// <summary>
        /// Get a component of type T on any of the children recursivly
        /// use:
        /// Player player = gameObject.GetExtComponentInChildrens<Player>();
        /// Player player = gameObject.GetExtComponentInChildrens<Player>(4, true);
        /// Player[] player = gameObject.GetExtComponentInChildrens<Player>();
        /// Player[] player = gameObject.GetExtComponentInChildrens<Player>(4, true);
        /// </summary>
        public static T GetExtComponentInChildrens<T>(this Component component, int depth = 99, bool startWithOurSelf = false)
            where T : Component
        {
            return (GetExtComponentInChildrens<T>(component.gameObject, depth, startWithOurSelf));
        }
        public static T GetExtComponentInChildrens<T>(this GameObject gameObject, int depth = 99, bool startWithOurSelf = false)
            where T : Component
        {
            if (startWithOurSelf)
            {
                T result = gameObject.GetComponent<T>();
                if (result != null)
                    return (result);
            }

            foreach (Transform t in gameObject.transform)
            {
                if (depth - 1 <= 0)
                    return (null);
                return (t.gameObject.GetExtComponentInChildrens<T>(depth - 1, true));
            }
            return (null);
        }
        public static T[] GetExtComponentsInChildrens<T>(this Component component, int depth = 99, bool startWithOurSelf = false)
            where T : Component
        {
            return (GetExtComponentsInChildrens<T>(component.gameObject, depth, startWithOurSelf));
        }
        public static T[] GetExtComponentsInChildrens<T>(this GameObject gameObject, int depth = 99, bool startWithOurSelf = false)
            where T : Component
        {
            List<T> results = new List<T>();
            if (startWithOurSelf)
            {
                T[] result = gameObject.GetComponents<T>();
                for (int i = 0; i < result.Length; i++)
                {
                    results.Add(result[i]);
                }
            }

            foreach (Transform t in gameObject.transform)
            {
                if (depth - 1 <= 0)
                    break;
                results.AddRange(t.gameObject.GetExtComponentsInChildrens<T>(depth - 1, true));
            }

            return results.ToArray();
        }

        #endregion

        #region string
        public static char Pop(this string s, int index, out string remaining)
        {
            char c = s[0];
            remaining = s.Remove(index, 1);

            return c;
        }

        public static string Concat(this IList<string> stringArray, string separator)
        {
            return stringArray.Concat(separator, 0, stringArray.Count);
        }

        public static string Concat(this IList<string> stringArray, string separator, int startIndex, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = startIndex; i < Mathf.Min(startIndex + count, stringArray.Count); i++)
            {
                stringBuilder.Append(stringArray[i]);
                if (i < stringArray.Count - 1)
                {
                    stringBuilder.Append(separator);
                }
            }
            return stringBuilder.ToString();
        }
        #endregion

        #region path
        /// <summary>
        /// from an absolutePath, return the relative Path
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public static string ConvertAbsoluteToRelativePath(string absolutePath)
        {
            string gameDataFolder = Application.dataPath;
            absolutePath = ReformatPathForUnity(absolutePath);

            if (absolutePath.StartsWith(gameDataFolder))
            {
                string relativepath = "Assets" + absolutePath.Substring(Application.dataPath.Length);
                return (relativepath);
            }
            return (absolutePath);
        }

        /// <summary>
        /// change a path from
        /// Assets\path\of\file
        /// to
        /// Assets/path/of/file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ReformatPathForUnity(string path, char characterReplacer = '-')
        {
            string formattedPath = path.Replace('\\', '/');
            formattedPath = formattedPath.Replace('|', characterReplacer);
            return (formattedPath);
        }
        #endregion

        //end of class
    }
    //end of nameSpace
}
