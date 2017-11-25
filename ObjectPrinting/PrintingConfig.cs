using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private readonly HashSet<Type> excludedTypes = new HashSet<Type>();
        private readonly HashSet<string> excludedProperties = new HashSet<string>();

        private readonly Dictionary<Type, Delegate> typeSerializations = new Dictionary<Type, Delegate>();
        private readonly Dictionary<string, Delegate> propertySerializations = new Dictionary<string, Delegate>();
        private readonly Dictionary<Type, CultureInfo> cultures = new Dictionary<Type, CultureInfo>();
        private readonly Dictionary<string, int> propertiesLength = new Dictionary<string, int>();

        private readonly Type[] finalTypes =
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan)
        };

        public string PrintToString(TOwner obj) => PrintToString(obj, 0);

        public PrintingConfig<TOwner> ExcludeType<TPropType>()
        {
            excludedTypes.Add(typeof(TPropType));
            return this;
        }

        public PrintingConfig<TOwner> ExcludeProperty<TPropType>(Expression<Func<TOwner, TPropType>> selector)
        {
            var property = ((MemberExpression)selector.Body).Member;
            excludedProperties.Add(property.Name);
            return this;
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> selector)
        {
            var property = ((MemberExpression)selector.Body).Member;
            return new PropertyPrintingConfig<TOwner, TPropType>(this, property.Name);
        }

        public void AddSerialization(Type type, Delegate serializeFunc) =>
            typeSerializations[type] = serializeFunc;

        public void AddSerialization(string propertyName, Delegate serializeFunc) =>
            propertySerializations[propertyName] = serializeFunc;

        public void AddCulture(Type type, CultureInfo culture) =>
            cultures[type] = culture;

        public void AddPropertyLength(string propertyName, int length) =>
            propertiesLength[propertyName] = length;

        private string PrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null" + Environment.NewLine;
            if (finalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (excludedTypes.Contains(propertyInfo.PropertyType) ||
                    excludedProperties.Contains(propertyInfo.Name)) continue;

                sb.Append(identation + propertyInfo.Name + " = " +
                          Serialize(obj, propertyInfo, nestingLevel));
            }
            return sb.ToString();
        }

        private string Serialize(object obj, PropertyInfo propertyInfo, int nestingLevel)
        {
            var value = propertyInfo.GetValue(obj);

            if (propertiesLength.ContainsKey(propertyInfo.Name))
                return ((string)value).Substring(0, propertiesLength[propertyInfo.Name]) + Environment.NewLine;

            if (cultures.ContainsKey(propertyInfo.PropertyType))
                return ((IFormattable)value).ToString(null, cultures[propertyInfo.PropertyType]) + Environment.NewLine;

            if (typeSerializations.ContainsKey(propertyInfo.PropertyType))
                return typeSerializations[propertyInfo.PropertyType].DynamicInvoke(value) + Environment.NewLine;

            if (propertySerializations.ContainsKey(propertyInfo.Name))
                return propertySerializations[propertyInfo.Name].DynamicInvoke(value) + Environment.NewLine;

            return PrintToString(value, nestingLevel + 1);
        }
    }
}