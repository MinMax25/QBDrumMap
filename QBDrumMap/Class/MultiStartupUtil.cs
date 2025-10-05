namespace QBDrumMap.Class
{
    public static class MultiStartupUtil
    {
        public static void SingleStartupContext(string key, Action action)
        {
            using (var mutex = new Mutex(true, key, out bool createdNew))
            {
                if (!createdNew)
                {
                    return;
                }
                action?.Invoke();
                mutex.ReleaseMutex();
            }
        }
    }
}