﻿namespace Dragonfly.Migration7To8Helpers.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FormInputsPropertyToProperty
    {
        public string AvailablePropertiesCSV { get; set; }
        public string DocTypeAlias { get; set; }
        public string ContentNodeIdsCsv { get; set; }
        public string PropertyAliasFrom { get; set; }
        public string PropertyAliasTo { get; set; }

        public bool PreviewOnly { get; set; }

        public bool OverwriteExistingData { get; set; }

    }
}