namespace QBDrumMap.Class
{
    public static class ModelBaseExtension
    {
        public static T Clone<T>(this T target)
            where T : ModelBase
        {
            return (T)target.Clone();
        }
    }
}
