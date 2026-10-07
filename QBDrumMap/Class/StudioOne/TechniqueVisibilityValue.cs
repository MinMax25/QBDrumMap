using QBDrumMap.Views.Controls;

namespace QBDrumMap.Class.StudioOne;

public class TechniqueVisibilityValue : IElementVisibility
{
    public StudioOneTechnique Technique { get; set; }

    public bool Circ { get; set; }

    public bool Ccir { get; set; }

    public bool Plus { get; set; }

    public bool Trm1 { get; set; }

    public bool Trm2 { get; set; }

    public bool Trm3 { get; set; }

    public bool Stac { get; set; }

    public bool[] Values
    {
        get
        {
            return
            [
                Circ,
                Ccir,
                Plus,
                Trm1,
                Trm2,
                Trm3,
                Stac
            ];
        }
    }

    public TechniqueVisibilityValue(
        StudioOneTechnique technique,
        bool circ = false,
        bool ccir = false,
        bool plus = false,
        bool trm1 = false,
        bool trm2 = false,
        bool trm3 = false,
        bool stac = false)
    {
        Technique = technique;
        Circ = circ;
        Ccir = ccir;
        Plus = plus;
        Trm1 = trm1;
        Trm2 = trm2;
        Trm3 = trm3;
        Stac = stac;
    }
}