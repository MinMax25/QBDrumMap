using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.UndoRedo;
using QBDrumMap.Class.MapModels;

namespace QBDrumMap.Class;

public abstract partial class ModelBase : ObservableValidator, IHasDisplayOrder, ICloneable
{
    private int _displayOrder;

    protected IUndoManager UndoManager;

    [JsonIgnore]
    public int DisplayOrder
    {
        get
        {
            return _displayOrder;
        }
        set
        {
            SetProperty(ref _displayOrder, value);
        }
    }

    public void RegisterUndoManager(IUndoManager undoManager)
    {
        UndoManager = undoManager;
    }

    public void UnregisterUndoManager()
    {
        UndoManager = null;
    }

    /// <summary>検証エラーをすべて消す（読込結果に現在のデータとの重複エラーを残さないために使う）。</summary>
    public void ResetValidation()
    {
        ClearErrors();
    }

    public abstract object Clone();
}