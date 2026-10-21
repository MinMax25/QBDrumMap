namespace QBDrumMap.Cubase.Score.Technique;

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class kScoreLibrary
{
    private kScoreLibraryPlayingTechniques playingTechniquesField;

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

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryPlayingTechniques
{
    private kScoreLibraryPlayingTechniquesEntities entitiesField;

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

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryPlayingTechniquesEntities
{
    private kScoreLibraryPlayingTechniquesEntitiesPlayingTechniqueDefinition[] playingTechniqueDefinitionField;

    private bool arrayField;

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