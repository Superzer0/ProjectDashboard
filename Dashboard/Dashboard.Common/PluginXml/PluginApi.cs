using System.Xml.Serialization;

namespace Dashboard.Common.PluginXml
{
    public class PluginApi
    {
        
    }


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class plugin
    {

        private string nameField;

        private string pluginIdField;

        private string versionField;

        private string communicationTypeField;

        private string startingProgramField;

        private pluginMethod[] apiField;

        /// <remarks/>
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

        /// <remarks/>
        public string pluginId
        {
            get
            {
                return this.pluginIdField;
            }
            set
            {
                this.pluginIdField = value;
            }
        }

        /// <remarks/>
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        public string communicationType
        {
            get
            {
                return this.communicationTypeField;
            }
            set
            {
                this.communicationTypeField = value;
            }
        }

        /// <remarks/>
        public string startingProgram
        {
            get
            {
                return this.startingProgramField;
            }
            set
            {
                this.startingProgramField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("method", IsNullable = false)]
        public pluginMethod[] api
        {
            get
            {
                return this.apiField;
            }
            set
            {
                this.apiField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class pluginMethod
    {

        private string inputTypeField;

        private string outputTypeField;

        private string nameField;

        /// <remarks/>
        public string inputType
        {
            get
            {
                return this.inputTypeField;
            }
            set
            {
                this.inputTypeField = value;
            }
        }

        /// <remarks/>
        public string outputType
        {
            get
            {
                return this.outputTypeField;
            }
            set
            {
                this.outputTypeField = value;
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
