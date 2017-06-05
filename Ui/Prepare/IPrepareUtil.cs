using System;
using System.Threading.Tasks;

namespace MyCC.Ui.Prepare
{
    public interface IPrepareUtil
    {
        bool PreparingNeeded { get; }
        Task Prepare(Action<(double progress, string infoText)> onProgress);
    }
}