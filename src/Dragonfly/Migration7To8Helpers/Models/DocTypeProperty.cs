namespace Dragonfly.Migration7To8Helpers.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Umbraco.Core.Models;

    public class DocTypeProperty
    {
        public string DocTypeAlias { get; set; }
        public string CompositionDocTypeAlias { get; set; }
        public string GroupName { get; set; }

        public PropertyType Property { get; set; }
    }
}
