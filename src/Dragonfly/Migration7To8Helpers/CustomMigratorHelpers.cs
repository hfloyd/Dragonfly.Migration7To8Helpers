namespace Dragonfly.Migration7To8Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.Web.Http;
    using Dragonfly.Migration7To8Helpers.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web.Composing;
    using Umbraco.Web.PropertyEditors;

    public static class CustomMigratorHelpers
    {
        private static ServiceContext _services = Current.Services;

        #region Public

        public static IEnumerable<string> GetNestedContentPropertyDocTypes(int DataTypeId)
        {
            var list = new List<string>();

            var dType = _services.DataTypeService.GetDataType(DataTypeId);
            if (dType.EditorAlias != "Umbraco.NestedContent")
            {
                throw new ArgumentException(
                    $"Provided DataType must be Umbraco.NestedContent. (Argument DataType: {dType.Id} = {dType.Name} ({dType.EditorAlias}))");
            }

            var config = dType.Configuration as NestedContentConfiguration;
            //var config = JsonConvert.DeserializeObject<NestedContentConfiguration>(configJson.ToString());

            foreach (var contentType in config.ContentTypes)
            {
                var docTypeAlias = contentType.Alias;
                var groupName = contentType.TabAlias;

                list.Add(docTypeAlias);
            }

            return list;
        }

        #endregion


        #region Internal 
        /// <summary>
        /// Gets All Assemblies of a Specified Custom Migrator Type
        /// </summary>
        /// <param name="MigratorType"></param>
        /// <returns></returns>
        internal static IEnumerable<string> GetAssemblyNames(Enums.CustomMigratorTypes MigratorType)
        {
            var systemType = Enums.GetCustomMigratorType(MigratorType);

            List<string> assemblyNames = new List<string>();

            foreach (string assemblyName in AssemblyHelpers.GetAssemblyNames())
            {
                Assembly assembly = AssemblyHelpers.GetAssembly(assemblyName);

                if (assembly != null)
                {
                    if (assembly.GetLoadableTypes().Any(x => systemType.IsAssignableFrom(x)))
                    {
                        assemblyNames.Add(assemblyName);
                    }
                }
            }

            return assemblyNames;
        }

        /// <summary>
        /// Gets Classed from a specified Assembly of a Specified Custom Migrator Type
        /// </summary>
        /// <param name="MigratorType"></param>
        /// <returns></returns>
        internal static IEnumerable<string> GetClassNames(string AssemblyName, Enums.CustomMigratorTypes MigratorType)
        {
            var systemType = Enums.GetCustomMigratorType(MigratorType);
            Assembly assembly = AssemblyHelpers.GetAssembly(AssemblyName);

            if (assembly != null)
            {
                return assembly
                    .GetLoadableTypes()
                    .Where(x => systemType.IsAssignableFrom(x))
                    .Select(x => x.FullName);
            }

            return null;
        }

        /// <summary>
        /// Gets All Classes of a Specified Custom Migrator Type
        /// </summary>
        /// <param name="MigratorType"></param>
        /// <returns></returns>
        internal static IEnumerable<string> GetAllMigratorClassNames(Enums.CustomMigratorTypes MigratorType)
        {
            var finalList = new List<string>();
            var systemType = Enums.GetCustomMigratorType(MigratorType);
            var allAssemblies = GetAssemblyNames(MigratorType);

            foreach (var assemblyName in allAssemblies)
            {
                Assembly assembly = AssemblyHelpers.GetAssembly(assemblyName);

                if (assembly != null)
                {
                    var names =  assembly
                        .GetLoadableTypes()
                        .Where(x => systemType.IsAssignableFrom(x))
                        .Select(x => x.FullName);

                    finalList.AddRange(names);
                }
            }

            return finalList;
        }

#endregion

    }
}
