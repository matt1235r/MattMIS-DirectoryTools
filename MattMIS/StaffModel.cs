using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattMIS_Sync
{
    public class StaffModel
    {

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class SuperStarReport
        {

            private StaffMISRecord[] recordField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Record")]
            public StaffMISRecord[] Record
            {
                get
                {
                    return this.recordField;
                }
                set
                {
                    this.recordField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class StaffMISRecord
        {

            private string multiple_idField;

            private ushort idField;

            private string preferredSurnameField;

            private string preferredForenameField;

            private string fullNameField;

            private string titleCodeField;

            private string staffCodeField;

            private string workEmailField;

            private System.DateTime employmentStartDateField;

            private string roleTextField;

            private string roleField;

            private System.DateTime startDateField;

            private bool startDateFieldSpecified;

            /// <remarks/>
            public string multiple_id
            {
                get
                {
                    return this.multiple_idField;
                }
                set
                {
                    this.multiple_idField = value;
                }
            }

            /// <remarks/>
            public ushort ID
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Preferred Surname")]
            public string PreferredSurname
            {
                get
                {
                    return this.preferredSurnameField;
                }
                set
                {
                    this.preferredSurnameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Preferred Forename")]
            public string PreferredForename
            {
                get
                {
                    return this.preferredForenameField;
                }
                set
                {
                    this.preferredForenameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Full Name")]
            public string FullName
            {
                get
                {
                    return this.fullNameField;
                }
                set
                {
                    this.fullNameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Title Code")]
            public string TitleCode
            {
                get
                {
                    return this.titleCodeField;
                }
                set
                {
                    this.titleCodeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Staff Code")]
            public string StaffCode
            {
                get
                {
                    return this.staffCodeField;
                }
                set
                {
                    this.staffCodeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Work Email")]
            public string WorkEmail
            {
                get
                {
                    return this.workEmailField;
                }
                set
                {
                    this.workEmailField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Employment Start Date")]
            public System.DateTime EmploymentStartDate
            {
                get
                {
                    return this.employmentStartDateField;
                }
                set
                {
                    this.employmentStartDateField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Role Text")]
            public string RoleText
            {
                get
                {
                    return this.roleTextField;
                }
                set
                {
                    this.roleTextField = value;
                }
            }

            /// <remarks/>
            public string Role
            {
                get
                {
                    return this.roleField;
                }
                set
                {
                    this.roleField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Start Date")]
            public System.DateTime StartDate
            {
                get
                {
                    return this.startDateField;
                }
                set
                {
                    this.startDateField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool StartDateSpecified
            {
                get
                {
                    return this.startDateFieldSpecified;
                }
                set
                {
                    this.startDateFieldSpecified = value;
                }
            }
        }





    }
}
