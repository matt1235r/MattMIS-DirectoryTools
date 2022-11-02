using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace MattMIS_Directory_Manager
{
    public static class Config
    {

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class ConfigurationModel
        {

            private string aDUserRootField;

            private string serverAddressField;

            private string usernameField;

            private string passwordField;

            /// <remarks/>
            public string ADUserRoot
            {
                get
                {
                    return this.aDUserRootField;
                }
                set
                {
                    this.aDUserRootField = value;
                }
            }

            /// <remarks/>
            public string ServerAddress
            {
                get
                {
                    return this.serverAddressField;
                }
                set
                {
                    this.serverAddressField = value;
                }
            }

            /// <remarks/>
            public string Username
            {
                get
                {
                    return this.usernameField;
                }
                set
                {
                    this.usernameField = value;
                }
            }

            /// <remarks/>
            public string Password
            {
                get
                {
                    return this.passwordField;
                }
                set
                {
                    this.passwordField = value;
                }
            }
        }



        public static ConfigurationModel Settings;

        public static void LoadConfig(string path = "Config.xml")
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlSerializer serializer2 = new XmlSerializer(typeof(ConfigurationModel));
            using (StringReader reader = new StringReader(doc.InnerXml))
            {
                Settings = (ConfigurationModel)serializer2.Deserialize(reader);
            }
        }


    }
}
