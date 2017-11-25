using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtensions
    {
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, int> config, CultureInfo culture)
        {
            var printingConfig = ((IPropertyPrintingConfig<TOwner, int>)config).PrintingConfig;
            printingConfig.AddCulture(typeof(int), culture);
            return printingConfig;
        }
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, double> config, CultureInfo culture)
        {
            var printingConfig = ((IPropertyPrintingConfig<TOwner, double>)config).PrintingConfig;
            printingConfig.AddCulture(typeof(double), culture);
            return printingConfig;
        }

        public static PrintingConfig<TOwner> CutToLength<TOwner>(this PropertyPrintingConfig<TOwner, string> config, int lenght)
        {
            var printingConfig = ((IPropertyPrintingConfig<TOwner, string>)config).PrintingConfig;
            printingConfig.AddPropertyLength(config.propertyName, lenght);
            return printingConfig;
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