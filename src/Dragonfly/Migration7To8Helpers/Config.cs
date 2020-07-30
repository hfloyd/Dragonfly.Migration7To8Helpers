namespace Dragonfly.Migration7To8Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Hosting;
    using System.Xml.Serialization;

    ///// <summary>
    ///// Configuration settings read from a config XML file
    ///// </summary>
    //[XmlRoot("Dragonfly")]
    public class Config
    {
        private const string ConfigFileName = "Dragonfly.config";
        private const string AppPluginsPath = "/App_Plugins/Dragonfly.Migration7To8Helpers/";
        private const string DefaultDataPath = "/App_Data/Dragonfly.Migration7To8Helpers/";
        private static Config _config;

        public DateTime ConfigTimestamp { get; set; }

        //[XmlElement]
        //public string LocalTimezone { get; set; }

        //[XmlArray("MultipleThings")]
        //[XmlArrayItem("Thing")]
        //public List<string> MultipleThings = new List<string>();

        public static Config GetConfig()
        {
            _config = new Config();

            ////Read XML Config
            //string path = $"~/Config/{ConfigFileName}";
            //string pathMapped = HostingEnvironment.MapPath(path);
            //var lastModified = System.IO.File.GetLastWriteTime(pathMapped);

            //if (_config == null || lastModified > _config.ConfigTimestamp)
            //{
            //    XmlSerializer serializer = new XmlSerializer(typeof(Config));

            //    // Read from file
            //    try
            //    {
            //        using (var reader = new StreamReader(pathMapped))
            //        {
            //            _config = (Config)serializer.Deserialize(reader);
            //        }
            //    }
            //    catch (InvalidOperationException ex)
            //    {
            //        throw new InvalidOperationException($"The format of '{ConfigFileName}' is invalid. Details: " + ex.Message, ex.InnerException);
            //    }

            //    _config.ConfigTimestamp = lastModified;
            //}

            return _config;
        }

        public string GetAppPluginsPath(bool ReturnMapped = false)
        {
            var path = AppPluginsPath;

            if (ReturnMapped)
            {
                return HostingEnvironment.MapPath(path);
            }
            else
            {
                return path;
            }
        }

        public string GetViewsPath()
        {
            var path = GetAppPluginsPath() + "Views/";
            return path;
        }

        public string GetDataPath(bool ReturnMapped = false)
        {
            var path = DefaultDataPath;
            //var path = _config.DataStoragePath != "" ? _config.DataStoragePath : DefaultDataPath;

            if (ReturnMapped)
            {
                return HostingEnvironment.MapPath(path);
            }
            else
            {
                return path;
            }
        }
    }
}
