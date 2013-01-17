using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Common.Attributes;
using Indexer;

namespace Indexer.Attributes
{
    /// <summary>
    /// attribute to process the field which type of date
    /// </summary>
    public class DateAttribute : ChangeableAttribute
    {
        [UtilityLib.Reflection.Priority(2)]
        public override void Execute(IContext context)
        {
            var indexContext = context as IndexContext;
            var fieldValue = DateTime.Parse(indexContext.CurrentFieldInfo.FieldValue.ToString());
            indexContext.CurrentFieldInfo.FieldValue = UtilityLib.Extensions.DateTimeExtension.GetCompareTimeExtractSec(fieldValue);
        }

        public override void GetChange(object changedObj, System.Reflection.PropertyInfo changedProp, object value)
        {
            UtilityLib.Reflection.Property.SetValue(changedObj,
                changedProp,
                UtilityLib.Extensions.DateTimeExtension.RestoreFromSec(long.Parse(value.ToString())));
        }
    }
}
