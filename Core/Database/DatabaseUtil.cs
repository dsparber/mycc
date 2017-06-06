using System;
using SQLite;

namespace MyCC.Core.Database
{
    public static class DatabaseUtil
    {
        public static ISqLiteConnection SqLiteConnection { set; private get; }

        private static ISqLiteConnection ConnectionClass
            => SqLiteConnection ??
               throw new InvalidOperationException("SqLiteConnection not set up. Make sure you assigned DatabaseUtil.SqLiteConnection");

        public static SQLiteAsyncConnection OldConnection => ConnectionClass.GetOldConnection();
        public static SQLiteAsyncConnection Connection => ConnectionClass.Connection;
    }
}