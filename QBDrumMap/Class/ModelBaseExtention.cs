namespace QBDrumMap.Class
{
    public static class ModelBaseExtention
    {
        public static T Clone<T>(this T target)
            where T : ModelBase
        {
            return (T)target.Clone();
        }
    }
}
