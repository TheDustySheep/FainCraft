using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionSerialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading
{
    public class FileLoadingSystem : IFileLoadingSystem
    {
        private readonly IRegionSerializer _regionSerializer;
        private readonly string _folderPath;

        // Completed load results
        private readonly ConcurrentQueue<(RegionCoord, RegionData?)> _completed = new();
        // Deduplication for load requests
        private readonly ConcurrentDictionary<RegionCoord, bool> _requested = new();
        // Pending region saves, grouped by SaveCoord
        private readonly ConcurrentDictionary<SaveCoord, ConcurrentDictionary<RegionCoord, RegionData>> _saveQueues = new();
        // One semaphore per SaveCoord
        private readonly ConcurrentDictionary<SaveCoord, SemaphoreSlim> _fileLocks = new();
        // One background save task per SaveCoord
        private readonly ConcurrentDictionary<SaveCoord, Task> _saveTasks = new();

        public FileLoadingSystem(string saveFileName, IRegionSerializer regionSerializer)
        {
            _regionSerializer = regionSerializer;
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _folderPath = Path.Combine(appData, ".faincraft", "saves", saveFileName);
            Directory.CreateDirectory(_folderPath);
        }

        public bool Request(RegionCoord coord)
        {
            if (!_requested.TryAdd(coord, true))
                return false;

            var saveCoord = (SaveCoord)coord;
            _ = Task.Run(async () =>
            {
                var sem = _fileLocks.GetOrAdd(saveCoord, _ => new SemaphoreSlim(1, 1));
                await sem.WaitAsync();
                try
                {
                    var data = await LoadRegionFromFileAsync(saveCoord, coord);
                    _completed.Enqueue((coord, data));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Load error {coord}: {ex}");
                    _completed.Enqueue((coord, null));
                }
                finally
                {
                    sem.Release();
                }
            });

            return true;
        }

        public void Save(RegionCoord coord, RegionData region)
        {
            var saveCoord = (SaveCoord)coord;
            var map = _saveQueues.GetOrAdd(saveCoord, _ => new ConcurrentDictionary<RegionCoord, RegionData>());
            map[coord] = region;
            StartSaveLoop(saveCoord);
        }

        public IEnumerable<(RegionCoord, RegionData?)> GetComplete()
        {
            while (_completed.TryDequeue(out var item))
            {
                _requested.TryRemove(item.Item1, out _);
                yield return item;
            }
        }

        //—— Helpers ——//

        private async Task<RegionData?> LoadRegionFromFileAsync(SaveCoord saveCoord, RegionCoord target)
        {
            var path = GetFilePath(saveCoord);
            if (!File.Exists(path)) return null;

            using var stream = new FileStream(path,
                                             FileMode.Open,
                                             FileAccess.ReadWrite,
                                             FileShare.Read,
                                             bufferSize: 4096,
                                             useAsync: true);
            await Task.Yield();
            if (_regionSerializer.Deserialize(stream, target, out var data))
                return data;
            return null;
        }

        private async Task AppendRegionsToFileAsync(SaveCoord saveCoord, IDictionary<RegionCoord, RegionData> toAppend)
        {
            var path = GetFilePath(saveCoord);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            using var stream = new FileStream(path,
                                             FileMode.OpenOrCreate,
                                             FileAccess.ReadWrite,
                                             FileShare.None,
                                             bufferSize: 4096,
                                             useAsync: true);
            // Seek to end to append
            stream.Seek(0, SeekOrigin.End);
            await Task.Yield();

            foreach (var kv in toAppend)
                _regionSerializer.Serialize(stream, kv.Key, kv.Value);

            await stream.FlushAsync();
        }

        private string GetFilePath(SaveCoord saveCoord)
            => Path.Combine(_folderPath, $"region_{saveCoord.X}_{saveCoord.Z}.fcr");

        private void StartSaveLoop(SaveCoord saveCoord)
        {
            if (_saveTasks.ContainsKey(saveCoord))
                return;

            var task = Task.Run(async () =>
            {
                var sem = _fileLocks.GetOrAdd(saveCoord, _ => new SemaphoreSlim(1, 1));

                while (true)
                {
                    if (!_saveQueues.TryRemove(saveCoord, out var pending) || pending.IsEmpty)
                        break;

                    await sem.WaitAsync();
                    try
                    {
                        // Append only the changed regions
                        await AppendRegionsToFileAsync(saveCoord, pending);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Save error {saveCoord}: {ex}");
                        break;
                    }
                    finally
                    {
                        sem.Release();
                    }

                    // Batch rapid saves
                    await Task.Delay(50);
                }

                _saveTasks.TryRemove(saveCoord, out _);
            });

            _saveTasks[saveCoord] = task;
        }
    }
}
