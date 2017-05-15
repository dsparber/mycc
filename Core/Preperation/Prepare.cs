using System.Threading.Tasks;
using MyCC.Core.Settings;

namespace MyCC.Core.Preperation
{
    public static class Prepare
    {
        public static bool PreparingNeeded => ApplicationSettings.FirstLaunch || ApplicationSettings.LastCoreVersion < new Version(1, 1);

        public static Task ExecutePreperations()
        {
            // Insert any preperation
            return new Task(() => { });
        }
    }
}