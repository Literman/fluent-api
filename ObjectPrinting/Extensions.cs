using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtensions
    {
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, int> config, CultureInfo culture)
        {
            return ((IPropertyPrintingConfig<TOwner, int>) config).PrintingConfig;
        }
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, double> config, CultureInfo culture)
        {
            return ((IPropertyPrintingConfig<TOwner, double>)config).PrintingConfig;
        }

        public static PrintingConfig<TOwner> CutToLength<TOwner>(this PropertyPrintingConfig<TOwner, string> config, int lenght)
        {
            return ((IPropertyPrintingConfig<TOwner, string>) config).PrintingConfig;
        }        
    }

    public static class ObjectExtensions
    {
        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> config)
        {
            return config(ObjectPrinter.For<T>()).PrintToString(obj);
        }

        public static string PrintToString<T>(this T obj)
        {
            return ObjectPrinter.For<T>().PrintToString(obj);
        }
    }
}