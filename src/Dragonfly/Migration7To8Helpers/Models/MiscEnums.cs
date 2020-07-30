namespace Dragonfly.Migration7To8Helpers.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public partial class Enums
    {
        #region CustomMigrationHelper
        public enum CustomMigratorTypes
        {
            ICustomFindReplaceDataMigrator,
            ICustomPropToPropDataMigrator
        }
        
        public static Dictionary<CustomMigratorTypes, Type> CustomMigratorTypesWithTypes()
        {
            var dict = new Dictionary<CustomMigratorTypes, Type>();

            dict.Add(CustomMigratorTypes.ICustomFindReplaceDataMigrator, typeof(ICustomFindReplaceDataMigrator));
            dict.Add(CustomMigratorTypes.ICustomPropToPropDataMigrator, typeof(ICustomPropToPropDataMigrator));
           
            return dict;
        }

        public static Type GetCustomMigratorType(CustomMigratorTypes MigratorType)
        {
            var dict = CustomMigratorTypesWithTypes();
            return dict[MigratorType];
        }

        //public static IEnumerable<SelectListItem> FindReplaceTypesSelectList()
        //{
        //    var dict = FindReplaceTypesWithDisplayText();
        //    var options = dict.Select(d => new SelectListItem
        //    {
        //        Value = d.Key.ToString(),
        //        Text = d.Value.ToString()
        //    });

        //    //if default option needed... (string DefaultValue, string DefaultText)
        //    //if (!string.IsNullOrEmpty(DefaultValue))
        //    //{
        //    //    var defaultSelect = Enumerable.Repeat(new SelectListItem
        //    //    {
        //    //        Value = DefaultId,
        //    //        Text = DefaultText
        //    //    }, count: 1);

        //    //    return defaultSelect.Concat(options);
        //    //}

        //    return options;
        //}

        #endregion

    }

}
