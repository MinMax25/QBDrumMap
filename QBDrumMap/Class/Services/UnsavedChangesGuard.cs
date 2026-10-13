namespace QBDrumMap.Class.Services;

public enum UnsavedChoice
{
    Save,
    DontSave,
    Cancel,
}

public static class UnsavedChangesGuard
{
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
