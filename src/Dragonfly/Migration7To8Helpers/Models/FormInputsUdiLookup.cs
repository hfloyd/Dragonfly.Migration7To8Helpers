using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.Migration7To8Helpers.Models
{
    using System.Web.Mvc;

    partial class Enums
    {
        #region UmbracoObjectType

        public enum UmbracoObjectType
        {
            Content,
            Media,
            ContentType,
            MediaType,
            DataType,
            Unknown
        }

        public static UmbracoObjectType GetUmbracoObjectType(string TypeString)
        {
            switch (TypeString)
            {
                case "Content":
                    return UmbracoObjectType.Content;

                case "Media":
                    return UmbracoObjectType.Media;

                case "ContentType":
                    return UmbracoObjectType.ContentType;

                case "MediaType":
                    return UmbracoObjectType.MediaType;

                case "DataType":
                    return UmbracoObjectType.DataType;

                default:
                    return UmbracoObjectType.Unknown;
            }
        }

        public static Dictionary<UmbracoObjectType, string> UmbracoObjectTypesWithDisplayText()
        {
            var dict = new Dictionary<UmbracoObjectType, string>();

            dict.Add(UmbracoObjectType.Content, "Content Node");
            dict.Add(UmbracoObjectType.Media, "Media Node");
            dict.Add(UmbracoObjectType.ContentType, "Content Type");
            dict.Add(UmbracoObjectType.MediaType, "Media Type");
            dict.Add(UmbracoObjectType.DataType, "DataType");
            dict.Add(UmbracoObjectType.Unknown, "Unknown");
            return dict;
        }

        public static IEnumerable<SelectListItem> UmbracoObjectTypesSelectList()
        {
            var dict = UmbracoObjectTypesWithDisplayText();
            var options = dict.Select(d => new SelectListItem
            {
                Value = d.Key.ToString(),
                Text = d.Value.ToString()
            });

            //if default option needed... (string DefaultValue, string DefaultText)
            //if (!string.IsNullOrEmpty(DefaultValue))
            //{
            //    var defaultSelect = Enumerable.Repeat(new SelectListItem
            //    {
            //        Value = DefaultId,
            //        Text = DefaultText
            //    }, count: 1);

            //    return defaultSelect.Concat(options);
            //}

            return options;
        }


        #endregion
    }

    public class FormInputsUdiLookup
    {
        public string Udi { get; set; }

        public string Guid { get; set; }

        public Enums.UmbracoObjectType ObjectType { get; set; }

    }
}
