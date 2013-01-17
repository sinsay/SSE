using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Indexer;
using Common.Attributes;
using Common;

namespace Indexer.Attributes
{
    /// <summary>
    /// attribute to change field's index name
    /// </summary>
    [UtilityLib.Reflection.Priority(2)]
    public class ChangeNameAttribute : ChangeableAttribute
    {
        public string NewName { get; set; }
        public ChangeNameAttribute(string newName)
        {
            if (string.IsNullOrEmpty(newName))
                throw new ArgumentNullException("newName");
            this.NewName = newName;
        }

        public override void Execute(IContext context)
        {
            var indexContext = context as IndexContext;
            indexContext.CurrentFieldInfo.FieldName = this.NewName;
        }

        public override void GetChange(object changedObj, System.Reflection.PropertyInfo changedProp, object value)
        {
            //UtilityLib.Reflection.Property.SetValue(changedObj, changedProp, this.NewName); ??? error???
            UtilityLib.Reflection.Property.SetValue(changedObj, changedProp, value);
        }
    }
}
