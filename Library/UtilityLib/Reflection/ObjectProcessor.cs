using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityLib.Extensions;

namespace UtilityLib.Reflection
{
     //<summary>
     //特性处理器
     //为特定的对象抽取特定的处理器，并处理当前对象
     //</summary>
    public class ObjectProcessor<T> where T: System.Attribute
    {
        public Type AttributeType{get;private set;}

        public delegate void AttrHandler(T attr);

        public delegate void PropAttrHandler(T attr, object obj, System.Reflection.PropertyInfo propInfo);

        public event AttrHandler ProcessAttr;

        public event PropAttrHandler ProcessProp;

        public ObjectProcessor()
        {
            this.AttributeType = typeof(T);
        }

        public void Process(object obj)
        {

            // 先处理类型本身的特性
            var objAttrs = UtilityLib.Reflection.Attribute.GetAttributes<T>(obj);
            if (objAttrs != null)
            {
                foreach (var attr in objAttrs)
                {
                    if (this.ProcessAttr != null)
                    {
                        this.ProcessAttr(attr);
                    }
                }
            }

            // 接着处理类型各个属性的特性
            var properties = Property.GetProperties(obj.GetType());
            foreach (var property in properties)
            {
                var attributes = UtilityLib.Reflection.Attribute.GetAttributes<T>(obj, property);
                if (attributes == null) // means this property no need to be index
                    continue;

                foreach (var attribute in attributes.OrderByPriority())
                {
                    if (this.ProcessProp != null)
                    {
                        this.ProcessProp(attribute, obj, property);
                    }
                }
            }
        }
    }
}
