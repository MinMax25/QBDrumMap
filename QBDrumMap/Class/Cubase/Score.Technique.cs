namespace QBDrumMap.Cubase.Score.Technique
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class kScoreLibrary
    {

        private kScoreLibraryPlayingTechniques playingTechniquesField;

        /// <remarks/>
        public kScoreLibraryPlayingTechniques playingTechniques
        {
            get
            {
                return playingTechniquesField;
            }
            set
            {
                playingTechniquesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryPlayingTechniques
    {

        private kScoreLibraryPlayingTechniquesEntities entitiesField;

        /// <remarks/>
        public kScoreLibraryPlayingTechniquesEntities entities
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
    public partial class kScoreLibraryPlayingTechniquesEntities
    {

        private kScoreLibraryPlayingTechniquesEntitiesPlayingTechniqueDefinition[] playingTechniqueDefinitionField;

        private bool arrayField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PlayingTechniqueDefinition")]
        public kScoreLibraryPlayingTechniquesEntitiesPlayingTechniqueDefinition[] PlayingTechniqueDefinition
        {
            get
            {
                return playingTechniqueDefinitionField;
            }
            set
            {
                playingTechniqueDefinitionField = value;
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
    public partial class kScoreLibraryPlayingTechniquesEntitiesPlayingTechniqueDefinition
    {

        private string nameField;

        private string entityIDField;

        private object parentEntityIDField;

        private byte inheritanceMaskField;

        private object textField;

        private string articulationTypeField;

        private string groupTypeField;

        private string aliasForPlayingTechniqueIDField;

        private string fallbackPlayingTechniqueIDField;

        private string mutualExclusionGroupsField;

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
        public object text
        {
            get
            {
                return textField;
            }
            set
            {
                textField = value;
            }
        }

        /// <remarks/>
        public string articulationType
        {
            get
            {
                return articulationTypeField;
            }
            set
            {
                articulationTypeField = value;
            }
        }

        /// <remarks/>
        public string groupType
        {
            get
            {
                return groupTypeField;
            }
            set
            {
                groupTypeField = value;
            }
        }

        /// <remarks/>
        public string aliasForPlayingTechniqueID
        {
            get
            {
                return aliasForPlayingTechniqueIDField;
            }
            set
            {
                aliasForPlayingTechniqueIDField = value;
            }
        }

        /// <remarks/>
        public string fallbackPlayingTechniqueID
        {
            get
            {
                return fallbackPlayingTechniqueIDField;
            }
            set
            {
                fallbackPlayingTechniqueIDField = value;
            }
        }

        /// <remarks/>
        public string mutualExclusionGroups
        {
            get
            {
                return mutualExclusionGroupsField;
            }
            set
            {
                mutualExclusionGroupsField = value;
            }
        }
    }
}