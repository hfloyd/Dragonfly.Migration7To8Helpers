# Dragonfly.Migration7To8Helpers #
Data cleaning/conversion tools for Umbraco 7 to Umbraco 8 migrations created by [Heather Floyd](https://www.HeatherFloyd.com).

## Installation ##
Via NuGet:
[![Nuget Downloads](https://buildstats.info/nuget/Dragonfly.Migration7To8Helpers)](https://www.nuget.org/packages/Dragonfly.Migration7To8Helpers/)

     PM>   Install-Package Dragonfly.Migration7To8Helpers



## Description ##

To be installed into a v8 site right after database upgrade to v8, when data needs to be fixed for compatibility.

***DISCLAIMER*: This tool gives you a lot of power to alter your property data directly. Be responsible. Make database backups in case something goes horribly wrong. Use the "Preview Only" option to check the results before actually updating the data, etc. **

**If you screw up your website, it is your own fault.**

## Usage ##

After Installing, log-in to the Umbraco back-office, then visit:

http://YOURSITE.COM/Umbraco/backoffice/Api/MigrationHelperApi/Start

## Features ##

### General Visibility ###

You can view lists of Doctypes, DataTypes, and PropertyEditors, and see what other items are referencing them. Drill-down and view raw property data.

### Data Conversion Tools ###

#### Property-To-Property ####


#### Find/Replace in PropertyData ####
Allows you to bulk alter a node's property's raw data directly.

**Simple Text-to-Text** - An exact replacement of the entered text inside the property data

**Replace Integer Ids with UDIs** - To update Node IDs with UDIs inside of data (for example - in the case of Nested Content or other JSON), select this option and use the following syntax for your text options:
- Find: Use '~ID~' to indicate the Integer (ex: "imageProp": "~ID~")
- Replacement: Use '~UDI~' to indicate where the UDI should be placed (ex: "imageProp": "~UDI~") 

**Custom Migration** - use your own custom migration code (implementing interface ICustomFindReplaceDataMigrator)



### Custom Data Migrators : ICustomFindReplaceDataMigrator ###
You can create your own custom migrators to update data inside a property. This is useful when you are dealing with changing data formats inside of NestedContent or other more complex data.

Just add a class to your project that implements interface ICustomFindReplaceDataMigrator. The two methods are simple to work with.

####IsValidForData####

     bool IsValidForData(int NodeId, string ContentTypeAlias,  PropertyType PropType, object OriginalData);

Here you can add whatever tests are needed to ensure that your migrator is only operating on the data you want. The calling function will provide you with the current node Id, ContentType Alias, PropertyType, and the current data in the property. You can use what you need to and ignore the rest. Use Services to get more information about the objects as needed.

**Examples:**

    //Check that it is a specific DataType
    string MyDataTypeGuid = "2274379b-de2c-4e8c-bd88-3c1fb2caa5e8";
    
    if (PropType.DataTypeKey.ToString() == MyDataTypeGuid)
    {
    	return true;
    }
    else
    {
    	return false;
    }

--

    //Check that it is a specific Property Alias
	if (PropertyType.PropertyEditorAlias == "Umbraco.NestedContent")
    {
    	return true;
    }
    else
    {
    	return false;
    }



Your migrator will only get called if the original data in the node/property is not NULL. If your function returns "FALSE", the data will NOT be changed for that node/property. 

####ConvertOriginalData####
    
     object ConvertOriginalData(object OriginalData,out string ConversionErrorMsg);


This is where you do the conversion. You can do whatever you want here, just return the "new" data. Keep in mind that you are working with the data in the raw DB format, so objects will be in JSON format, for instance, and should be Serialized before getting returned.

Use the `ConversionErrorMessage` to also return any messages about why data was unable to be converted. In the event of an error, you should probably return the original data unchanged. The ConversionErrorMessage will be shown in the table on the "Find Replace Results" Page

**Some Tips:**
- When dealing with NestedContent or other complex object data, you should create some simple class models for deserializing to make it easier to work with your new data. The quickest way to do that is to copy some example data from the property and use https://app.quicktype.io with the "Attributes Only" option selected.

**Examples:**
See the ['~Custom Migrator Samples' folder](https://github.com/hfloyd/Dragonfly.Migration7To8Helpers/tree/master/src/Dragonfly/~Custom%20Migrator%20Samples) for sample migrators



## Resources ##

GitHub Repository: [https://github.com/hfloyd/Dragonfly.Migration7To8Helpers](https://github.com/hfloyd/Dragonfly.Migration7To8Helpers)