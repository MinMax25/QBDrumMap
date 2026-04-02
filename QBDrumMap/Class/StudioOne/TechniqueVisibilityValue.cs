using QBDrumMap.Views.Controls;

namespace QBDrumMap.Class.StudioOne
{
    public class TechniqueVisibilityValue : IElementVisibility
    {
        #region Properties

        // 対応するStudio Oneの奏法列挙型
        public StudioOneTechnique Technique { get; set; }

        // オープン（丸）記号の表示フラグ
        public bool Circ { get; set; }

        // センタード・オープン（点付き丸）記号の表示フラグ
        public bool Ccir { get; set; }

        // クローズ（プラス）記号の表示フラグ
        public bool Plus { get; set; }

        // トレモロ（斜線1本）の表示フラグ
        public bool Trm1 { get; set; }

        // トレモロ（斜線2本）の表示フラグ
        public bool Trm2 { get; set; }

        // トレモロ（斜線3本）の表示フラグ
        public bool Trm3 { get; set; }

        // スタッカートの表示フラグ
        public bool Stac { get; set; }

        // 各フラグを配列として一括取得するプロパティ
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

        #endregion

        #region ctor

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

        #endregion
    }
}