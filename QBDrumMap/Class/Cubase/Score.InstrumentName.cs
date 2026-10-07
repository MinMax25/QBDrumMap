namespace QBDrumMap.Cubase.Score.InstrumentName;

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class kScoreLibrary
{
    private kScoreLibraryInstrumentNames instrumentNamesField;

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

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryInstrumentNames
{
    private string languageField;

    private kScoreLibraryInstrumentNamesEntities entitiesField;

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

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryInstrumentNamesEntities
{
    private kScoreLibraryInstrumentNamesEntitiesInstrumentNameEntityDefinition[] instrumentNameEntityDefinitionField;

    private bool arrayField;

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
public partial class kScoreLibraryInstrumentNamesEntitiesInstrumentNameEntityDefinition
{
    private string entityIDField;

    private string nameField;

    private object parentEntityIDField;

    private byte inheritanceMaskField;

    private kScoreLibraryInstrumentNamesEntitiesInstrumentNameEntityDefinitionData dataField;

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