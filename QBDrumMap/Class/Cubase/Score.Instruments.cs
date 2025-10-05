namespace QBDrumMap.Cubase.Score.Instruments
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class kScoreLibrary
    {

        private kScoreLibraryInstruments instrumentsField;

        /// <remarks/>
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryInstruments
    {

        private kScoreLibraryInstrumentsEntities entitiesField;

        /// <remarks/>
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryInstrumentsEntities
    {

        private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinition[] instrumentEntityDefinitionField;

        private bool arrayField;

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

    /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefs
    {

        private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefsClefIDsForEachStave clefIDsForEachStaveField;

        /// <remarks/>
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefsClefIDsForEachStave
    {

        private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefsClefIDsForEachStaveStaveClefIDs[] staveClefIDsField;

        private bool arrayField;

        /// <remarks/>
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
    public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionClefsClefIDsForEachStaveStaveClefIDs
    {

        private string idForTransposingLayoutsField;

        private string idForNonTransposingLayoutsField;

        /// <remarks/>
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

        /// <remarks/>
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

    /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPitchedInstrumentDataStringedInstrumentStringLengthAndSpacings
    {

        private decimal scaleLengthField;

        private decimal interCourseStringSpacingAtNutField;

        private decimal interCourseStringSpacingAtSaddleField;

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPlayingTechniques
    {

        private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPlayingTechniquesPlayingTechnique[] playingTechniqueField;

        private bool arrayField;

        /// <remarks/>
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
    public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionPlayingTechniquesPlayingTechnique
    {

        private string techniqueIDField;

        private bool naturalEquivalentField;

        private bool naturalEquivalentFieldSpecified;

        /// <remarks/>
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

        /// <remarks/>
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

        /// <remarks/>
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentData
    {

        private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentDataStringPitches stringPitchesField;

        /// <remarks/>
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentDataStringPitches
    {

        private kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentDataStringPitchesStringPitch[] stringPitchField;

        private bool arrayField;

        /// <remarks/>
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
    public partial class kScoreLibraryInstrumentsEntitiesInstrumentEntityDefinitionStringedInstrumentDataStringPitchesStringPitch
    {

        private string pitchWithSpellingField;

        /// <remarks/>
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
}