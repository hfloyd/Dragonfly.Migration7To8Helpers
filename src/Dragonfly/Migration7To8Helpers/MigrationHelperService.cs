namespace Dragonfly.Migration7To8Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Collections;
    using System.Text.RegularExpressions;
    using Dragonfly.Migration7To8Helpers.Models;
    using Newtonsoft.Json;
    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.PropertyEditors;
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Web.Composing;
    using Umbraco.Web.PropertyEditors;

    class MigrationHelperService
    {
        private readonly ILogger _logger;
        private readonly AppCaches _appCaches;
        private readonly UmbracoContext _umbracoContext;
        private readonly UmbracoHelper _umbracoHelper;
        private readonly ServiceContext _services;

        private Config _config;


        #region  Public Properties

        private List<IContent> _allContent = new List<IContent>();
        public IEnumerable<IContent> AllContent
        {
            get
            {
                if (_allContent.Any())
                {
                    return _allContent;
                }
                else
                {
                    FillAllContent();
                    return _allContent;
                }
            }
        }


        private List<IContentType> _allDocTypes = new List<IContentType>();
        public IEnumerable<IContentType> AllDocTypes
        {
            get
            {
                if (_allDocTypes.Any())
                {
                    return _allDocTypes;
                }
                else
                {
                    FillDocTypesList();
                    return _allDocTypes;
                }
            }
        }


        private List<DocTypeProperty> _allDocTypeProperties = new List<DocTypeProperty>();
        public IEnumerable<DocTypeProperty> AllDocTypeProperties
        {
            get
            {
                if (_allDocTypeProperties.Any())
                {
                    return _allDocTypeProperties;
                }
                else
                {
                    FillPropertiesList();
                    return _allDocTypeProperties;
                }
            }
        }


        private List<IDataType> _allDataTypes = new List<IDataType>();
        public IEnumerable<IDataType> AllDataTypes
        {
            get
            {
                if (_allDataTypes.Any())
                {
                    return _allDataTypes;
                }
                else
                {
                    FillDataTypesList();
                    return _allDataTypes;
                }
            }
        }


        private IEnumerable<IDataEditor> _allPropertyEditorTypes = new List<IDataEditor>();
        public IEnumerable<IDataEditor> AllPropertyEditorTypes
        {
            get
            {
                if (_allPropertyEditorTypes.Any())
                {
                    return _allPropertyEditorTypes;
                }
                else
                {
                    FillPropertyEditorsList();
                    return _allPropertyEditorTypes;
                }
            }
        }


        private List<IMediaType> _allMediaTypes = new List<IMediaType>();
        public IEnumerable<IMediaType> AllMediaTypes
        {
            get
            {
                if (_allMediaTypes.Any())
                {
                    return _allMediaTypes;
                }
                else
                {
                    FillMediaTypesList();
                    return _allMediaTypes;
                }
            }
        }


        private List<IMedia> _allMedia = new List<IMedia>();
        public IEnumerable<IMedia> AllMedia
        {
            get
            {
                if (_allMedia.Any())
                {
                    return _allMedia;
                }
                else
                {
                    FillAllMedia();
                    return _allMedia;
                }
            }
        }


        private List<DocTypeProperty> _allMediaTypeProperties = new List<DocTypeProperty>();
        public IEnumerable<DocTypeProperty> AllMediaTypeProperties
        {
            get
            {
                if (_allMediaTypeProperties.Any())
                {
                    return _allMediaTypeProperties;
                }
                else
                {
                    FillMediaPropertiesList();
                    return _allMediaTypeProperties;
                }
            }
        }

        #endregion

        public MigrationHelperService()
        {
            //Services
            this._logger = Current.Logger;
            this._appCaches = Current.AppCaches;
            this._umbracoContext = Current.UmbracoContext;
            this._umbracoHelper = Current.UmbracoHelper;
            this._services = Current.Services;

            //Config Data
            _config = Config.GetConfig();

        }


        #region Lookup Data

        public IEnumerable<IContentTypeComposition> GetAllCompositionDocTypes()
        {
            var docTypes = AllDocTypes;

            var comps = new List<IContentTypeComposition>();
            foreach (var type in docTypes)
            {
                comps.AddRange(type.ContentTypeComposition.ToList());
            }

            return comps.DistinctBy(n => n.Id);
        }

        public IEnumerable<IContentTypeComposition> GetAllCompositionMediaTypes()
        {
            var mediaTypes = AllMediaTypes;

            var comps = new List<IContentTypeComposition>();
            foreach (var type in mediaTypes)
            {
                comps.AddRange(type.ContentTypeComposition.ToList());
            }

            return comps.DistinctBy(n => n.Id);
        }
        public IEnumerable<IDataType> GetNestedContentDataTypes()
        {
            var ncTypes = AllDataTypes.Where(n => n.EditorAlias == "Umbraco.NestedContent");
            return ncTypes;
        }

        public IEnumerable<IDataType> GetNestedContentDataTypes(string DocTypeAlias)
        {
            var ncTypes = GetNestedContentDataTypes();

            var matches = new List<IDataType>();
            foreach (var type in ncTypes)
            {
                var config = type.Configuration as NestedContentConfiguration;
                var ctypes = config.ContentTypes.Select(c => c.Alias);
                if (ctypes.Contains(DocTypeAlias))
                {
                    matches.Add(type);
                }
            }

            return matches;
        }

        public IEnumerable<DocTypeProperty> GetPropertiesOnNestedContent(int DataTypeId)
        {
            var dType = _services.DataTypeService.GetDataType(DataTypeId);
            return GetPropertiesOnNestedContent(dType);
        }

        public IEnumerable<DocTypeProperty> GetPropertiesOnNestedContent(IDataType DataType)
        {
            var propsList = new List<DocTypeProperty>();

            if (DataType.EditorAlias != "Umbraco.NestedContent")
            {
                throw new ArgumentException(
                    $"Provided DataType must be Umbraco.NestedContent. (Argument DataType: {DataType.Id} = {DataType.Name} ({DataType.EditorAlias}))");
            }

            var config = DataType.Configuration as NestedContentConfiguration;
            //var config = JsonConvert.DeserializeObject<NestedContentConfiguration>(configJson.ToString());

            foreach (var contentType in config.ContentTypes)
            {
                var docTypeAlias = contentType.Alias;
                var groupName = contentType.TabAlias;

                //get all related props
                var relatedProps =
                    AllDocTypeProperties.Where(n => n.DocTypeAlias == docTypeAlias && n.GroupName == groupName);

                propsList.AddRange(relatedProps);
            }

            return propsList;
        }

        #endregion

        #region Process Data - Property to Property

        public PropToPropResultsSet ProcessPropertyToProperty(FormInputsPropertyToProperty FormInputs)
        {
            if (string.IsNullOrEmpty(FormInputs.PropertyAliasFrom) ||
                string.IsNullOrEmpty(FormInputs.PropertyAliasTo) || string.IsNullOrEmpty(FormInputs.DocTypeAlias))
            {
                var errMsg = $"ProcessPropertyToProperty: Form Inputs for DocTypeAlias ({FormInputs.DocTypeAlias}),  'To' Property ({FormInputs.PropertyAliasTo}), and 'From' Property ( {FormInputs.PropertyAliasFrom}) are all required.";
                throw new Exception(errMsg);
            }

            var resultSet = new PropToPropResultsSet();
            resultSet.FormInputs = FormInputs;

            resultSet.Type = Enums.PropToPropType.Copy; //default

            //TODO: HLF - Add checking for valid matching transfer types - throw error if data cannot be transferred between different types
            var fromDocTypeProp = AllDocTypeProperties.Where(n =>
                n.DocTypeAlias == FormInputs.DocTypeAlias && n.Property.Alias == FormInputs.PropertyAliasFrom).First();
            var fromPropDataType = _services.DataTypeService.GetDataType(fromDocTypeProp.Property.DataTypeId);
            var fromPropDbType = fromPropDataType.DatabaseType;

            var toDocTypeProp = AllDocTypeProperties.Where(n =>
                n.DocTypeAlias == FormInputs.DocTypeAlias && n.Property.Alias == FormInputs.PropertyAliasTo).First();
            var toPropDataType = _services.DataTypeService.GetDataType(toDocTypeProp.Property.DataTypeId);
            var toPropDbType = toPropDataType.DatabaseType;


            var results = new List<PropToPropResult>();

            //get content
            var nodes = IdCsvToContents(FormInputs.ContentNodeIdsCsv);

            //loop
            foreach (var node in nodes)
            {
                var fromPropMatches = node.Properties.Where(n => n.Alias == FormInputs.PropertyAliasFrom).ToList();
                var fromProp = fromPropMatches.Any() ? fromPropMatches.First() : null;

                var toPropMatches = node.Properties.Where(n => n.Alias == FormInputs.PropertyAliasTo).ToList();
                var toProp = toPropMatches.Any() ? toPropMatches.First() : null;

                if (fromProp == null)
                {
                    var errMsg = $"No property Matching the 'From' Property of {FormInputs.PropertyAliasFrom} exists on Content Node {node.Id}";
                    throw new Exception(errMsg);
                }
                //else if (toProp == null)
                //{
                //    var errMsg = $"No property Matching the 'To' Property of {FormInputs.PropertyAliasTo} exists on Content Node {node.Id}";
                //    throw new Exception(errMsg);
                //}
                else
                {
                    var result = new PropToPropResult();
                    result.ContentNode = node;
                    result.PropertyFromAlias = fromProp.Alias;
                    result.PropertyToAlias = FormInputs.PropertyAliasTo;

                    var fromPropData = fromProp.GetValue();
                    result.PropertyFromData = fromPropData;
                    result.PropertyFromDataFormat = fromPropData != null ? fromPropData.GetType().ToString() : "NULL";

                    var originalToData = toProp != null ? toProp.GetValue() : null;
                    result.PropertyToDataFormat = originalToData != null ? originalToData.GetType().ToString() : "NULL";

                    if (fromPropData != null)
                    {
                        if (toPropDbType == ValueStorageType.Ntext || toPropDbType == ValueStorageType.Nvarchar)
                        {
                            var stringToData = originalToData != null ? originalToData.ToString() : null;
                            if (string.IsNullOrEmpty(stringToData) || FormInputs.OverwriteExistingData)
                            {
                                result.PropertyToData = fromPropData.ToString();
                                result.ValidToTransfer = true;
                            }
                            else
                            {
                                result.PropertyToData = originalToData;
                            }
                        }
                        else //TODO: Support other datatypes - int, datetime, etc.
                        {
                            result.DataFormatIsNotValidForTransfer = true;
                            result.PropertyToData = originalToData;
                        }
                    }

                    results.Add(result);
                }
            }

            resultSet.Results = results;
            return resultSet;
        }

        public PropToPropResultsSet ProcessPropToPropIntToUdi(FormInputsPropertyToProperty FormInputs)
        {
            //TODO: Implement

            throw new NotImplementedException();

            //if (originalData is int)
            //{
            //    var intOriginalData = Convert.ToInt32(originalData);
            //    var lookupNode = _services.ContentService.GetById(intOriginalData);

            //    if (lookupNode != null)
            //    {
            //        result.MatchFound = true;
            //        result.NewPropertyData = lookupNode.GetUdi();
            //    }
            //    else
            //    {
            //        result.NewPropertyData = originalData;
            //    }

            //}
            //else if (originalData is string)
            //{
            //    int intOriginalData;
            //    var isInteger = Int32.TryParse(originalData.ToString(), out intOriginalData);
            //    if (isInteger)
            //    {
            //        var lookupNode = _services.ContentService.GetById(intOriginalData);

            //        if (lookupNode != null)
            //        {
            //            result.MatchFound = true;
            //            result.NewPropertyData = lookupNode.GetUdi();
            //        }
            //        else
            //        {
            //            result.NewPropertyData = originalData;
            //        }
            //    }
            //    else
            //    {
            //        result.DataFormatIsNotValidForReplace = true;
            //        result.NewPropertyData = originalData;
            //    }
            //}
            ////else if (originalData is null)
            ////{
            ////    result.DataFormatIsNotValidForReplace = true;
            ////    result.CurrentDataFormat = "NULL";
            ////    result.NewPropertyData = originalData;
            ////}
            //else
            //{
            //    result.DataFormatIsNotValidForReplace = true;
            //    result.NewPropertyData = originalData;
            //}
        }
        #endregion

        #region Process Data - Find/Replace
        public FindReplaceResultsSet ProcessFindReplace(FormInputsFindReplace FormInputs)
        {
            switch (FormInputs.FindReplaceTypeOption)
            {
                case Enums.FindReplaceType.TextToText:
                    return ProcessFindReplaceText(FormInputs);

                case Enums.FindReplaceType.IntsToUdis:
                    return ProcessFindReplaceIntUdi(FormInputs);

                case Enums.FindReplaceType.CustomMigration:
                    return ProcessFindReplaceCustomMigrator(FormInputs);
                default:
                    return ProcessFindReplaceText(FormInputs);
            }
        }

        private FindReplaceResultsSet ProcessFindReplaceCustomMigrator(FormInputsFindReplace FormInputs)
        {
            var resultSet = new FindReplaceResultsSet();
            resultSet.FormInputs = FormInputs;
            resultSet.Type = Enums.FindReplaceType.CustomMigration;

            var aliases = CsvToEnumerable(FormInputs.PropertyAliasesCsv);
            resultSet.PropertyAliases = aliases;

            var results = new List<FindReplaceResult>();

            //Test for Valid Custom option
            if (string.IsNullOrEmpty(FormInputs.CustomMigrationClass))
            {
                //Error - return empty
                resultSet.HasError = true;
                resultSet.ErrorMessage = "No Custom Migrator Class Provided";
                resultSet.Results = results;
                return resultSet;
            }

            ICustomFindReplaceDataMigrator customMigrator = AssemblyHelpers.GetAssemblyTypeInstance(FormInputs.CustomMigrationClass) as ICustomFindReplaceDataMigrator;

            if (customMigrator == null)
            {
                //Error - return empty
                resultSet.HasError = true;
                resultSet.ErrorMessage = $"Unable to get an instance of Migrator '{FormInputs.CustomMigrationClass}'";
                resultSet.Results = results;
                return resultSet;
            }

            //get content
            var nodes = IdCsvToContents(FormInputs.ContentNodeIdsCsv);

            //loop
            foreach (var node in nodes)
            {
                var propsData = node.Properties.Where(n => aliases.Contains(n.Alias));

                foreach (var propertyData in propsData)
                {
                    var originalData = propertyData.GetValue();

                    var result = new FindReplaceResult();
                    result.ContentNode = node;
                    result.PropertyAlias = propertyData.Alias;
                    result.OriginalPropertyData = originalData;

                    //var testPropertyType = AssemblyHelpers.TestSerializable(typeof(PropertyType));
                    //var testObject = AssemblyHelpers.TestSerializable(typeof(object));
                    //var testIenumString = AssemblyHelpers.TestSerializable(typeof(IEnumerable<string>));
                    //var testListString = AssemblyHelpers.TestSerializable(typeof(List<string>));

                    //Check that migrator is valid for this data
                    if (customMigrator.IsValidForData(node.Id, node.ContentType.Alias, propertyData.PropertyType, originalData))
                    {
                        if (originalData != null)
                        {
                            var conversionError = "";
                            var newData = customMigrator.ConvertOriginalData(originalData, out conversionError);

                            result.NewPropertyData = newData;
                            result.Status = conversionError;
                            result.MatchFound = true;
                        }
                        else
                        {
                            result.NewPropertyData = originalData;
                        }
                    }
                    else
                    {
                        //migrator not valid for this
                        result.DataFormatIsNotValidForReplace = true;
                        result.NewPropertyData = originalData;
                    }

                    results.Add(result);
                }
            }


            resultSet.Results = results;
            return resultSet;
        }


        public FindReplaceResultsSet ProcessFindReplaceText(FormInputsFindReplace FormInputs)
        {
            var resultSet = new FindReplaceResultsSet();
            resultSet.FormInputs = FormInputs;
            resultSet.Type = Enums.FindReplaceType.TextToText;

            var aliases = CsvToEnumerable(FormInputs.PropertyAliasesCsv);
            resultSet.PropertyAliases = aliases;

            var results = new List<FindReplaceResult>();

            //get content
            var nodes = IdCsvToContents(FormInputs.ContentNodeIdsCsv);

            //loop
            foreach (var node in nodes)
            {
                var propsData = node.Properties.Where(n => aliases.Contains(n.Alias));

                foreach (var propertyData in propsData)
                {
                    var originalData = propertyData.GetValue();

                    var result = new FindReplaceResult();
                    result.ContentNode = node;
                    result.PropertyAlias = propertyData.Alias;
                    result.OriginalPropertyData = originalData;
                    result.FindStrings.Add(FormInputs.Find);
                    result.ReplaceStrings.Add(FormInputs.Replace);

                    if (originalData is string)
                    {
                        var stringData = originalData.ToString();
                        if (stringData.Contains(FormInputs.Find))
                        {
                            result.MatchFound = true;
                            result.NewPropertyData = stringData.Replace(FormInputs.Find, FormInputs.Replace);
                        }
                        else
                        {
                            result.NewPropertyData = originalData;
                        }

                    }
                    else
                    {
                        result.DataFormatIsNotValidForReplace = true;
                        result.NewPropertyData = originalData;
                    }

                    results.Add(result);
                }
            }

            resultSet.Results = results;
            return resultSet;
        }

        public FindReplaceResultsSet ProcessFindReplaceIntUdi(FormInputsFindReplace FormInputs)
        {
            //Determine Type of Replacement
            if (FormInputs.FullPropertyReplace)
            {
                if (FormInputs.FullPropertyIsMultiple)
                {
                    return ProcessFR_FullPropMultiple(FormInputs);
                }
                else
                {
                    return ProcessFR_FullPropSingle(FormInputs);
                }
            }
            else
            {
                return ProcessFR_SearchInText(FormInputs);
            }

        }

        private FindReplaceResultsSet ProcessFR_SearchInText(FormInputsFindReplace FormInputs)
        {
            var resultSet = new FindReplaceResultsSet();
            resultSet.FormInputs = FormInputs;
            resultSet.Type = Enums.FindReplaceType.IntsToUdis;

            //Setup
            var aliases = CsvToEnumerable(FormInputs.PropertyAliasesCsv);
            resultSet.PropertyAliases = aliases;

            //Make some alterations to the provided Find and Replace values
            var formFindString = FormInputs.Find;
            var formReplaceString = FormInputs.Replace;

            //If the fields are empty, assume full-field replacement 
            if (string.IsNullOrEmpty(formFindString))
            {
                formFindString = "~ID~";
            }
            else if (formFindString.Contains(@"\"))
            {
                //escape special chars in Find for regex
                formFindString = formFindString.Replace(@"\", @"\\");
            }

            if (string.IsNullOrEmpty(formReplaceString))
            {
                formReplaceString = "~UDI~";
            }

            var results = new List<FindReplaceResult>();

            //get content
            var nodes = IdCsvToContents(FormInputs.ContentNodeIdsCsv);

            //loop
            foreach (var node in nodes)
            {
                var propsData = node.Properties.Where(n => aliases.Contains(n.Alias));

                foreach (var propertyData in propsData)
                {
                    var originalData = propertyData.GetValue();

                    var result = new FindReplaceResult();
                    result.ContentNode = node;
                    result.PropertyAlias = propertyData.Alias;
                    result.OriginalPropertyData = originalData;
                    result.CurrentDataFormat = originalData != null ? originalData.GetType().ToString() : "NULL";

                    if (originalData is string)
                    {
                        //Since we are looping, this value with continually be updated with each match
                        var newData = originalData.ToString();

                        //Use Regex to locate the "Find" substring
                        var constructRegex = formFindString.Replace("~ID~", @"\d+");
                        Regex regexFindString = new Regex(constructRegex);

                        var findMatches = regexFindString.Matches(originalData.ToString());
                        if (findMatches.Count > 0)
                        {
                            //Loop through all matches replacing values
                            foreach (Match findMatch in findMatches)
                            {
                                var findString = findMatch.Value;
                                result.FindStrings.Add(findString);

                                //Get the INT ID
                                Regex regexId = new Regex(@"\d+");
                                Match idMatch = regexId.Match(findString);
                                if (idMatch.Success)
                                {
                                    string foundId = idMatch.Value;

                                    int intOriginalId;
                                    var isInteger = Int32.TryParse(foundId, out intOriginalId);
                                    if (isInteger)
                                    {
                                        //Get the new UDI
                                        var lookupContentNode = _services.ContentService.GetById(intOriginalId);
                                        if (lookupContentNode != null)
                                        {
                                            //Create the Replace string
                                            var replaceString =
                                                formReplaceString.Replace("~UDI~", lookupContentNode.GetUdi().ToString());
                                            result.ReplaceStrings.Add(replaceString);

                                            //UPDATE THE DATA
                                            newData = newData.Replace(findString, replaceString);
                                        }
                                        else
                                        {
                                            //Try Media
                                            var lookupMediaNode = _services.MediaService.GetById(intOriginalId);
                                            if (lookupMediaNode != null)
                                            {
                                                //Create the Replace string
                                                var replaceString =
                                                    formReplaceString.Replace("~UDI~", lookupMediaNode.GetUdi().ToString());
                                                result.ReplaceStrings.Add(replaceString);

                                                //UPDATE THE DATA
                                                newData = newData.Replace(findString, replaceString);
                                            }
                                            else
                                            {
                                                //no node found, add a message and continue
                                                var msg = $"Unable to find a matching node for Id {intOriginalId}";
                                                result.Status = result.Status + "; " + msg;
                                                result.ReplaceStrings.Add($"[{msg}]");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Not a valid int, add a message and continue
                                        var msg = $"{foundId} is not a valid Integer Id ({findString})";
                                        result.Status = result.Status + "; " + msg;
                                        result.ReplaceStrings.Add($"[{msg}]");
                                    }
                                }
                                else
                                {
                                    //Unable to get Int ID, add a message and continue
                                    var msg = $"Unable to get a valid Integer Id for {findString}";
                                    result.Status = result.Status + "; " + msg;
                                    result.ReplaceStrings.Add($"[{msg}]");
                                }
                            }

                            //All done replacing...
                            result.Status = $"OK";
                            result.MatchFound = true;
                            result.NewPropertyData = newData;
                        }
                        else
                        {
                            //No matches found
                            result.Status = $"No Match";
                            result.NewPropertyData = originalData;
                        }
                    }
                    else
                    {
                        //Original Data is not a string value
                        result.Status = $"Original Data is not a string value";
                        result.DataFormatIsNotValidForReplace = true;
                        result.NewPropertyData = originalData;
                    }

                    results.Add(result);
                }
            }

            resultSet.Results = results;
            return resultSet;
        }

        private FindReplaceResultsSet ProcessFR_FullPropSingle(FormInputsFindReplace FormInputs)
        {
            //TODO: Simplify this
            var resultSet = new FindReplaceResultsSet();
            resultSet.FormInputs = FormInputs;
            resultSet.Type = Enums.FindReplaceType.IntsToUdis;

            //Setup
            var aliases = CsvToEnumerable(FormInputs.PropertyAliasesCsv);
            resultSet.PropertyAliases = aliases;



            //Make some alterations to the provided Find and Replace values
            var formFindString = FormInputs.Find;
            var formReplaceString = FormInputs.Replace;

            //If the fields are empty, assume full-field replacement 
            if (string.IsNullOrEmpty(formFindString))
            {
                formFindString = "~ID~";
            }
            else if (formFindString.Contains(@"\"))
            {
                //escape special chars in Find for regex
                formFindString = formFindString.Replace(@"\", @"\\");
            }

            if (string.IsNullOrEmpty(formReplaceString))
            {
                formReplaceString = "~UDI~";
            }


            var results = new List<FindReplaceResult>();

            //get content
            var nodes = IdCsvToContents(FormInputs.ContentNodeIdsCsv);

            //loop
            foreach (var node in nodes)
            {
                var propsData = node.Properties.Where(n => aliases.Contains(n.Alias));

                foreach (var propertyData in propsData)
                {
                    var originalData = propertyData.GetValue();

                    var result = new FindReplaceResult();
                    result.ContentNode = node;
                    result.PropertyAlias = propertyData.Alias;
                    result.OriginalPropertyData = originalData;
                    result.CurrentDataFormat = originalData != null ? originalData.GetType().ToString() : "NULL";

                    if (originalData is string)
                    {
                        //Since we are looping, this value with continually be updated with each match
                        var newData = originalData.ToString();

                        //Use Regex to locate the "Find" substring
                        var constructRegex = formFindString.Replace("~ID~", @"\d+");
                        Regex regexFindString = new Regex(constructRegex);

                        var findMatches = regexFindString.Matches(originalData.ToString());
                        if (findMatches.Count > 0)
                        {
                            //Loop through all matches replacing values
                            foreach (Match findMatch in findMatches)
                            {
                                var findString = findMatch.Value;
                                result.FindStrings.Add(findString);

                                //Get the INT ID
                                Regex regexId = new Regex(@"\d+");
                                Match idMatch = regexId.Match(findString);
                                if (idMatch.Success)
                                {
                                    string foundId = idMatch.Value;

                                    int intOriginalId;
                                    var isInteger = Int32.TryParse(foundId, out intOriginalId);
                                    if (isInteger)
                                    {
                                        //Get the new UDI
                                        var lookupContentNode = _services.ContentService.GetById(intOriginalId);
                                        if (lookupContentNode != null)
                                        {
                                            //Create the Replace string
                                            var replaceString =
                                                formReplaceString.Replace("~UDI~", lookupContentNode.GetUdi().ToString());
                                            result.ReplaceStrings.Add(replaceString);

                                            //UPDATE THE DATA
                                            newData = newData.Replace(findString, replaceString);
                                        }
                                        else
                                        {
                                            //Try Media
                                            var lookupMediaNode = _services.MediaService.GetById(intOriginalId);
                                            if (lookupMediaNode != null)
                                            {
                                                //Create the Replace string
                                                var replaceString =
                                                    formReplaceString.Replace("~UDI~", lookupMediaNode.GetUdi().ToString());
                                                result.ReplaceStrings.Add(replaceString);

                                                //UPDATE THE DATA
                                                newData = newData.Replace(findString, replaceString);
                                            }
                                            else
                                            {
                                                //no node found, add a message and continue
                                                var msg = $"Unable to find a matching node for Id {intOriginalId}";
                                                result.Status = result.Status + "; " + msg;
                                                result.ReplaceStrings.Add($"[{msg}]");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Not a valid int, add a message and continue
                                        var msg = $"{foundId} is not a valid Integer Id ({findString})";
                                        result.Status = result.Status + "; " + msg;
                                        result.ReplaceStrings.Add($"[{msg}]");
                                    }
                                }
                                else
                                {
                                    //Unable to get Int ID, add a message and continue
                                    var msg = $"Unable to get a valid Integer Id for {findString}";
                                    result.Status = result.Status + "; " + msg;
                                    result.ReplaceStrings.Add($"[{msg}]");
                                }
                            }

                            //All done replacing...
                            result.Status = $"OK";
                            result.MatchFound = true;
                            result.NewPropertyData = newData;
                        }
                        else
                        {
                            //No matches found
                            result.Status = $"No Match";
                            result.NewPropertyData = originalData;
                        }
                    }
                    else
                    {
                        //Original Data is not a string value
                        result.Status = $"Original Data is not a string value";
                        result.DataFormatIsNotValidForReplace = true;
                        result.NewPropertyData = originalData;
                    }

                    results.Add(result);
                }
            }

            resultSet.Results = results;
            return resultSet;
        }

        private FindReplaceResultsSet ProcessFR_FullPropMultiple(FormInputsFindReplace FormInputs)
        {
            var resultSet = new FindReplaceResultsSet();
            resultSet.FormInputs = FormInputs;
            resultSet.Type = Enums.FindReplaceType.IntsToUdis;

            //Setup
            var aliases = CsvToEnumerable(FormInputs.PropertyAliasesCsv);
            resultSet.PropertyAliases = aliases;

            var results = new List<FindReplaceResult>();

            //get content
            var nodes = IdCsvToContents(FormInputs.ContentNodeIdsCsv);

            //loop
            foreach (var node in nodes)
            {
                var propsData = node.Properties.Where(n => aliases.Contains(n.Alias));

                foreach (var propertyData in propsData)
                {
                    var originalData = propertyData.GetValue();

                    var result = new FindReplaceResult();
                    result.ContentNode = node;
                    result.PropertyAlias = propertyData.Alias;
                    result.OriginalPropertyData = originalData;
                    result.CurrentDataFormat = originalData != null ? originalData.GetType().ToString() : "NULL";

                    if (originalData is string)
                    {
                        //Since we are looping, this value with continually be updated with each match
                        var newDataList = new List<string>();

                        //Get list of all Ids
                        var originalIdsList = SplitDataIntoList(originalData.ToString());

                        //Use Regex to locate the "Find" substring
                        //var constructRegex = formFindString.Replace("~ID~", @"\d+");
                        //Regex regexFindString = new Regex(constructRegex);

                        //var findMatches = regexFindString.Matches(originalData.ToString());

                        //Loop through all matches replacing values
                        foreach (var strId in originalIdsList)
                        {
                            result.FindStrings.Add(strId);

                            //Get the INT ID
                            int intOriginalId;
                            var isInteger = Int32.TryParse(strId, out intOriginalId);
                            if (isInteger)
                            {
                                //Get the new UDI
                                var lookupContentNode = _services.ContentService.GetById(intOriginalId);
                                if (lookupContentNode != null)
                                {
                                    //Add to New Data
                                    var contentUdi = lookupContentNode.GetUdi().ToString();
                                    newDataList.Add(contentUdi);
                                    result.ReplaceStrings.Add(contentUdi);
                                    result.MatchFound = true;
                                }
                                else
                                {
                                    //Try Media
                                    var lookupMediaNode = _services.MediaService.GetById(intOriginalId);
                                    if (lookupMediaNode != null)
                                    {
                                        //Add to New Data
                                        var mediaUdi = lookupMediaNode.GetUdi().ToString();
                                        newDataList.Add(mediaUdi);
                                        result.ReplaceStrings.Add(mediaUdi);
                                        result.MatchFound = true;
                                    }
                                    else
                                    {
                                        //no node found, add a message and continue
                                        var msg = $"Unable to find a matching node for Id {intOriginalId}";
                                        result.Status = result.Status + "; " + msg;
                                        newDataList.Add(strId); //Original ID added again
                                        result.ReplaceStrings.Add($"[{msg}]");
                                    }
                                }
                            }
                            else
                            {
                                //Not a valid int, add a message and continue
                                var msg = $"{strId} is not a valid Integer Id";
                                result.Status = result.Status + "; " + msg;
                                newDataList.Add(strId); //Original ID added again
                                result.ReplaceStrings.Add($"[{msg}]");
                            }
                        }

                        //All done replacing for this propertyData...
                        result.NewPropertyData = string.Join(",", newDataList);
                    }
                    else
                    {
                        //Original Data is not a string value
                        result.Status = $"Original Data is not a string value";
                        result.DataFormatIsNotValidForReplace = true;
                        result.NewPropertyData = originalData;
                    }

                    results.Add(result);
                }
            }

            //All nodes done
            resultSet.Results = results;
            return resultSet;
        }

        private IEnumerable<string> SplitDataIntoList(string OriginalData)
        {
            if (OriginalData.DetectIsJson())
            {
                //TODO: Figure out JSON parsing?
                return OriginalData.AsEnumerableOfOne();
            }
            else
            {
                //Assume CSV
                return OriginalData.Split(',');
            }
        }

        public IEnumerable<string> CsvToEnumerable(string StringsCsv)
        {
            if (StringsCsv != null)
            {
                var strings = StringsCsv.Split(',').ToList();

                return strings;
            }
            else
            {
                return new List<string>();
            }
        }

        public IEnumerable<IContent> IdCsvToContents(string IdsCsv)
        {
            if (IdsCsv != null)
            {
                var idStrings = IdsCsv.Split(',');
                var idInts = idStrings.Select(n => Convert.ToInt32(n));
                var nodes = _services.ContentService.GetByIds(idInts);

                return nodes;
            }
            else
            {
                return new List<IContent>();
            }
        }

        #endregion

        #region Fill Properties with Collections

        private void FillDocTypesList()
        {
            _allDocTypes = _services.ContentTypeService.GetAll().ToList();
        }
        private void FillMediaTypesList()
        {
            _allMediaTypes = _services.MediaTypeService.GetAll().ToList();
        }
        private void FillDataTypesList()
        {
            _allDataTypes = _services.DataTypeService.GetAll().ToList();
        }
        private void FillPropertyEditorsList()
        {
            var allEditors = AllDataTypes.Select(n => n.Editor);
            _allPropertyEditorTypes = allEditors.DistinctBy(n => n.Alias);
        }
        private void FillPropertiesList()
        {
            var docTypes = this.AllDocTypes;
            var props = new List<DocTypeProperty>();

            foreach (var docType in docTypes)
            {
                //var dtCompsCount = docType.ContentTypeComposition.Count();

                //Props in No Group
                foreach (var propType in docType.NoGroupPropertyTypes)
                {
                    var dtProp = new DocTypeProperty();
                    dtProp.DocTypeAlias = docType.Alias;
                    dtProp.GroupName = "";
                    dtProp.Property = propType;

                    props.Add(dtProp);
                }

                //Props in groups
                foreach (var propGroup in docType.PropertyGroups)
                {
                    foreach (var propType in propGroup.PropertyTypes)
                    {
                        var dtProp = new DocTypeProperty();
                        dtProp.DocTypeAlias = docType.Alias;
                        dtProp.GroupName = propGroup.Name;
                        dtProp.Property = propType;

                        props.Add(dtProp);
                    }
                }

                //Props in Compositions
                foreach (var comp in docType.ContentTypeComposition)
                {
                    //Comp Props in No Group
                    foreach (var propType in comp.NoGroupPropertyTypes)
                    {
                        var dtProp = new DocTypeProperty();
                        dtProp.DocTypeAlias = docType.Alias;
                        dtProp.GroupName = "";
                        dtProp.Property = propType;
                        dtProp.CompositionDocTypeAlias = comp.Alias;

                        props.Add(dtProp);
                    }

                    //Comp Props in groups
                    foreach (var propGroup in comp.PropertyGroups)
                    {
                        foreach (var propType in propGroup.PropertyTypes)
                        {
                            var dtProp = new DocTypeProperty();
                            dtProp.DocTypeAlias = docType.Alias;
                            dtProp.GroupName = propGroup.Name;
                            dtProp.Property = propType;
                            dtProp.CompositionDocTypeAlias = comp.Alias;

                            props.Add(dtProp);
                        }
                    }
                }

                //props.AddRange(docType.NoGroupPropertyTypes);
                //props.AddRange(docType.CompositionPropertyTypes);
            }

            _allDocTypeProperties = props;
        }

        private void FillMediaPropertiesList()
        {
            var mediaTypes = this.AllMediaTypes;
            var props = new List<DocTypeProperty>();

            foreach (var type in mediaTypes)
            {
                //var dtCompsCount = docType.ContentTypeComposition.Count();

                //Props in No Group
                foreach (var propType in type.NoGroupPropertyTypes)
                {
                    var dtProp = new DocTypeProperty();
                    dtProp.DocTypeAlias = type.Alias;
                    dtProp.GroupName = "";
                    dtProp.Property = propType;

                    props.Add(dtProp);
                }

                //Props in groups
                foreach (var propGroup in type.PropertyGroups)
                {
                    foreach (var propType in propGroup.PropertyTypes)
                    {
                        var dtProp = new DocTypeProperty();
                        dtProp.DocTypeAlias = type.Alias;
                        dtProp.GroupName = propGroup.Name;
                        dtProp.Property = propType;

                        props.Add(dtProp);
                    }
                }

                //Props in Compositions
                foreach (var comp in type.ContentTypeComposition)
                {
                    //Comp Props in No Group
                    foreach (var propType in comp.NoGroupPropertyTypes)
                    {
                        var dtProp = new DocTypeProperty();
                        dtProp.DocTypeAlias = type.Alias;
                        dtProp.GroupName = "";
                        dtProp.Property = propType;
                        dtProp.CompositionDocTypeAlias = comp.Alias;

                        props.Add(dtProp);
                    }

                    //Comp Props in groups
                    foreach (var propGroup in comp.PropertyGroups)
                    {
                        foreach (var propType in propGroup.PropertyTypes)
                        {
                            var dtProp = new DocTypeProperty();
                            dtProp.DocTypeAlias = type.Alias;
                            dtProp.GroupName = propGroup.Name;
                            dtProp.Property = propType;
                            dtProp.CompositionDocTypeAlias = comp.Alias;

                            props.Add(dtProp);
                        }
                    }
                }

                //props.AddRange(docType.NoGroupPropertyTypes);
                //props.AddRange(docType.CompositionPropertyTypes);
            }

            _allMediaTypeProperties = props;
        }
        public void FillAllContent()
        {
            var rootContent = _services.ContentService.GetRootContent();
            if (rootContent != null && rootContent.Any())
            {
                foreach (var c in rootContent)
                {
                    FillRecursiveContent(c);
                }
            }
        }
        private void FillRecursiveContent(IContent Content)
        {
            if (Content != null && !Content.Trashed)
            {
                _allContent.Add(Content);

                if (_services.ContentService.HasChildren(Content.Id))
                {
                    var countChildren = _services.ContentService.CountChildren(Content.Id);
                    long xTotalRecs;
                    var allChildren =
                        _services.ContentService.GetPagedChildren(Content.Id, 0, countChildren, out xTotalRecs);

                    foreach (var child in allChildren)
                    {
                        FillRecursiveContent(child);
                    }
                }
            }
        }

        private void FillAllMedia()
        {
            var rootMedia = _services.MediaService.GetRootMedia();
            if (rootMedia != null && rootMedia.Any())
            {
                foreach (var m in rootMedia)
                {
                    FillRecursiveMedia(m);
                }
            }
        }
        private void FillRecursiveMedia(IMedia MediaItem)
        {
            if (MediaItem != null && !MediaItem.Trashed)
            {
                _allMedia.Add(MediaItem);

                if (_services.MediaService.HasChildren(MediaItem.Id))
                {
                    var countChildren = _services.MediaService.CountChildren(MediaItem.Id);
                    long xTotalRecs;
                    var allChildren =
                        _services.MediaService.GetPagedChildren(MediaItem.Id, 0, countChildren, out xTotalRecs);

                    foreach (var child in allChildren)
                    {
                        FillRecursiveMedia(child);
                    }
                }
            }
        }

        #endregion


        #region Looping Example

        //public void LoopAllContent()
        //{
        //    var rootContent = _services.ContentService.GetRootContent();
        //    if (rootContent != null)
        //    {
        //        foreach (var c in rootContent)
        //        {
        //            RecursiveLoopNodes(c);
        //        }
        //    }
        //}

        //private void RecursiveLoopNodes(IContent Content)
        //{
        //    if (Content != null && !Content.Trashed)
        //    {
        //        //Do something

        //        if (_services.ContentService.HasChildren(Content.Id))
        //        {
        //            var countChildren = _services.ContentService.CountChildren(Content.Id);
        //            long xTotalRecs;
        //            var allChildren = _services.ContentService.GetPagedChildren(Content.Id, 0, countChildren, out xTotalRecs);

        //            foreach (var child in allChildren)
        //            {
        //                RecursiveLoopNodes(child);
        //            }
        //        }
        //    }
        //}

        #endregion


    }
}
