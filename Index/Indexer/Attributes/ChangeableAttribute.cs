using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Attributes;

namespace Indexer.Attributes
{
    /// <summary>
    /// 所有可能会改变原有字段类型或名称的特性需继承此类
    /// </summary>
    public abstract class ChangeableAttribute: BaseAttribute
    {
        /// <summary>
        /// 获取或还原被改变了的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract void GetChange(object changedObj, System.Reflection.PropertyInfo changedProp, object value);
    }
}
