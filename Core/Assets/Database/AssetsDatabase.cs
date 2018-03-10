using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Assets.Models;
using MyCC.Core.Database;
using SQLite;

namespace MyCC.Core.Assets.Database
{
    internal static class AssetsDatabase
    {
        public static async Task<IEnumerable<AssetsSource>> Load()
        {
            var connection = await Connect();

            var dbSources = await connection.Table<AssetsSourceDbm>().ToListAsync();
            var dbAssets = await connection.Table<AssetDbm>().ToListAsync();

            return dbSources.Select(dbSource =>
            {
                var source = dbSource.ToAssetsSource();
                if (source.Type != AssetsSourceType.WithAmount)
                {
                    source.Assets = dbAssets.Where(dbAsset => dbSource.Id == source.Id)
                        .Select(dbAsset => dbAsset.ToAssets(source));
                }
                return source;
            }).ToList();
        }

        public static async Task Insert(AssetsSource source)
        {
            var connection = await Connect();
            
            var dbObject = new AssetsSourceDbm(source);
            await connection.InsertAsync(dbObject);
            source.Id = dbObject.Id;

            await connection.InsertAllAsync(source.Assets.Select(asset => new AssetDbm(asset)));
        }
        
        public static async Task Delete(AssetsSource source)
        {
            var connection = await Connect();
            
            var dbObject = new AssetsSourceDbm(source);
            await connection.DeleteAsync(dbObject);
            await Task.WhenAll(source.Assets.Select(asset => connection.DeleteAsync(new AssetDbm(asset))));
        }

        private static async Task<SQLiteAsyncConnection> Connect()
        {
            var connection = DatabaseUtil.Connection;
            await connection.CreateTableAsync<AssetsSourceDbm>();
            await connection.CreateTableAsync<AssetDbm>();
            return connection;
        }
    }
}