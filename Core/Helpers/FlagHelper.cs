namespace MyCC.Core.Helpers
{
    public static class FlagHelper
    {
        public static int AddFlags(this int flags, int flagsToSet) => flags | flagsToSet;

        public static int RemoveFlags(this int flags, int flagsToRemove) => flags & ~flagsToRemove;

        public static bool IsSet(this int flags, int testFlags) => (flags & testFlags) == testFlags;
    }
}