using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Assets.Models;

namespace MyCC.Core.Assets
{
    public interface IAssets
    {
        Task Add(AssetsSource source);
        Task Update(AssetsSource source);
        Task Remove(AssetsSource source);
        
        IEnumerable<AssetsSource> Sources { get; }
    }
}