namespace Dragonfly.Migration7To8Helpers.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Umbraco.Core.Models;

    public interface ICustomPropToPropDataMigrator
    {
        bool IsValidForData(int NodeId, string ContentTypeAlias, PropertyType FromPropertyType, object FromPropertyData, PropertyType ToPropertyType, object ToPropertyData, out string NotValidReasonMsg);
        object TransformData(object FromPropData, out string ConversionErrorMsg);
    }
}
