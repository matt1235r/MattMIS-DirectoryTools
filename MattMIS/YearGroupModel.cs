using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattMIS_Sync
{
    public class YearGroupModel
    {

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class YearGroupProvisoning
        {

            private string expiresOnField;
            private StudentProvisoningSettings settingsField;

            private YearGroupRecord[] yearsField;

            /// <remarks/>
            public string ExpiresOn
            {
                get
                {
                    return this.expiresOnField;
                }
                set
                {
                    this.expiresOnField = value;
                }
            }

            /// <remarks/>
            public StudentProvisoningSettings Settings
            {
                get
                {
                    return this.settingsField;
                }
                set
                {
                    this.settingsField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("year", IsNullable = false)]
            public YearGroupRecord[] years
            {
                get
                {
                    return this.yearsField;
                }
                set
                {
                    this.yearsField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class StudentProvisoningSettings
        {

            private bool useFirstNameInitialField;

            private string defaultPasswordField;


            /// <remarks/>
            public bool UseFirstNameInitial
            {
                get
                {
                    return this.useFirstNameInitialField;
                }
                set
                {
                    this.useFirstNameInitialField = value;
                }
            }

            /// <remarks/>
            public string DefaultPassword
            {
                get
                {
                    return this.defaultPasswordField;
                }
                set
                {
                    this.defaultPasswordField = value;
                }
            }

        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class YearGroupRecord
        {

            private string ouField;

            private string profilePathField;

            private int shortStartYearCodeField;

            private string uPNField;

            private string[] groupsField;

            private int codeField;

            /// <remarks/>
            public string OU
            {
                get
                {
                    return this.ouField;
                }
                set
                {
                    this.ouField = value;
                }
            }

            /// <remarks/>
            public string ProfilePath
            {
                get
                {
                    return this.profilePathField;
                }
                set
                {
                    this.profilePathField = value;
                }
            }

            /// <remarks/>
            public int ShortStartYearCode
            {
                get
                {
                    return this.shortStartYearCodeField;
                }
                set
                {
                    this.shortStartYearCodeField = value;
                }
            }

            /// <remarks/>
            public string UPN
            {
                get
                {
                    return this.uPNField;
                }
                set
                {
                    this.uPNField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("group", IsNullable = false)]
            public string[] groups
            {
                get
                {
                    return this.groupsField;
                }
                set
                {
                    this.groupsField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public int code
            {
                get
                {
                    return this.codeField;
                }
                set
                {
                    this.codeField = value;
                }
            }
        }



    }
}
