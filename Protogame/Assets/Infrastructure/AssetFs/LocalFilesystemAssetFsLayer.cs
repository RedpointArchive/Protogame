﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Protogame
{
    public class LocalFilesystemAssetFsLayer : IAssetFsLayer, IDisposable
    {
        private readonly string _basePath;
        private readonly FileSystemWatcher _watcher;
        private readonly Dictionary<string, IAssetFsFile> _knownFiles;
        private readonly object _dictionaryLock = new object();
        private readonly List<string> _changedSinceLastUpdate;

        public LocalFilesystemAssetFsLayer(string basePath)
        {
            _basePath = basePath;
            _knownFiles = new Dictionary<string, IAssetFsFile>();
            _changedSinceLastUpdate = new List<string>();

            _watcher = new FileSystemWatcher(_basePath);
            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;

            _watcher.Created += _watcher_Created;
            _watcher.Changed += _watcher_Changed;
            _watcher.Renamed += _watcher_Renamed;
            _watcher.Deleted += _watcher_Deleted;

            _watcher.EnableRaisingEvents = true;

            lock (_dictionaryLock)
            {
                ScanDirectory(_basePath);
                _changedSinceLastUpdate.Clear();
            }
        }

        private bool AcceptAsset(string assetName)
        {
            if (string.IsNullOrWhiteSpace(assetName))
            {
                return false;
            }

            if (Enum.GetNames(typeof(TargetPlatform)).Any(x => assetName.StartsWith(x)))
            {
                return false;
            }

            return true;
        }

        private void ScanDirectory(string path)
        {
            var directory = new DirectoryInfo(path);
            foreach (var subdirectory in directory.GetDirectories())
            {
                ScanDirectory(subdirectory.FullName);
            }
            foreach (var file in directory.GetFiles())
            {
                var name = FilesystemToAssetName(file.FullName);
                if (AcceptAsset(name))
                {
                    _knownFiles.Add(name, new LocalAssetFsFile(name, file.FullName));
                }
            }
        }

        private string FilesystemToAssetName(string fullPath)
        {
            var directoryInfo = new DirectoryInfo(_basePath);

            var relativePath = fullPath.Substring(directoryInfo.FullName.Length + 1);
            if (relativePath.LastIndexOf('.') != -1)
            {
                relativePath = relativePath.Substring(0, relativePath.LastIndexOf('.'));
            }
            relativePath = relativePath.Replace(Path.DirectorySeparatorChar, '.');
            return relativePath;
        }

        private void _watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            lock (_dictionaryLock)
            {
                var name = FilesystemToAssetName(new FileInfo(e.FullPath).FullName);
                if (AcceptAsset(name))
                {
                    _knownFiles.Remove(name);
                    if (!_changedSinceLastUpdate.Contains(name))
                    {
                        _changedSinceLastUpdate.Add(name);
                    }
                }
            }
        }

        private void _watcher_Renamed(object sender, RenamedEventArgs e)
        {
            lock (_dictionaryLock)
            {
                var oldName = FilesystemToAssetName(new FileInfo(e.OldFullPath).FullName);
                if (AcceptAsset(oldName))
                {
                    _knownFiles.Remove(oldName);
                    if (!_changedSinceLastUpdate.Contains(oldName))
                    {
                        _changedSinceLastUpdate.Add(oldName);
                    }
                }
            }

            lock (_dictionaryLock)
            {
                var newName = FilesystemToAssetName(new FileInfo(e.FullPath).FullName);
                if (AcceptAsset(newName))
                {
                    _knownFiles.Add(newName, new LocalAssetFsFile(newName, new FileInfo(e.FullPath).FullName));
                    if (!_changedSinceLastUpdate.Contains(newName))
                    {
                        _changedSinceLastUpdate.Add(newName);
                    }
                }
            }
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (_dictionaryLock)
            {
                var name = FilesystemToAssetName(new FileInfo(e.FullPath).FullName);
                if (AcceptAsset(name))
                {
                    if (!_changedSinceLastUpdate.Contains(name))
                    {
                        _changedSinceLastUpdate.Add(name);
                    }
                }
            }
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            lock (_dictionaryLock)
            {
                var name = FilesystemToAssetName(new FileInfo(e.FullPath).FullName);
                if (AcceptAsset(name))
                {
                    _knownFiles.Add(name, new LocalAssetFsFile(name, new FileInfo(e.FullPath).FullName));
                    if (!_changedSinceLastUpdate.Contains(name))
                    {
                        _changedSinceLastUpdate.Add(name);
                    }
                }
            }
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }

        public async Task<IAssetFsFile> Get(string name)
        {
            lock (_dictionaryLock)
            {
                if (_knownFiles.ContainsKey(name))
                {
                    return _knownFiles[name];
                }
            }

            return null;
        }

        public async Task<IAssetFsFile[]> List()
        {
            lock (_dictionaryLock)
            {
                return _knownFiles.Values.ToArray();
            }
        }

        public void GetChangedSinceLastUpdate(ref List<string> names)
        {
            lock (_dictionaryLock)
            {
                names.AddRange(_changedSinceLastUpdate);
                _changedSinceLastUpdate.Clear();
            }
        }

        private class LocalAssetFsFile : IAssetFsFile
        {
            private readonly FileInfo _fileInfo;

            public LocalAssetFsFile(string assetName, string fullPath)
            {
                _fileInfo = new FileInfo(fullPath);
                ModificationTimeUtcTimestamp = new DateTimeOffset(_fileInfo.LastWriteTimeUtc, TimeSpan.Zero);
                Extension = _fileInfo.Extension.TrimStart('.');
                Name = assetName;
            }

            public DateTimeOffset ModificationTimeUtcTimestamp { get; }

            public string Name { get; }

            public string Extension { get; }

            public async Task<Stream> GetContentStream()
            {
                var memory = new MemoryStream();
                using (var file = new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await file.CopyToAsync(memory).ConfigureAwait(false);
                }
                memory.Seek(0, SeekOrigin.Begin);
                return memory;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
