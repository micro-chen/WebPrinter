using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace SmartClient.Common.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 返回枚举项的描述信息。
        /// </summary>
        /// <param name="value">要获取描述信息的枚举项。</param>
        /// <returns>枚举想的描述信息。</returns>
        public static string GetDescription(this Enum value)
        {
            Type enumType = value.GetType();
            // 获取枚举常数名称。
            string name = Enum.GetName(enumType, value);
            if (name != null)
            {
                // 获取枚举字段。
                FieldInfo fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    // 获取描述的属性。
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(fieldInfo,
                        typeof(DescriptionAttribute), false) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 把枚举转换为键值对集合
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="getText">获得值得文本</param>
        /// <returns>以枚举值为key，枚举文本为value的键值对集合</returns>
        public static Dictionary<Int32, String> EnumToDictionary(this Enum emValue)
        {
            Type enumType = emValue.GetType();
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "emValue");
            }
            Dictionary<Int32, String> enumDic = new Dictionary<int, string>();
            Array enumValues = Enum.GetValues(enumType);
            foreach (Enum enumValueItem in enumValues)
            {
                Int32 key = Convert.ToInt32(enumValueItem);
                String value = enumValueItem.GetDescription();
                enumDic.Add(key, value);
            }
            return enumDic;
        }
    }

}