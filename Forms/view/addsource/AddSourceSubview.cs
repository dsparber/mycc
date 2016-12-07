﻿using System;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.addsource
{

    public abstract class AddSourceSubview
    {
        public abstract TableSection InputSection { get; }
        public abstract bool Enabled { set; }
        public abstract string Description { get; }
        public abstract string DefaultName { get; }
        public Action NameChanged = () => { };
        public abstract void Unfocus();
    }
}
