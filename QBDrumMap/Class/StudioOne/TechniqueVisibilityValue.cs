using QBDrumMap.Views.Controls;

namespace QBDrumMap.Class.StudioOne
{
    public class TechniqueVisibilityValue
    (
        StudioOneTechnique technique,
        bool circ = false,
        bool ccir = false,
        bool plus = false,
        bool trm1 = false,
        bool trm2 = false,
        bool trm3 = false,
        bool stac = false
    )
        : IElementVisibility
    {
        #region Properties

        public StudioOneTechnique Technique { get; set; } = technique;

        public bool Circ { get; set; } = circ;

        public bool Ccir { get; set; } = ccir;

        public bool Plus { get; set; } = plus;

        public bool Trm1 { get; set; } = trm1;

        public bool Trm2 { get; set; } = trm2;

        public bool Trm3 { get; set; } = trm3;

        public bool Stac { get; set; } = stac;

        public bool[] Values =>
        [
            Circ,
            Ccir,
            Plus,
            Trm1,
            Trm2,
            Trm3,
            Stac,
        ];

        #endregion
    }
}
