namespace QBDrumMap.Class.Services;

/// <summary>未保存の変更があるときの利用者の選択。</summary>
public enum UnsavedChoice
{
    Save,
    DontSave,
    Cancel,
}

/// <summary>New/Open/Close の前に、未保存の変更を「保存する／保存しない／キャンセル」で確認する。</summary>
public static class UnsavedChangesGuard
{
    /// <summary>続行してよい場合だけtrue。保存を選んで保存に失敗または中止した場合、およびキャンセルはfalse。</summary>
    public static async Task<bool> ConfirmAsync(Func<bool> isModified, Func<Task<UnsavedChoice>> ask, Func<Task<bool>> save)
    {
        if (!isModified())
        {
            return true;
        }

        switch (await ask())
        {
            case UnsavedChoice.Save:
                return await save();
            case UnsavedChoice.DontSave:
                return true;
            default:
                return false;
        }
    }
}
