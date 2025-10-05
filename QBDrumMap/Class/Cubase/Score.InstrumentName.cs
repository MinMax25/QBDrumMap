namespace QBDrumMap.Cubase.Score.InstrumentName
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class kScoreLibrary
    {

        private kScoreLibraryInstrumentNames instrumentNamesField;

        /// <remarks/>
        public kScoreLibraryInstrumentNames instrumentNames
        {
            get
            {
                return instrumentNamesField;
            }
            set
            {
                instrumentNamesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryInstrumentNames
    {

        private string languageField;

        private kScoreLibraryInstrumentNamesEntities entitiesField;

        /// <remarks/>
        public string language
        {
            get
            {
                return languageField;
            }
            set
            {
                languageField = value;
            }
        }

        /// <remarks/>
        public kScoreLibraryInstrumentNamesEntities entities
        {
            get
            {
                return entitiesField;
            }
            set
            {
                entitiesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryInstrumentNamesEntities
    {

        private kScoreLibraryInstrumentNamesEntitiesInstrumentNameEntityDefinition[] instrumentNameEntityDefinitionField;

        private bool arrayField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("InstrumentNameEntityDefinition")]
        public kScoreLibraryInstrumentNamesEntitiesInstrumentNameEntityDefinition[] InstrumentNameEntityDefinition
        {
            get
            {
                return instrumentNameEntityDefinitionField;
            }
            set
            {
                instrumentNameEntityDefinitionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool array
        {
            get
            {
                return arrayField;
            }
            set
            {
                arrayField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryInstrumentNamesEntitiesInstrumentNameEntityDefinition
    {

        private string entityIDField;

        private string nameField;

        private object parentEntityIDField;

        private byte inheritanceMaskField;

        private kScoreLibraryInstrumentNamesEntitiesInstrumentNameEntityDefinitionData dataField;

        /// <remarks/>
        public string entityID
        {
            get
            {
                return entityIDField;
            }
            set
            {
                entityIDField = value;
            }
        }

        /// <remarks/>
        public string name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }

        /// <remarks/>
        public object parentEntityID
        {
            get
            {
                return parentEntityIDField;
            }
            set
            {
                parentEntityIDField = value;
            }
        }

        /// <remarks/>
        public byte inheritanceMask
        {
            get
            {
                return inheritanceMaskField;
            }
            set
            {
                inheritanceMaskField = value;
            }
        }

        /// <remarks/>
        public kScoreLibraryInstrumentNamesEntitiesInstrumentNameEntityDefinitionData data
        {
            get
            {
                return dataField;
            }
            set
            {
                dataField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryInstrumentNamesEntitiesInstrumentNameEntityDefinitionData
    {

        private string uiNameField;

        private string singularFullNameField;

        private string singularShortNameField;

        private string pluralFullNameField;

        private string pluralShortNameField;

        private string genderField;

        private string languageField;

        /// <remarks/>
        public string uiName
        {
            get
            {
                return uiNameField;
            }
            set
            {
                uiNameField = value;
            }
        }

        /// <remarks/>
        public string singularFullName
        {
            get
            {
                return singularFullNameField;
            }
            set
            {
                singularFullNameField = value;
            }
        }

        /// <remarks/>
        public string singularShortName
        {
            get
            {
                return singularShortNameField;
            }
            set
            {
                singularShortNameField = value;
            }
        }

        /// <remarks/>
        public string pluralFullName
        {
            get
            {
                return pluralFullNameField;
            }
            set
            {
                pluralFullNameField = value;
            }
        }

        /// <remarks/>
        public string pluralShortName
        {
            get
            {
                return pluralShortNameField;
            }
            set
            {
                pluralShortNameField = value;
            }
        }

        /// <remarks/>
        public string gender
        {
            get
            {
                return genderField;
            }
            set
            {
                genderField = value;
            }
        }

        /// <remarks/>
        public string language
        {
            get
            {
                return languageField;
            }
            set
            {
                languageField = value;
            }
        }
    }
}