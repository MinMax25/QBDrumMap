using System.Windows;
using QBDrumMap.Views.Controls;

namespace QBDrumMap.Class.StudioOne
{
    public static class StudioOneScore
    {
        #region Properties

        // 五線譜上の音符表示位置（ピッチ）と補助線の定義リスト
        public static List<ScoreNotePitch> ScoreNotePitches
        {
            get
            {
                return
                [
                    new(0, "", false, false, false, false, false, false),
                    new(93, "A5", true, true, true, true, false, false),
                    new(91, "G5", true, true, true, true, false, false),
                    new(89, "F5", false, true, true, true, false, false),
                    new(88, "E5", false, true, true, true, false, false),
                    new(86, "D5", false, false, true, true, false, false),
                    new(84, "C5", false, false, true, true, false, false),
                    new(83, "B4", false, false, false, true, false, false),
                    new(81, "A4", false, false, false, true, false, false),
                    new(79, "G4", false, false, false, false, false, false),
                    new(77, "F4", false, false, false, false, false, false),
                    new(76, "E4", false, false, false, false, false, false),
                    new(74, "D4", false, false, false, false, false, false),
                    new(72, "C4", false, false, false, false, false, false),
                    new(71, "B3", false, false, false, false, false, false),
                    new(69, "A3", false, false, false, false, false, false),
                    new(67, "G3", false, false, false, false, false, false),
                    new(65, "F3", false, false, false, false, false, false),
                    new(64, "E3", false, false, false, false, false, false),
                    new(62, "D3", false, false, false, false, false, false),
                    new(60, "C3", false, false, false, false, true, false),
                    new(59, "B2", false, false, false, false, true, true),
                    new(57, "A2", false, false, false, false, true, true),
                ];
            }
        }

        // ノートヘッドの種類に応じた描画要素の可視性定義
        public static readonly List<NoteHeadVisbilityValue> NoteHeadVisibilities =
        [
            new(StudioOneNoteHead.None, nhNo: true),
            new(StudioOneNoteHead.nhdX, nhdX: true),
            new(StudioOneNoteHead.nhTr, nhTr: true),
            new(StudioOneNoteHead.nhSl, nhNo: true, nhSl: true),
            new(StudioOneNoteHead.nhCX, nhdX: true, nhCX: true),
            new(StudioOneNoteHead.nhDi, nhDi: true),
        ];

        // テクニック（奏法記号）の種類に応じた描画要素の可視性定義
        public static readonly List<TechniqueVisibilityValue> TechniqueVisibilities =
        [
            new(StudioOneTechnique.None),
            new(StudioOneTechnique.circ, circ: true),
            new(StudioOneTechnique.ccir, circ: true, ccir: true),
            new(StudioOneTechnique.plus, ccir: true, plus: true),
            new(StudioOneTechnique.trm1, trm1: true),
            new(StudioOneTechnique.trm2, trm1: true, trm2: true),
            new(StudioOneTechnique.trm3, trm1: true, trm2: true, trm3: true),
            new(StudioOneTechnique.stac, stac: true),
        ];

        #endregion

        #region Methods

        #region General

        // UIElementの可視性を設定する拡張メソッド
        public static void SetVisibility(this UIElement element, bool isVisible)
        {
            if (element != null)
            {
                element.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            }
        }

        // 複数のUIElementに対して一括で可視性を設定する拡張メソッド
        public static void SetVisibility(this UIElement[] target, IElementVisibility values)
        {
            for (int i = 0; i < target.Length; i++)
            {
                target[i].SetVisibility(values.Values[i]);
            }
        }

        #endregion

        #endregion
    }
}