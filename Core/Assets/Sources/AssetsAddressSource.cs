using MyCC.Core.Assets.Models;

namespace MyCC.Core.Assets.Sources
{
    internal class AssetsAddressSource : AssetsSource
    {
        internal override AssetsSourceType Type => AssetsSourceType.WithAddress;
    }
}