using System.Reflection;
using libMidi.Messages.attributes;
using libMidi.Messages.enums;

namespace QBDrumMap.Class.Extentions
{
    public static class SysExHelper
    {
        public static byte[] GetExclusiveData(SysExType type)
        {
            byte[] data = [0xF0];

            var fieldInfo = typeof(SysExType).GetField(type.ToString());
            if (fieldInfo == null) return null;

            var attribute = fieldInfo.GetCustomAttribute<ExclusiveAttribute>();
            if (attribute?.Data == null) return null;

            return data.Concat(attribute.Data).ToArray();
        }
    }
}
