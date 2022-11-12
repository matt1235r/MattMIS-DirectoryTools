using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MattMIS_Sync
{
    public static class Config
    {
        public static ConfigurationModel AppConfig;
        public static void LoadConfig(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlSerializer serializer2 = new XmlSerializer(typeof(ConfigurationModel));
            using (StringReader reader = new StringReader(doc.InnerXml))
            {
                AppConfig = (ConfigurationModel)serializer2.Deserialize(reader);                
            }
            //Iterate through all staff in this report
            //remeber current staff member 
            
        }

            // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
            /// <remarks/>
            [System.SerializableAttribute()]
            [System.ComponentModel.DesignerCategoryAttribute("code")]
            [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
            [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
            public partial class ConfigurationModel
            {

            private ConfigActiveDirectory activeDirectoryField;

            private ConfigMISExports mISExportsField;

            private ConfigAccountProvisioning accountProvisioningField;

            private ConfigReporting reportingField;

            /// <remarks/>
            public ConfigActiveDirectory ActiveDirectory
            {
                get
                {
                    return this.activeDirectoryField;
                }
                set
                {
                    this.activeDirectoryField = value;
                }
            }

            /// <remarks/>
            public ConfigMISExports MISExports
            {
                get
                {
                    return this.mISExportsField;
                }
                set
                {
                    this.mISExportsField = value;
                }
            }

            /// <remarks/>
            public ConfigAccountProvisioning AccountProvisioning
            {
                get
                {
                    return this.accountProvisioningField;
                }
                set
                {
                    this.accountProvisioningField = value;
                }
            }

            /// <remarks/>
            public ConfigReporting Reporting
            {
                get
                {
                    return this.reportingField;
                }
                set
                {
                    this.reportingField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class ConfigActiveDirectory
        {

            private string userGroupsRootOUField;

            private string staffRootOUField;

            private string shortDomainName;

            private string studentRootOUField;

            /// <remarks/>
            public string UserGroupsRootOU
            {
                get
                {
                    return this.userGroupsRootOUField;
                }
                set
                {
                    this.userGroupsRootOUField = value;
                }
            }
            public string ShortDomainName
            {
                get
                {
                    return this.shortDomainName;
                }
                set
                {
                    this.shortDomainName = value;
                }
            }

            /// <remarks/>
            public string StaffRootOU
            {
                get
                {
                    return this.staffRootOUField;
                }
                set
                {
                    this.staffRootOUField = value;
                }
            }

            /// <remarks/>
            public string StudentRootOU
            {
                get
                {
                    return this.studentRootOUField;
                }
                set
                {
                    this.studentRootOUField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class ConfigMISExports
        {

            private string reportRunnerPathField;

            private string staffReportPathField;

            private string studentReportPathField;

            private bool deleteReportsAfterProcessingField;

            /// <remarks/>
            public string ReportRunnerPath
            {
                get
                {
                    return this.reportRunnerPathField;
                }
                set
                {
                    this.reportRunnerPathField = value;
                }
            }

            /// <remarks/>
            public string StaffReportPath
            {
                get
                {
                    return this.staffReportPathField;
                }
                set
                {
                    this.staffReportPathField = value;
                }
            }

            /// <remarks/>
            public string StudentReportPath
            {
                get
                {
                    return this.studentReportPathField;
                }
                set
                {
                    this.studentReportPathField = value;
                }
            }

            /// <remarks/>
            public bool DeleteReportsAfterProcessing
            {
                get
                {
                    return this.deleteReportsAfterProcessingField;
                }
                set
                {
                    this.deleteReportsAfterProcessingField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class ConfigAccountProvisioning
        {

            private string staffSettingsPathField;

            private string studentSettingsPathField;

            /// <remarks/>
            public string StaffSettingsPath
            {
                get
                {
                    return this.staffSettingsPathField;
                }
                set
                {
                    this.staffSettingsPathField = value;
                }
            }

            /// <remarks/>
            public string StudentSettingsPath
            {
                get
                {
                    return this.studentSettingsPathField;
                }
                set
                {
                    this.studentSettingsPathField = value;
                }
            }

            /// <remarks/>
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class ConfigReporting
        {

            private ConfigReportingEmailAlerts emailAlertsField;

            /// <remarks/>
            public ConfigReportingEmailAlerts EmailAlerts
            {
                get
                {
                    return this.emailAlertsField;
                }
                set
                {
                    this.emailAlertsField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class ConfigReportingEmailAlerts
        {

            private bool enabledField;

            private string gmailAppKeyField;

            private string sendFromAddressField;

            private string[] sendToField;

            /// <remarks/>
            public bool Enabled
            {
                get
                {
                    return this.enabledField;
                }
                set
                {
                    this.enabledField = value;
                }
            }

            /// <remarks/>
            public string GmailAppKey
            {
                get
                {
                    return this.gmailAppKeyField;
                }
                set
                {
                    this.gmailAppKeyField = value;
                }
            }

            /// <remarks/>
            public string SendFromAddress
            {
                get
                {
                    return this.sendFromAddressField;
                }
                set
                {
                    this.sendFromAddressField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("Email", IsNullable = false)]
            public string[] SendTo
            {
                get
                {
                    return this.sendToField;
                }
                set
                {
                    this.sendToField = value;
                }
            }
        }


    }
}
