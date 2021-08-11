namespace Dragonfly.Migration7To8Helpers.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Umbraco.Core.Composing;
    using Umbraco.Core.Models;


    public interface ICustomFindReplaceDataMigrator : IDiscoverable
    {
        /// <summary>
        /// Allows your custom migrator to test that the provided property is valid for conversion with this Migrator
        /// </summary>
        /// <param name="ContentType"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        bool IsValidForData(int NodeId, string ContentTypeAlias,  PropertyType PropType, object OriginalData, out string NotValidReasonMsg);

        /// <summary>
        /// Does the Conversion
        /// </summary>
        /// <param name="OriginalData"></param>
        /// <returns></returns>
        object ConvertOriginalData(object OriginalData,out string ConversionErrorMsg);
    }
}
