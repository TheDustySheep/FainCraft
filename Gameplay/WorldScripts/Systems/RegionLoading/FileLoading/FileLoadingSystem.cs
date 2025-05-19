using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionFileWriter;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionSerialization;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading
{
    public class FileLoadingSystem : IFileLoadingSystem
    {
        private readonly IRegionSerializer _regionSerializer;
        private readonly string _folderPath;

        private readonly ConcurrentDictionary<SaveCoord, SemaphoreSlim> _fileLocks = new();

        public FileLoadingSystem(string saveFileName, IRegionSerializer regionSerializer)
        {
            _regionSerializer = regionSerializer;
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _folderPath = Path.Combine(appData, ".faincraft", "saves", saveFileName);
            Directory.CreateDirectory(_folderPath);
        }

        public async Task<RegionData?> LoadAsync(RegionCoord rCoord)
        {
            var saveCoord = (SaveCoord)rCoord;
            var sem = _fileLocks.GetOrAdd(saveCoord, _ => new SemaphoreSlim(1, 1));

            await sem.WaitAsync();
            try
            {
                LoadResult? result = await Task.Run(() =>
                {
                    var path = GetFilePath(saveCoord);

                    if (!File.Exists(path))
                        return (LoadResult?)null;

                    using var stream = new FileStream(
                        path,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        bufferSize: 4096);

                    return _regionSerializer.Load(new LoadRequest()
                    {
                        SaveCoord = saveCoord,
                        RegionCoords = [rCoord],
                        Stream = stream,
                    });
                });

                return result == null ? null : result?.Regions[rCoord];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error {rCoord}: {ex}");
                return null;
            }
            finally
            {
                sem.Release();
            }
        }

        public async Task<bool> SaveAsync(RegionCoord rCoord, RegionData region)
        {
            var saveCoord = (SaveCoord)rCoord;
            var sem = _fileLocks.GetOrAdd(saveCoord, _ => new SemaphoreSlim(1, 1));

            await sem.WaitAsync();
            try
            {
                SaveResult result = await Task.Run(() =>
                {
                    var path = GetFilePath(saveCoord);

                    using var stream = new FileStream(
                        path,
                        FileMode.OpenOrCreate,
                        FileAccess.ReadWrite,
                        FileShare.Read,
                        bufferSize: 4096);

                    return _regionSerializer.Save(new SaveRequest()
                    {
                        SaveCoord = saveCoord,
                        Regions = new() { [rCoord] = region },
                        Stream = stream,
                    });
                });

                return result.Regions[rCoord];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error {rCoord}: {ex}");
                return false;
            }
            finally
            {
                sem.Release();
            }
        }

        private string GetFilePath(SaveCoord saveCoord)
            => Path.Combine(_folderPath, $"region_{saveCoord.X}_{saveCoord.Z}.fcr");
    }
}
