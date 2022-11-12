using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattMIS_Sync
{
    public class StudentModel
    {
        
        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class SuperStarReport
        {

            private StudentMISRecord[] recordField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Record")]
            public StudentMISRecord[] Record
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
        public partial class StudentMISRecord
        {

            private ushort primary_idField;

            private ushort idField;

            private string forenameField;

            private string surnameField;

            private byte yeartaughtinCodeField;

            private string regField;

            private string nameField;

            private System.DateTime startdateField;

            private string yearField;

            private string descriptionField;

            private ushort yearofentryField;

            private string fullNameField;

            /// <remarks/>
            public ushort primary_id
            {
                get
                {
                    return this.primary_idField;
                }
                set
                {
                    this.primary_idField = value;
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
            public string Forename
            {
                get
                {
                    return this.forenameField;
                }
                set
                {
                    this.forenameField = value;
                }
            }

            /// <remarks/>
            public string Surname
            {
                get
                {
                    return this.surnameField;
                }
                set
                {
                    this.surnameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Year taught in Code")]
            public byte YeartaughtinCode
            {
                get
                {
                    return this.yeartaughtinCodeField;
                }
                set
                {
                    this.yeartaughtinCodeField = value;
                }
            }

            /// <remarks/>
            public string Reg
            {
                get
                {
                    return this.regField;
                }
                set
                {
                    this.regField = value;
                }
            }

            /// <remarks/>
            public string Name
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

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Start date")]
            public System.DateTime Startdate
            {
                get
                {
                    return this.startdateField;
                }
                set
                {
                    this.startdateField = value;
                }
            }

            /// <remarks/>
            public string Year
            {
                get
                {
                    return this.yearField;
                }
                set
                {
                    this.yearField = value;
                }
            }

            /// <remarks/>
            public string Description
            {
                get
                {
                    return this.descriptionField;
                }
                set
                {
                    this.descriptionField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Year of entry")]
            public ushort Yearofentry
            {
                get
                {
                    return this.yearofentryField;
                }
                set
                {
                    this.yearofentryField = value;
                }
            }

            /// <remarks/>
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
        }





    }
}
