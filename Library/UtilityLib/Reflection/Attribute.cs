using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SysAttrubute = System.Attribute;

namespace UtilityLib.Reflection
{
    public static class Attribute
    {
        #region Static Field

        private static Dictionary<Type, Dictionary<String, SysAttrubute[]>> propAttributeCache;
        private static Dictionary<Type, SysAttrubute[]> typeAttributeCache;

        #endregion


        #region Constructor

        static Attribute()
        {
            typeAttributeCache = new Dictionary<Type, SysAttrubute[]>();
            propAttributeCache = new Dictionary<Type, Dictionary<String, SysAttrubute[]>>();
        }

        #endregion


        #region Public Method

        public static bool HasAttribute(object obj, Type attrType)
        {
            var attrs = GetAttributes(obj);
            return attrs.Any(a => a.GetType() == attrType);
        }

        public static bool HasAttribute(object obj, PropertyInfo prop, Type attrType)
        {
            var attrs = GetAttributes(obj, prop);
            return attrs.Any(a => a.GetType() == attrType);
        }

        public static SysAttrubute[] GetAttributes(object obj)
        {
            return GetAttributesPrivate(obj.GetType(), null);
        }

        public static T[] GetAttributes<T>(object obj) where T : SysAttrubute
        {
            return GetAttributes<T>(obj, null);
        }

        public static T[] GetAttributes<T>(object obj, PropertyInfo propInfo) where T: SysAttrubute
        {
            var attributes = GetAttributesPrivate(obj.GetType(), propInfo);
            List<T> tList = new List<T>();
            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    var tAttr = attribute as T;
                    if (tAttr != null)
                    {
                        tList.Add(tAttr);
                    }
                }
            }

            return tList.ToArray();
        }


        public static SysAttrubute[] GetAttributes(object obj, PropertyInfo propInfo)
        {
            return GetAttributesPrivate(obj.GetType(), propInfo);
        }

        /// <summary>
        /// get the attrType attribute instance whitch addon obj and construct with parameters
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attrType"></param>
        /// <param name="parameters">if the builid with the default constructor, parameters will be null</param>
        /// <returns></returns>
        public static T[] GetInstance<T>(object obj, PropertyInfo prop, object parameters) where T : SysAttrubute
        {
            var attrs = GetAttributesPrivate(obj.GetType(), prop) ?? new SysAttrubute[0];
            var targetAttrs = attrs.Where(a => a is T).ToArray();

            if (targetAttrs == null)
                return new T[0];

            T[] attributes = new T[targetAttrs.Length];
            var attrType = typeof(T);
            for (int i = 0; i < targetAttrs.Length; i++)
            {
                var attrInstance = (T)Reflection.CreateInstance(parameters, targetAttrs[i].GetType());

                var attrProps = Property.GetProperties(attrType);
                foreach (var attProp in attrProps)
                {
                    if (attProp.CanWrite && attProp.CanRead)
                    {
                        var value = Property.GetValue(targetAttrs[i], attProp);
                        if (value != null)
                        {
                            Property.SetValue(attrInstance, attProp, value);
                        }
                    }
                }
            }

            return attributes;
        }

        ///// <summary>
        ///// get the attrType attribute instance whitch addon obj and construct with parameters
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <param name="attrType"></param>
        ///// <param name="parameters">if the builid with the default constructor, parameters will be null</param>
        ///// <returns></returns>
        //public static T GetInstance<T>(object obj, PropertyInfo prop, Type attrType, object parameters) where T : SysAttrubute
        //{
        //    return (T)GetInstance(obj, prop, attrType, parameters);
        //}

        #endregion


        #region Private Method

        private static SysAttrubute[] GetAttributesPrivate(Type type, PropertyInfo prop)
        {
            SysAttrubute[] attrsResult = null;

            if (!propAttributeCache.ContainsKey(type))
            {
                foreach (var p in Property.GetProperties(type))
                {
                    var attrs = SysAttrubute.GetCustomAttributes(p);
                    if (!propAttributeCache.ContainsKey(type))
                    {
                        propAttributeCache[type] = new Dictionary<string, SysAttrubute[]>();
                    }
                    propAttributeCache[type][p.Name] = attrs;
                }
            }

            if (!typeAttributeCache.ContainsKey(type))
            {
                typeAttributeCache[type] = SysAttrubute.GetCustomAttributes(type);
            }

            if (prop != null)
            {
                attrsResult = propAttributeCache[type][prop.Name];
            }
            else
            {
                attrsResult = typeAttributeCache[type];
            }

            return attrsResult;
        }

        #endregion
    }
}
