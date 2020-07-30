﻿namespace Dragonfly.Migration7To8Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Dragonfly.Migration7To8Helpers.Models;
    using Dragonfly.NetHelpers;
    using Dragonfly.NetModels;
    using Dragonfly.UmbracoHelpers;
    using Newtonsoft.Json;
    using Umbraco.Core.Migrations.Expressions.Update;
    using Umbraco.Core.Models;
    using Umbraco.Core.PropertyEditors;
    using Umbraco.Web.WebApi;

    // [IsBackOffice]
    // GET: /Umbraco/backoffice/Api/MigrationHelperApi <-- UmbracoAuthorizedApiController

    [IsBackOffice]
    public class MigrationHelperApiController : UmbracoAuthorizedApiController
    {
        private Config _Config = Config.GetConfig();

        #region "Start" Pages

        /// /Umbraco/backoffice/Api/MigrationHelperApi/Start
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage Start()
        {
            var returnSB = new StringBuilder();
            var returnStatusMsg = new StatusMessage(true); //assume success
            var pvPath = $"{_Config.GetViewsPath()}Start.cshtml";

            //Setup
            var ms = new MigrationHelperService();

            //GET DATA TO DISPLAY
            var propertyEditors = ms.AllPropertyEditorTypes;
            var allDataTypes = ms.AllDataTypes;
            var allDocTypes = ms.AllDocTypes;
            var allContentCompositions = ms.GetAllCompositionDocTypes();
            var allContentNodes = ms.AllContent;
            var allMediaTypes = ms.AllMediaTypes;
            var allMediaNodes = ms.AllMedia;
            var allMediaCompositions = ms.GetAllCompositionMediaTypes();

            //UPDATE STATUS MSG
            //returnStatusMsg.Success = true;

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = returnStatusMsg;
            viewData.Add("SpecialMessage", "");
            viewData.Add("SpecialMessageClass", "bg-info");
            viewData.Add("AllPropertyEditorTypes", propertyEditors);
            viewData.Add("AllDataTypes", allDataTypes);
            viewData.Add("AllDocTypes", allDocTypes);
            viewData.Add("AllContentCompositions", allContentCompositions);
            viewData.Add("AllContentNodes", allContentNodes);
            viewData.Add("AllMediaTypes", allMediaTypes);
            viewData.Add("AllMediaCompositions", allMediaCompositions);
            viewData.Add("AllMediaNodes", allMediaNodes);
            //viewData.Add("AllDocTypes", allDocTypes);
            //viewData.Add("AllDocTypes", allDocTypes);

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml =
                ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/MigrationHelperApi/StartPropertyEditorType?PropertyEditorAlias=xx
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage StartPropertyEditorType(string PropertyEditorAlias)
        {
            var returnSB = new StringBuilder();
            var returnStatusMsg = new StatusMessage(true); //assume success
            var pvPath = $"{_Config.GetViewsPath()}StartPropertyEditorType.cshtml";

            //Setup
            var ms = new MigrationHelperService();

            //GET DATA TO DISPLAY
            IDataEditor selEditor = null;
            var allPropertyEditors = ms.AllPropertyEditorTypes;
            var matches = allPropertyEditors.Where(n => n.Alias == PropertyEditorAlias).ToList();
            selEditor = matches.Any() ? matches.First() : null;

            var allRelatedDataTypes = ms.AllDataTypes.Where(n => n.EditorAlias == PropertyEditorAlias).ToList();
            var dataTypeIds = allRelatedDataTypes.Select(n => n.Id).ToList();
            var allRelatedProperties = ms.AllDocTypeProperties.Where(n => dataTypeIds.Contains(n.Property.DataTypeId));
            //UPDATE STATUS MSG
            //returnStatusMsg.Message = "";


            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = returnStatusMsg;
            viewData.Add("SpecialMessage", "");
            viewData.Add("SpecialMessageClass", "bg-info");

            viewData.Add("SelectedPropEditor", selEditor);
            viewData.Add("RelatedDataTypes", allRelatedDataTypes);
            viewData.Add("RelatedProperties", allRelatedProperties);

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml =
                ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/MigrationHelperApi/StartDataType?DataTypeId=xx
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage StartDataType(int DataTypeId)
        {
            var returnSB = new StringBuilder();
            var returnStatusMsg = new StatusMessage(true); //assume success
            var pvPath = $"{_Config.GetViewsPath()}StartDataType.cshtml";

            //Setup
            var ms = new MigrationHelperService();

            //GET DATA TO DISPLAY
            IDataType selDataType = null;
            var allDataTypes = ms.AllDataTypes;
            var matches = allDataTypes.Where(n => n.Id == DataTypeId).ToList();
            selDataType = matches.Any() ? matches.First() : null;

            var allRelatedProperties = ms.AllDocTypeProperties.Where(n => n.Property.DataTypeId == DataTypeId);

            //UPDATE STATUS MSG
            //returnStatusMsg.Message = "";


            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = returnStatusMsg;
            viewData.Add("SpecialMessage", "");
            viewData.Add("SpecialMessageClass", "bg-info");

            viewData.Add("SelectedDataType", selDataType);
            viewData.Add("RelatedProperties", allRelatedProperties);

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml =
                ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/MigrationHelperApi/StartDocType?DocTypeId=xx
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage StartDocType(int DocTypeId)
        {
            var returnSB = new StringBuilder();
            var returnStatusMsg = new StatusMessage(true); //assume success
            var pvPath = $"{_Config.GetViewsPath()}StartDocType.cshtml";

            //Setup
            var ms = new MigrationHelperService();

            //GET DATA TO DISPLAY
            IContentType selDocType = null;
            var allDocTypes = ms.AllDocTypes;
            var matches = allDocTypes.Where(n => n.Id == DocTypeId).ToList();
            selDocType = matches.Any() ? matches.First() : null;

            var allRelatedProperties = ms.AllDocTypeProperties.Where(n => n.DocTypeAlias == selDocType.Alias);

            var allCompositions = ms.GetAllCompositionDocTypes();

            var allContentNodesOfType = ms.AllContent.Where(n => n.ContentTypeId == selDocType.Id);

            //UPDATE STATUS MSG
            //returnStatusMsg.Message = "";


            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = returnStatusMsg;
            viewData.Add("SpecialMessage", "");
            viewData.Add("SpecialMessageClass", "bg-info");

            viewData.Add("SelectedDocType", selDocType);
            viewData.Add("AllCompositions", allCompositions);
            viewData.Add("RelatedProperties", allRelatedProperties);
            viewData.Add("ContentNodesOfType", allContentNodesOfType);

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml =
                ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }


        /// /Umbraco/backoffice/Api/MigrationHelperApi/StartMediaType?MediaTypeId=xx
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage StartMediaType(int MediaTypeId)
        {
            var returnSB = new StringBuilder();
            var returnStatusMsg = new StatusMessage(true); //assume success
            var pvPath = $"{_Config.GetViewsPath()}StartMediaType.cshtml";

            //Setup
            var ms = new MigrationHelperService();

            //GET DATA TO DISPLAY
            IMediaType selMediaType = null;
            var allMediaTypes = ms.AllMediaTypes;
            var matches = allMediaTypes.Where(n => n.Id == MediaTypeId).ToList();
            selMediaType = matches.Any() ? matches.First() : null;

            var allRelatedProperties = ms.AllMediaTypeProperties.Where(n => n.DocTypeAlias == selMediaType.Alias);

            var allCompositions = ms.GetAllCompositionMediaTypes();

            var allMediaNodesOfType = ms.AllMedia.Where(n => n.ContentTypeId == selMediaType.Id);

            //UPDATE STATUS MSG
            //returnStatusMsg.Message = "";


            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = returnStatusMsg;
            viewData.Add("SpecialMessage", "");
            viewData.Add("SpecialMessageClass", "bg-info");

            viewData.Add("SelectedMediaType", selMediaType);
            viewData.Add("AllCompositions", allCompositions);
            viewData.Add("RelatedProperties", allRelatedProperties);
            viewData.Add("MediaNodesOfType", allMediaNodesOfType);

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml =
                ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        #endregion


        #region Data-Editing Pages

        /// /Umbraco/backoffice/Api/MigrationHelperApi/TransferPropertyToPropertySetup?DocTypeAlias=xx
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage TransferPropertyToPropertySetup(int DocTypeId, string PropertyFrom ="", string PropertyTo="")
        {
            var returnSB = new StringBuilder();
            var returnStatusMsg = new StatusMessage(true); //assume success
            var pvPath = $"{_Config.GetViewsPath()}TransferPropertyToProperty.cshtml";

            //Setup
            var ms = new MigrationHelperService();

            //GET DATA TO DISPLAY
            var selDocType = Services.ContentTypeService.Get(DocTypeId);
            var availableProperties = ms.AllDocTypeProperties.Where(n => n.DocTypeAlias == selDocType.Alias);

            var affectedNodes = ms.AllContent.Where(n => n.ContentTypeId == selDocType.Id);
            var nodesList = ViewHelpers.ConvertToCsvIds(affectedNodes);
            var propertyAliases = availableProperties.Select(n => n.Property.Alias).ToList();

            var formInputs = new FormInputsPropertyToProperty();
            formInputs.PreviewOnly = true; //default
            formInputs.AvailablePropertiesCSV = string.Join(",", propertyAliases);
            formInputs.ContentNodeIdsCsv = nodesList;
            formInputs.DocTypeAlias = selDocType.Alias;
            formInputs.PropertyAliasFrom = PropertyFrom;
            formInputs.PropertyAliasTo = PropertyTo;

            //UPDATE STATUS MSG
            //returnStatusMsg.Success = true;

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = returnStatusMsg;
            viewData.Add("SpecialMessage", "");
            viewData.Add("SpecialMessageClass", "bg-info");

            //viewData.Add("AvailableProperties", availableProperties);
            viewData.Add("SelectedDocType", selDocType);
            viewData.Add("AffectedContentNodes", affectedNodes);
            viewData.Add("FormInputs", formInputs);

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml =
                ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/MigrationHelperApi/TransferPropertyToProperty
        [HttpPost]
        public HttpResponseMessage TransferPropertyToProperty(FormInputsPropertyToProperty FormInputs)
        {
            var returnSB = new StringBuilder();
            var returnStatusMsg = new StatusMessage(true); //assume success
            var pvPath = $"{_Config.GetViewsPath()}TransferPropertyToProperty.cshtml";

            //Setup
            var ms = new MigrationHelperService();

            //GET DATA TO DISPLAY
            IContentType selDocType = null;
            IEnumerable<IContent> affectedNodes = new List<IContent>();
            var specialMessage = "";
            var specialMsgClass = "bg-info text-white";

            var resultSet = new PropToPropResultsSet();

            if (FormInputs != null)
            {
                selDocType = Services.ContentTypeService.Get(FormInputs.DocTypeAlias);
                affectedNodes = ms.IdCsvToContents(FormInputs.ContentNodeIdsCsv);

                if (string.IsNullOrEmpty(FormInputs.PropertyAliasFrom)|| string.IsNullOrEmpty(FormInputs.PropertyAliasTo))
                {
                    specialMessage = "Both a 'From' Property and a 'To' Property are required.";
                    specialMsgClass = "bg-danger text-white";
                    //returnStatusMsg.Success = false;
                    //returnStatusMsg.Message =
                    //    $"Form Inputs data was missing - Both a 'From' Property and a 'To' Property are required.";
                }
                else
                {
                    resultSet = ms.ProcessPropertyToProperty(FormInputs);

                    if (!FormInputs.PreviewOnly)
                    {
                        //Do Content updates
                        foreach (var toUpdate in resultSet.Results.Where(n => n.ValidToTransfer))
                        {
                            var key = toUpdate.Key;
                            toUpdate.ContentNode.SetValue(toUpdate.PropertyToAlias, toUpdate.PropertyToData);

                            if (toUpdate.ContentNode.Published)
                            {
                                Services.ContentService.SaveAndPublish(toUpdate.ContentNode);
                                resultSet.Results.Find(p => p.Key == key).ContentUpdated = true;
                            }
                            else
                            {
                                Services.ContentService.Save(toUpdate.ContentNode);
                                resultSet.Results.Find(p => p.Key == key).ContentUpdated = true;
                            }
                        }

                        resultSet.DataUpdatedAndSaved = true;
                    }
                }
            }
            else
            {
                returnStatusMsg.Success = false;
                returnStatusMsg.Message =
                    $"Form Inputs data was missing.";
            }

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = returnStatusMsg;
            viewData.Add("SpecialMessage",specialMessage);
            viewData.Add("SpecialMessageClass", specialMsgClass);

            //viewData.Add("AvailableProperties", availableProperties);
            viewData.Add("SelectedDocType", selDocType);
            viewData.Add("AffectedContentNodes", affectedNodes);
            viewData.Add("FormInputs", FormInputs);
            viewData.Add("Results", resultSet);

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml = ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }


        /// /Umbraco/backoffice/Api/MigrationHelperApi/SetupReplaceInData?DocTypeAlias=xxDataTypeId=xx
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage SetupReplaceInData(string DocTypeAlias, int DataTypeId)
        {
            var returnSB = new StringBuilder();
            var returnStatusMsg = new StatusMessage(true); //assume success
            var pvPath = $"{_Config.GetViewsPath()}FindReplaceInRawData.cshtml";

            //Setup
            var ms = new MigrationHelperService();

            //GET DATA TO DISPLAY
            var selDocType = Services.ContentTypeService.Get(DocTypeAlias);
            var selDataType = Services.DataTypeService.GetDataType(DataTypeId);

            var affectedProperties = ms.AllDocTypeProperties.Where(n => n.DocTypeAlias == DocTypeAlias && n.Property.DataTypeId == DataTypeId);

            var affectedNodes = ms.AllContent.Where(n => n.ContentTypeId == selDocType.Id);
            var nodesList = ViewHelpers.ConvertToCsvIds(affectedNodes);
            var propertyAliases = affectedProperties.Select(n => n.Property.Alias).ToList();

            var formInputs = new FormInputsFindReplace();
            formInputs.PreviewOnly = true; //default
            formInputs.PropertyAliasesCsv = string.Join(",", propertyAliases);
            formInputs.ContentNodeIdsCsv = nodesList;

            //UPDATE STATUS MSG
            //returnStatusMsg.Success = true;

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = returnStatusMsg;
            viewData.Add("SpecialMessage", "");
            viewData.Add("SpecialMessageClass", "bg-info");

            viewData.Add("AffectedProperties", affectedProperties);
            viewData.Add("SelectedDocType", selDocType);
            viewData.Add("SelectedDataType", selDataType);
            viewData.Add("SelectedProperty", null);
            viewData.Add("AffectedContentNodes", affectedNodes);
            viewData.Add("FormInputs", formInputs);

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml =
                ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/MigrationHelperApi/SetupReplaceInData?DocTypeAlias=xxDataTypeId=xx
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage SetupReplaceInData(string DocTypeAlias, string PropertyAlias)
        {
            var returnSB = new StringBuilder();
            var returnStatusMsg = new StatusMessage(true); //assume success
            var pvPath = $"{_Config.GetViewsPath()}FindReplaceInRawData.cshtml";

            //Setup
            var ms = new MigrationHelperService();

            //GET DATA TO DISPLAY
            var selDocType = Services.ContentTypeService.Get(DocTypeAlias);
            
            var affectedProperties = ms.AllDocTypeProperties.Where(n => n.DocTypeAlias == DocTypeAlias && n.Property.Alias == PropertyAlias);
            var selProperty = affectedProperties.First().Property;
            var selDataType = Services.DataTypeService.GetDataType(selProperty.DataTypeId);

            var affectedNodes = ms.AllContent.Where(n => n.ContentTypeId == selDocType.Id);
            var nodesList = ViewHelpers.ConvertToCsvIds(affectedNodes);
            var propertyAliases = affectedProperties.Select(n => n.Property.Alias).ToList();

            var formInputs = new FormInputsFindReplace();
            formInputs.PreviewOnly = true; //default
            formInputs.PropertyAliasesCsv = string.Join(",", propertyAliases);
            formInputs.ContentNodeIdsCsv = nodesList;

            //UPDATE STATUS MSG
            //returnStatusMsg.Success = true;

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = returnStatusMsg;
            viewData.Add("SpecialMessage", "");
            viewData.Add("SpecialMessageClass", "bg-info");

            viewData.Add("AffectedProperties", affectedProperties);
            viewData.Add("SelectedDocType", selDocType);
            viewData.Add("SelectedProperty", selProperty);
            viewData.Add("SelectedDataType", selDataType);
            viewData.Add("AffectedContentNodes", affectedNodes);
            viewData.Add("FormInputs", formInputs);

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml =
                ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/MigrationHelperApi/FindReplace
        [HttpPost]
        public HttpResponseMessage FindReplace(FormInputsFindReplace FormInputs)
        {
            var returnSB = new StringBuilder();
            var returnStatusMsg = new StatusMessage(true); //assume success
            var pvPath = $"{_Config.GetViewsPath()}FindReplaceResults.cshtml";

            //Setup
            var ms = new MigrationHelperService();

            //GET DATA TO DISPLAY
            var resultSet = new FindReplaceResultsSet();

            if (FormInputs != null)
            {
                if (FormInputs.FindReplaceTypeOption== Enums.FindReplaceType.CustomMigration && string.IsNullOrEmpty(FormInputs.CustomMigrationClass))
                {
                    returnStatusMsg.Success = false;
                    returnStatusMsg.Message =
                        $"Custom Migrator option was selected, but no Migration class was provided. Please check form inputs.";
                }

                resultSet = ms.ProcessFindReplace(FormInputs);

                if (!FormInputs.PreviewOnly)
                {
                    //Do Content updates
                    foreach (var toUpdate in resultSet.Results.Where(n => n.MatchFound))
                    {
                        //int index = resultSet.Results.FindIndex(n => n.Key == toUpdate.Key);
                        var key = toUpdate.Key;
                        toUpdate.ContentNode.SetValue(toUpdate.PropertyAlias, toUpdate.NewPropertyData);

                        if (toUpdate.ContentNode.Published)
                        {
                            Services.ContentService.SaveAndPublish(toUpdate.ContentNode);
                            resultSet.Results.Find(p => p.Key == key).ContentUpdated = true;
                        }
                        else
                        {
                            Services.ContentService.Save(toUpdate.ContentNode);
                            resultSet.Results.Find(p => p.Key == key).ContentUpdated = true;
                        }
                    }

                    resultSet.DataUpdatedAndSaved = true;
                }
            }
            else
            {
                returnStatusMsg.Success = false;
                returnStatusMsg.Message =
                    $"Form Inputs data was missing.";
            }

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = returnStatusMsg;
            viewData.Add("FormInputs", FormInputs);
            viewData.Add("FindReplaceResults", resultSet);
            //viewData.Add("SearchResultsSet", searchResults);

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml = ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/MigrationHelperApi/ReplaceWithUdis
        [HttpPost]
        public HttpResponseMessage ReplaceWithUdis(FormInputsFindReplace FormInputs)
        {
            var returnSB = new StringBuilder();
            var returnStatusMsg = new StatusMessage(true); //assume success
            var pvPath = $"{_Config.GetViewsPath()}ReplaceWithUdiResults.cshtml";

            //Setup
            var ms = new MigrationHelperService();

            //GET DATA TO DISPLAY
            var resultSet = new FindReplaceResultsSet();

            if (FormInputs != null)
            {
                resultSet = ms.ProcessFindReplaceIntUdi(FormInputs);

                if (!FormInputs.PreviewOnly)
                {
                    //Do Content updates
                    foreach (var toUpdate in resultSet.Results.Where(n => n.MatchFound))
                    {
                        var key = toUpdate.Key;
                        toUpdate.ContentNode.SetValue(toUpdate.PropertyAlias, toUpdate.NewPropertyData);

                        if (toUpdate.ContentNode.Published)
                        {
                            Services.ContentService.SaveAndPublish(toUpdate.ContentNode);
                            resultSet.Results.Find(p => p.Key == key).ContentUpdated = true;
                        }
                        else
                        {
                            Services.ContentService.Save(toUpdate.ContentNode);
                            resultSet.Results.Find(p => p.Key == key).ContentUpdated = true;
                        }
                    }

                    resultSet.DataUpdatedAndSaved = true;
                }
            }
            else
            {
                returnStatusMsg.Success = false;
                returnStatusMsg.Message =
                    $"Form Inputs data was missing.";
            }

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = returnStatusMsg;
            viewData.Add("FormInputs", FormInputs);
            viewData.Add("FindReplaceResults", resultSet);
            //viewData.Add("SearchResultsSet", searchResults);

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml = ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }



        #endregion


        #region JSON Returns
        /// /Umbraco/backoffice/Api/MigrationHelperApi/TestRaw
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage TestRaw()
        {
            var returnSB = new StringBuilder();
            var returnStatus = new StatusMessage();

            //GET DATA TO DISPLAY
            var migrationService = new MigrationHelperService();
            var allContent = migrationService.AllContent;

            var docTypeToFix = "home";
            var contentOfType = allContent.Where(n => n.ContentType.Alias == docTypeToFix);


            //UPDATE STATUS MSG
            returnStatus.Success = true;
            returnStatus.Message = $"All Content of Type: {docTypeToFix}";
            returnStatus.MessageDetails = $"Number of Content Nodes: {contentOfType.Count()}";
            returnStatus.RelatedObject = contentOfType;

            string json = JsonConvert.SerializeObject(returnStatus);

            returnSB.AppendLine(json);

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "application/json"
                )
            };
        }

        /// /Umbraco/backoffice/Api/MigrationHelperApi/GetAllProperties
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllProperties()
        {
            var returnSB = new StringBuilder();
            var returnStatus = new StatusMessage();

            //GET DATA TO DISPLAY
            var migrationService = new MigrationHelperService();
            var allContent = migrationService.AllContent;

            var propEditor = "Umbraco.NestedContent";
            //var contentOfType = allContent.Where(n => n.ContentType.Alias == docTypeToFix);

            var allProps = migrationService.AllDocTypeProperties;

            var matchingProps = allProps.Where(n => n.Property.PropertyEditorAlias == propEditor);

            //UPDATE STATUS MSG
            returnStatus.Success = true;
            returnStatus.Message = $"All Properties of Property Editor Type: {propEditor}";
            returnStatus.MessageDetails = $"Number of Properties: {matchingProps.Count()}";
            returnStatus.RelatedObject = matchingProps;

            string json = JsonConvert.SerializeObject(returnStatus);

            returnSB.AppendLine(json);

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "application/json"
                )
            };
        }
        #endregion
    }
}
