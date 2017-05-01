using System;

namespace MyCC.Ui.Helpers
{
    public interface IErrorDialog
    {
        void Display(Exception exception);
    }
}