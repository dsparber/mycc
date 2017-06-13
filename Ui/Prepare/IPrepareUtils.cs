using System;
using System.Threading.Tasks;

namespace MyCC.Ui.Prepare
{
    public interface IPrepareUtils
    {
        bool PreparingNeeded { get; }
        Task Prepare(Action<(double progress, string infoText)> onProgress);
    }
}