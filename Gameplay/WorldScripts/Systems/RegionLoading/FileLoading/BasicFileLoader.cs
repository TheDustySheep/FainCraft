using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionSerialization;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading
{
    public class BasicFileLoader : IFileLoadingSystem, IDisposable
    {
        private readonly string _folderPath;
        private readonly WorkerThread workerThread;
        private readonly FileQueue _queue = new();

        IRegionSerializer _regionSerializer;

        public BasicFileLoader(string saveFileName, IRegionSerializer regionSerializer)
        {
            // Save folder path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _folderPath = Path.Combine(appDataPath, ".faincraft", "saves", saveFileName);

            // Make sure the folder exists
            Directory.CreateDirectory(_folderPath);

            // Start the worker thread
            workerThread = new WorkerThread("Save File Loading Thread", ThreadFunction);
            _regionSerializer = regionSerializer;
        }

        ~BasicFileLoader()
        {
            workerThread.Dispose();
        }

        private void ThreadFunction()
        {
            // Handle saving files
            while (_queue.TryDequeueSaveRequest(out var coord, out var data))
            {
                string filePath = RegionFilePath(coord);
                using FileStream fs = new (filePath, FileMode.Create);

                _regionSerializer.Save(fs, coord, data);
                Console.WriteLine("Handling save request");
            }

            // Handle loading files
            while (_queue.TryDequeueLoadRequest(out var coord))
            {
                Console.WriteLine("Yep, found a request");

                string filePath = RegionFilePath(coord);
                using FileStream fs = new(filePath, FileMode.Open);

                _regionSerializer.Load(fs, coord, out var data);
                Console.WriteLine("Yep, deserialized");

                _queue.LoadResult(coord, data);
            }
        }

        public IEnumerable<(RegionCoord, RegionData?)> GetComplete()
        {
            while (_queue.TryDequeueLoadResult(out var coord, out var data))
            {
                yield return (coord, data);
            }
        }

        public bool Request(RegionCoord coord)
        {
            if (!File.Exists(RegionFilePath(coord)))
                return false;

            _queue.LoadRequest(coord);
            return true;
        }

        public void Save(RegionCoord coord, RegionData region)
        {
            _queue.SaveRequest(coord, region);
        }

        private string RegionFilePath(RegionCoord coord) => Path.Combine(_folderPath, $"r_X_{coord.X}_Z_{coord.Z}.fcr");

        public void Dispose()
        {
            workerThread.Dispose();
        }
    }
}
