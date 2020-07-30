namespace Dragonfly.Migration7To8Helpers.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Umbraco.Core.Models;

    public partial class Enums
    {
        public enum PropToPropType
        {
            Copy,
            Transform
        }
    }

    public class PropToPropResultsSet
    {
        public Enums.PropToPropType Type { get; set; }
        public FormInputsPropertyToProperty FormInputs { get; set; }

        public List<PropToPropResult> Results { get; set; }
        public IEnumerable<string> PropertyAliases { get; set; }
        public bool DataUpdatedAndSaved { get; set; }

        public PropToPropResultsSet()
        {
            Results = new List<PropToPropResult>();
        }
    }

    public class PropToPropResult
    {
        public IContent ContentNode { get; set; }

        public string PropertyFromAlias { get; set; }

        public object PropertyFromData { get; set; }

        public string PropertyFromDataFormat { get; set; }
        public string PropertyToAlias { get; set; }
        public object PropertyToData { get; set; }
        public string PropertyToDataFormat { get; set; }
        public bool DataFormatIsNotValidForTransfer { get; set; }
        public bool ValidToTransfer { get; set; }
        public bool ContentUpdated { get; set; }

        public Guid Key { get; set; }
       


        public PropToPropResult()
        {
            Key = Guid.NewGuid();
        }
    }
}
