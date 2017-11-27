using System;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        internal readonly string propertyName;
        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.PrintingConfig => printingConfig;

        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig, string propertyName = null)
        {
            this.printingConfig = printingConfig;
            this.propertyName = propertyName;
        }

        public PrintingConfig<TOwner> Using(Func<TPropType, string> serializeFunc)
        {
            if (propertyName == null)
                printingConfig.AddSerialization(typeof(TPropType), x => serializeFunc((TPropType) x));
            else
                printingConfig.AddSerialization(propertyName, x => serializeFunc((TPropType) x));
            return printingConfig;
        }
    }
}