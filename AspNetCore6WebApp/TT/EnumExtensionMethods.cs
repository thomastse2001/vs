using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT
{
    public static class EnumExtensionMethods
    {
        /// https://www.c-sharpcorner.com/code/323/how-do-i-get-description-of-enum.aspx
        /// https://stackoverflow.com/questions/2650080/how-to-get-c-sharp-enum-description-from-value
        /// https://blog.hildenco.com/2018/07/getting-enum-descriptions-using-c.html
        public static string GetEnumDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null) return string.Empty;
            var descriptionAttributes = (System.ComponentModel.DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }
    }
}
