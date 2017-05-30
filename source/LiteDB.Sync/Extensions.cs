﻿using System;
using System.Reflection;
using LiteDB.Sync.Contract;

namespace LiteDB.Sync
{
    using System.Collections.Generic;

    internal static class Extensions
    {
        internal static IEnumerable<BsonDocument> FindDirtyEntities(this ILiteCollection<BsonDocument> collection)
        {
            var query = Query.Not(Query.EQ(nameof(ILiteSyncEntity.RequiresSync), new BsonValue(true)));

            return collection.Find(query);
        }

        internal static bool IsSyncEntityType(this Type type)
        {
            return typeof(ILiteSyncEntity).IsAssignableFrom(type);
        }

        internal static Head GetSyncHead(this ILiteDatabase db)
        {
            return db.GetCollection<Head>(LiteSyncDatabase.SyncStateCollectionName).FindById(LiteSyncDatabase.LocalHeadId);
        }
    }
}