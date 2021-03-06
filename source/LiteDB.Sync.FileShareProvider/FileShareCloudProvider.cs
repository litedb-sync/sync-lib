﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LiteDB.Sync.Exceptions;

namespace LiteDB.Sync.FileShareProvider
{
    public class FileShareCloudProvider : ILiteSyncCloudProvider
    {
        public FileShareCloudProvider()
        {
            var id = Guid.NewGuid().ToString("N");
            this.DirectoryPath = $"%TEMP%\\{id}\\";
        }

        public string DirectoryPath { get; }

        private string InitFilePath => Path.Combine(this.DirectoryPath, "Init.sync");

        public Task<Stream> DownloadInitFile(CancellationToken ct)
        {
            this.EnsureDirectory();

            if (!File.Exists(this.InitFilePath))
            {
                return Task.FromResult((Stream)null);
            }

            return Task.FromResult(this.ReadFile(this.InitFilePath));
        }

        public async Task UploadInitFile(Stream contents)
        {
            this.EnsureDirectory();

            if (File.Exists(this.InitFilePath))
            {
                throw new LiteSyncConflictOccuredException();
            }

            await this.WriteFileAsync(this.InitFilePath, contents);
        }

        public Task<Stream> DownloadPatchFile(string id, CancellationToken ct)
        {
            this.EnsureDirectory();

            var fileName = this.GetPatchFilePath(id);

            if (!File.Exists(fileName))
            {
                return Task.FromResult((Stream)null);
            }

            return Task.FromResult(this.ReadFile(fileName));
        }

        public async Task UploadPatchFile(string id, Stream contents)
        {
            this.EnsureDirectory();

            var patchFilePath = this.GetPatchFilePath(id);

            if (File.Exists(patchFilePath))
            {
                throw new LiteSyncConflictOccuredException();
            }

            await this.WriteFileAsync(patchFilePath, contents);
        }

        public void Cleanup()
        {
            if (Directory.Exists(this.DirectoryPath))
            {
                Directory.Delete(this.DirectoryPath, true);
            }
        }

        private void EnsureDirectory()
        {
            if (!Directory.Exists(this.DirectoryPath))
            {
                Directory.CreateDirectory(this.DirectoryPath);
            }
        }

        private string GetPatchFilePath(string id)
        {
            var fileName = string.Format("{0}.patch", id);

            return Path.Combine(this.DirectoryPath, fileName);
        }

        private async Task WriteFileAsync(string path, Stream contents)
        {
            using (var fs = new FileStream(path, System.IO.FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await contents.CopyToAsync(fs);
            }
        }

        private Stream ReadFile(string path)
        {
            return new FileStream(path, System.IO.FileMode.Open, FileAccess.Read, FileShare.Write);
        }
    }
}