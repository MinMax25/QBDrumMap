namespace QBDrumMap.Cubase.Score.Instruments;

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class kScoreLibrary
{
    private kScoreLibraryInstruments instrumentsField;

    public kScoreLibraryInstruments instruments
    {
        get
        {
            return instrumentsField;
        }
        set
        {
            instrumentsField = value;
        }
    }
}

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryInstruments
{
    private kScoreLibraryInstrumentsEntities entitiesField;

    public kScoreLibraryInstrumentsEntities entities
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
public partial class kScoreLibraryInstrumentsEntities
{
    private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinition[] instrumentEntityDefinitionField;

    private bool arrayField;

    [System.Xml.Serialization.XmlElementAttribute("InstrumentEntityDefinition")]
    public kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinition[] InstrumentEntityDefinition
    {
        get
        {
            return instrumentEntityDefinitionField;
        }
        set
        {
            instrumentEntityDefinitionField = value;
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
public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinition
{
    private string nameField;

    private string entityIDField;

    private string parentEntityIDField;

    private string aliasOfEntityIDField;

    private byte inheritanceMaskField;

    private string nameIDField;

    private byte numStavesField;

    private string musicXMLSoundIDField;

    private string numberingStyleField;

    private bool showGuitarChordsField;

    private bool showChordSymbolsField;

    private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStaveDefinition staveDefinitionField;

    private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefs clefsField;

    private string percussionKitDefinitionIDField;

    private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPitchedInstrumentData pitchedInstrumentDataField;

    private string percussionInstrumentDataIDField;

    private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPlayingTechniques playingTechniquesField;

    private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentData stringedInstrumentDataField;

    private string frettedInstrumentDataIDField;

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

    public string parentEntityID
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

    public string aliasOfEntityID
    {
        get
        {
            return aliasOfEntityIDField;
        }
        set
        {
            aliasOfEntityIDField = value;
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

    public string nameID
    {
        get
        {
            return nameIDField;
        }
        set
        {
            nameIDField = value;
        }
    }

    public byte numStaves
    {
        get
        {
            return numStavesField;
        }
        set
        {
            numStavesField = value;
        }
    }

    public string musicXMLSoundID
    {
        get
        {
            return musicXMLSoundIDField;
        }
        set
        {
            musicXMLSoundIDField = value;
        }
    }

    public string numberingStyle
    {
        get
        {
            return numberingStyleField;
        }
        set
        {
            numberingStyleField = value;
        }
    }

    public bool showGuitarChords
    {
        get
        {
            return showGuitarChordsField;
        }
        set
        {
            showGuitarChordsField = value;
        }
    }

    public bool showChordSymbols
    {
        get
        {
            return showChordSymbolsField;
        }
        set
        {
            showChordSymbolsField = value;
        }
    }

    public kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStaveDefinition staveDefinition
    {
        get
        {
            return staveDefinitionField;
        }
        set
        {
            staveDefinitionField = value;
        }
    }

    public kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefs clefs
    {
        get
        {
            return clefsField;
        }
        set
        {
            clefsField = value;
        }
    }

    public string percussionKitDefinitionID
    {
        get
        {
            return percussionKitDefinitionIDField;
        }
        set
        {
            percussionKitDefinitionIDField = value;
        }
    }

    public kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPitchedInstrumentData pitchedInstrumentData
    {
        get
        {
            return pitchedInstrumentDataField;
        }
        set
        {
            pitchedInstrumentDataField = value;
        }
    }

    public string percussionInstrumentDataID
    {
        get
        {
            return percussionInstrumentDataIDField;
        }
        set
        {
            percussionInstrumentDataIDField = value;
        }
    }

    public kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPlayingTechniques playingTechniques
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

    public kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentData stringedInstrumentData
    {
        get
        {
            return stringedInstrumentDataField;
        }
        set
        {
            stringedInstrumentDataField = value;
        }
    }

    public string frettedInstrumentDataID
    {
        get
        {
            return frettedInstrumentDataIDField;
        }
        set
        {
            frettedInstrumentDataIDField = value;
        }
    }
}

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStaveDefinition
{
    private byte numStaveLinesField;

    private byte barlineSpanField;

    private byte bracketSpanField;

    private bool bracketSpanFieldSpecified;

    private bool useBraceField;

    private bool isVocalStaveField;

    private bool defaultContextualStemDirectionIsUpField;

    public byte numStaveLines
    {
        get
        {
            return numStaveLinesField;
        }
        set
        {
            numStaveLinesField = value;
        }
    }

    public byte barlineSpan
    {
        get
        {
            return barlineSpanField;
        }
        set
        {
            barlineSpanField = value;
        }
    }

    public byte bracketSpan
    {
        get
        {
            return bracketSpanField;
        }
        set
        {
            bracketSpanField = value;
        }
    }

    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool bracketSpanSpecified
    {
        get
        {
            return bracketSpanFieldSpecified;
        }
        set
        {
            bracketSpanFieldSpecified = value;
        }
    }

    public bool useBrace
    {
        get
        {
            return useBraceField;
        }
        set
        {
            useBraceField = value;
        }
    }

    public bool isVocalStave
    {
        get
        {
            return isVocalStaveField;
        }
        set
        {
            isVocalStaveField = value;
        }
    }

    public bool defaultContextualStemDirectionIsUp
    {
        get
        {
            return defaultContextualStemDirectionIsUpField;
        }
        set
        {
            defaultContextualStemDirectionIsUpField = value;
        }
    }
}

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefs
{
    private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefsClefIDsForEachStave clefIDsForEachStaveField;

    public kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefsClefIDsForEachStave clefIDsForEachStave
    {
        get
        {
            return clefIDsForEachStaveField;
        }
        set
        {
            clefIDsForEachStaveField = value;
        }
    }
}

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefsClefIDsForEachStave
{
    private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefsClefIDsForEachStaveStaveClefIDs[] staveClefIDsField;

    private bool arrayField;

    [System.Xml.Serialization.XmlElementAttribute("staveClefIDs")]
    public kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefsClefIDsForEachStaveStaveClefIDs[] staveClefIDs
    {
        get
        {
            return staveClefIDsField;
        }
        set
        {
            staveClefIDsField = value;
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
public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefsClefIDsForEachStaveStaveClefIDs
{
    private string idForTransposingLayoutsField;

    private string idForNonTransposingLayoutsField;

    public string idForTransposingLayouts
    {
        get
        {
            return idForTransposingLayoutsField;
        }
        set
        {
            idForTransposingLayoutsField = value;
        }
    }

    public string idForNonTransposingLayouts
    {
        get
        {
            return idForNonTransposingLayoutsField;
        }
        set
        {
            idForNonTransposingLayoutsField = value;
        }
    }
}

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPitchedInstrumentData
{
    private string standardRangeField;

    private string advancedRangeField;

    private sbyte concertOctaveTranspositionField;

    private sbyte transposedChromaticTranspositionField;

    private sbyte transposedDiatonicTranspositionField;

    private bool useKeySignaturesField;

    private string showTranspositionField;

    private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPitchedInstrumentDataStringedInstrumentStringLengthAndSpacings stringedInstrumentStringLengthAndSpacingsField;

    private string stringedInstrumentHoldField;

    public string standardRange
    {
        get
        {
            return standardRangeField;
        }
        set
        {
            standardRangeField = value;
        }
    }

    public string advancedRange
    {
        get
        {
            return advancedRangeField;
        }
        set
        {
            advancedRangeField = value;
        }
    }

    public sbyte concertOctaveTransposition
    {
        get
        {
            return concertOctaveTranspositionField;
        }
        set
        {
            concertOctaveTranspositionField = value;
        }
    }

    public sbyte transposedChromaticTransposition
    {
        get
        {
            return transposedChromaticTranspositionField;
        }
        set
        {
            transposedChromaticTranspositionField = value;
        }
    }

    public sbyte transposedDiatonicTransposition
    {
        get
        {
            return transposedDiatonicTranspositionField;
        }
        set
        {
            transposedDiatonicTranspositionField = value;
        }
    }

    public bool useKeySignatures
    {
        get
        {
            return useKeySignaturesField;
        }
        set
        {
            useKeySignaturesField = value;
        }
    }

    public string showTransposition
    {
        get
        {
            return showTranspositionField;
        }
        set
        {
            showTranspositionField = value;
        }
    }

    public kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPitchedInstrumentDataStringedInstrumentStringLengthAndSpacings stringedInstrumentStringLengthAndSpacings
    {
        get
        {
            return stringedInstrumentStringLengthAndSpacingsField;
        }
        set
        {
            stringedInstrumentStringLengthAndSpacingsField = value;
        }
    }

    public string stringedInstrumentHold
    {
        get
        {
            return stringedInstrumentHoldField;
        }
        set
        {
            stringedInstrumentHoldField = value;
        }
    }
}

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPitchedInstrumentDataStringedInstrumentStringLengthAndSpacings
{
    private decimal scaleLengthField;

    private decimal interCourseStringSpacingAtNutField;

    private decimal interCourseStringSpacingAtSaddleField;

    public decimal scaleLength
    {
        get
        {
            return scaleLengthField;
        }
        set
        {
            scaleLengthField = value;
        }
    }

    public decimal interCourseStringSpacingAtNut
    {
        get
        {
            return interCourseStringSpacingAtNutField;
        }
        set
        {
            interCourseStringSpacingAtNutField = value;
        }
    }

    public decimal interCourseStringSpacingAtSaddle
    {
        get
        {
            return interCourseStringSpacingAtSaddleField;
        }
        set
        {
            interCourseStringSpacingAtSaddleField = value;
        }
    }
}

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPlayingTechniques
{
    private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPlayingTechniquesPlayingTechnique[] playingTechniqueField;

    private bool arrayField;

    [System.Xml.Serialization.XmlElementAttribute("playingTechnique")]
    public kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPlayingTechniquesPlayingTechnique[] playingTechnique
    {
        get
        {
            return playingTechniqueField;
        }
        set
        {
            playingTechniqueField = value;
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
public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPlayingTechniquesPlayingTechnique
{
    private string techniqueIDField;

    private bool naturalEquivalentField;

    private bool naturalEquivalentFieldSpecified;

    public string techniqueID
    {
        get
        {
            return techniqueIDField;
        }
        set
        {
            techniqueIDField = value;
        }
    }

    public bool naturalEquivalent
    {
        get
        {
            return naturalEquivalentField;
        }
        set
        {
            naturalEquivalentField = value;
        }
    }

    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool naturalEquivalentSpecified
    {
        get
        {
            return naturalEquivalentFieldSpecified;
        }
        set
        {
            naturalEquivalentFieldSpecified = value;
        }
    }
}

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentData
{
    private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentDataStringPitches stringPitchesField;

    public kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentDataStringPitches stringPitches
    {
        get
        {
            return stringPitchesField;
        }
        set
        {
            stringPitchesField = value;
        }
    }
}

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentDataStringPitches
{
    private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentDataStringPitchesStringPitch[] stringPitchField;

    private bool arrayField;

    [System.Xml.Serialization.XmlElementAttribute("stringPitch")]
    public kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentDataStringPitchesStringPitch[] stringPitch
    {
        get
        {
            return stringPitchField;
        }
        set
        {
            stringPitchField = value;
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
public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentDataStringPitchesStringPitch
{
    private string pitchWithSpellingField;

    public string pitchWithSpelling
    {
        get
        {
            return pitchWithSpellingField;
        }
        set
        {
            pitchWithSpellingField = value;
        }
    }
}