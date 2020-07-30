namespace Dragonfly.Migration7To8Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.WebPages;
    using Dragonfly.Migration7To8Helpers.Models;
    using Umbraco.Core.Composing.CompositionExtensions;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web.Composing;

    public static class ViewHelpers
    {
        private static ServiceContext _services = Current.Services;
        public static string GetViewsPath()
        {
            var path = Config.GetConfig().GetViewsPath();
            return path;
        }
        public static IEnumerable<DocTypeProperty> GetPropertiesOnNestedContent(int DataTypeId)
        {
            var ms = new MigrationHelperService();
            return ms.GetPropertiesOnNestedContent(DataTypeId);
        }
        public static IEnumerable<DocTypeProperty> GetPropertiesOnNestedContent(IDataType DataType)
        {
            var ms = new MigrationHelperService();
            return ms.GetPropertiesOnNestedContent(DataType);
        }
        public static IEnumerable<IDataType> GetNestedContentDataTypes(string DocTypeAlias)
        {
            var ms = new MigrationHelperService();
            return ms.GetNestedContentDataTypes(DocTypeAlias);
        }
        public static bool IsEligibleForIdToUdiReplace(string PropertyEditorAlias)
        {
            var eligiblePropEditors = new List<string>()
            {
                "Umbraco.ContentPicker",
                "Umbraco.MediaPicker",
                "Umbraco.MemberPicker",
                "Umbraco.MultiUrlPicker",
                "Umbraco.MultiNodeTreePicker"
            };

            return eligiblePropEditors.Contains(PropertyEditorAlias);
        }
        public static string GetDataTypeFolderPath(IDataType DataType, string PathDelim = "/", string RootFolderName = "")
        {
            var folderNames = new List<string>();

            var folderIds = DataType.Path.Split(',');
            foreach (var id in folderIds)
            {
                var folderId = Convert.ToInt32(id);
                var folderContainer = _services.DataTypeService.GetContainer(folderId);
                var folderName = folderContainer != null ? folderContainer.Name : RootFolderName;
                folderNames.Add(folderName);
            }

            return string.Join(PathDelim, folderNames);
        }
        public static string GetDocTypeFolderPath(IContentType ContentType, string PathDelim = "/", string RootFolderName = "")
        {
            var folderNames = new List<string>();

            var folderIds = ContentType.Path.Split(',');
            foreach (var id in folderIds)
            {
                var folderId = Convert.ToInt32(id);
                var folderContainer = _services.ContentTypeService.GetContainer(folderId);
                var folderName = folderContainer != null ? folderContainer.Name : RootFolderName;
                folderNames.Add(folderName);
            }

            return string.Join(PathDelim, folderNames);
        }
        public static string GetMediaTypeFolderPath(IMediaType MediaType, string PathDelim = "/", string RootFolderName = "")
        {
            var folderNames = new List<string>();

            var folderIds = MediaType.Path.Split(',');
            foreach (var id in folderIds)
            {
                var folderId = Convert.ToInt32(id);
                var folderContainer = _services.MediaTypeService.GetContainer(folderId);
                var folderName = folderContainer != null ? folderContainer.Name : RootFolderName;
                folderNames.Add(folderName);
            }

            return string.Join(PathDelim, folderNames);
        }
        public static string ConvertToCsvIds(IEnumerable<IContent> AffectedContentNodes)
        {
            var csv = "";

            var ids = AffectedContentNodes.Select(n => n.Id);
            csv = string.Join(",", ids);

            return csv;
        }
        public static IHtmlString Highlight(string OriginalText, IEnumerable<string> TextStringsToHighlight)
        {
            var newText = OriginalText;
            foreach (var str in TextStringsToHighlight)
            {
                newText = Highlight(newText, str).ToString();
            }

            return new HtmlString(newText);
        }
        public static IHtmlString Highlight(string OriginalText, string TextStringToHighlight)
        {
            if (!string.IsNullOrEmpty(OriginalText))
            {
                if (!string.IsNullOrEmpty(TextStringToHighlight))
                {
                    var markedUp = $"<mark>{TextStringToHighlight}</mark>";
                    var newText = OriginalText.Replace(TextStringToHighlight, markedUp);
                    return new HtmlString(newText);
                }
                else
                {
                    return new HtmlString(OriginalText);
                }
            }
            else
            {
                return new HtmlString("");
            }
        }

        #region Custom Migrations

        //public static IEnumerable<SelectListItem> CustomMigratorsAssembliesSelectList(string DefaultValue, string DefaultText)
        //{
        //    var list = CustomMigratorHelpers.GetAssemblyNames();
        //    var options = list.Select(i => new SelectListItem
        //    {
        //        Value = i.ToString(),
        //        Text = i.ToString()
        //    });

        //    //if default option needed... (string DefaultValue, string DefaultText)
        //    if (!string.IsNullOrEmpty(DefaultValue))
        //    {
        //        var defaultSelect = Enumerable.Repeat(new SelectListItem
        //        {
        //            Value = DefaultValue,
        //            Text = DefaultText
        //        }, count: 1);

        //        return defaultSelect.Concat(options);
        //    }

        //    return options;
        //}

        public static IEnumerable<SelectListItem> CustomFindReplaceMigratorsAllClassesSelectList(string DefaultValue, string DefaultSelectText, string NoOptionsText)
        {
            var list = CustomMigratorHelpers.GetAllFindReplaceClassNames(Enums.CustomMigratorTypes.ICustomFindReplaceDataMigrator);
            var options = list.Select(i => new SelectListItem
            {
                Value = i.ToString(),
                Text = i.ToString()
            });

            if (!options.Any())
            {
                var defaultSelect = Enumerable.Repeat(new SelectListItem
                {
                    Value = DefaultValue,
                    Text = NoOptionsText
                }, count: 1);

                return defaultSelect;
            }
            else
            {
                //if default option needed... (string DefaultValue, string DefaultText)
                if (!string.IsNullOrEmpty(DefaultSelectText))
                {
                    var defaultSelect = Enumerable.Repeat(new SelectListItem
                    {
                        Value = DefaultValue,
                        Text = DefaultSelectText
                    }, count: 1);
                    
                    return defaultSelect.Concat(options);
                }
                else //no default to add
                {
                    return options;
                }
            }
        }

        #endregion

        #region Date/Time
        //public enum TimezoneFormatOption
        //{
        //    Full,
        //    Abbreviated
        //}
        //public static string FormatUtcDateTime(DateTime dt, TimezoneFormatOption TzFormat = TimezoneFormatOption.Full, string LocalTimezone = "", string DateFormat = "ddd MMM d, yyyy", string TimeFormat = "hh:mm tt" )
        //{
        //    //var dateFormat = "ddd MMM d, yyyy";
        //    //var timeFormat = "hh:mm tt";
        //    if (LocalTimezone == "")
        //    {
        //        var configTz = Config.GetConfig().LocalTimezone;
        //        LocalTimezone = configTz!=""? configTz: TimeZoneInfo.Local.Id;
        //    }
        //    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(LocalTimezone);
        //    DateTime localizedDT = TimeZoneInfo.ConvertTimeFromUtc(dt, timeZone);

        //    var zoneFullName = timeZone.IsDaylightSavingTime(localizedDT) ? timeZone.DaylightName : timeZone.StandardName;
        //    var zoneAbbrevName = zoneFullName.Abbreviate();
        //    var zoneInfo = TzFormat == TimezoneFormatOption.Abbreviated ? zoneAbbrevName : zoneFullName;

        //    var stringDate = string.Format("{0} at {1} ({2})", localizedDT.ToString(DateFormat), localizedDT.ToString(TimeFormat), zoneInfo);

        //    return stringDate;
        //}
        #endregion



    }
}
