﻿using LiteDB.Sync.Contract;
using LiteDB.Sync.Internal;

namespace LiteDB.Sync
{
    public class LiteSyncConflict
    {
        public LiteSyncConflict(EntityChange localChange, EntityChange remoteChange)
        {
            this.LocalChange = localChange;
            this.RemoteChange = remoteChange;
            this.Resolution = ConflictResolution.None;
        }

        public GlobalEntityId EntityId => this.LocalChange.GlobalEntityId;

        public EntityChange LocalChange { get; }

        public EntityChange RemoteChange { get; }

        internal ConflictResolution Resolution { get; private set; }

        internal BsonDocument MergedEntity { get; private set; }

        public void ResolveKeepLocal()
        {
            this.Resolution = ConflictResolution.KeepLocal;
        }

        public void ResolveKeepRemote()
        {
            this.Resolution = ConflictResolution.KeepLocal;
        }

        public void ResolveMerged(BsonDocument mergedEntity)
        {
            this.Resolution = ConflictResolution.Merge;
            this.MergedEntity = mergedEntity;
        }
    }
}