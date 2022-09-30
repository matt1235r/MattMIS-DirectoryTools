using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattMIS_Sync
{
    public class StaffRoleModel
    {
        

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class StaffProvisoning
        {

            private string expiresOnField;

            private StaffProvisoningSettings settingsField;

            public StaffRoleRecord[] Roles;

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
            public StaffProvisoningSettings Settings
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
            [System.Xml.Serialization.XmlArrayItemAttribute("Role", IsNullable = false)]
            public StaffRoleRecord[] staffRoleRecord
            {
                get
                {
                    return this.Roles;
                }
                set
                {
                    this.Roles = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class StaffProvisoningSettings
        {

            private bool useFirstNameInitialField;

            private string defaultPasswordField;

            private string validateWorkEmailEndsWith;

            private string fallbackStaffRoleName;
           

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

            /// <remarks/>
            public string FallbackStaffRoleName
            {
                get
                {
                    return this.fallbackStaffRoleName;
                }
                set
                {
                    this.fallbackStaffRoleName = value;
                }
            }

            /// <remarks/>
            public string ValidateWorkEmailEndsWith
            {
                get
                {
                    return this.validateWorkEmailEndsWith;
                }
                set
                {
                    this.validateWorkEmailEndsWith = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class StaffRoleRecord
        {

            private string ouField;

            private string profilePathField;

            private string uPNField;

            private string[] groupsField;

            private string nameField;

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
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }
        }


    }
}
