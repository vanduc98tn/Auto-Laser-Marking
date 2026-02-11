using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{

    public class FileQueueManager
    {
        private readonly string folderPath;
        private readonly string[] folderPathArr = new string[3];
        private readonly double maxSizeInGB;
        private readonly Queue<FileInfo> fileQueue = new Queue<FileInfo>();
        private readonly Queue<FileInfo>[] fileQueueArr = new Queue<FileInfo>[3];
        private long totalSizeInBytes = 0;
        private long[] totalSzInBytesArr = new long[3];
        private MyLogger logger = new MyLogger("Image Logger Manager");
        public FileQueueManager()
        {
            this.folderPath = "C:\\";
            this.maxSizeInGB = 1d;

            InitializeQueue();
        }
        public FileQueueManager(string folderPath, double maxSizeInGB)
        {

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            this.folderPath = folderPath;
            this.maxSizeInGB = maxSizeInGB;

            InitializeQueue();
        }
        public FileQueueManager(string[] folderPathArr, double maxSizeInGB)
        {
            for (int i = 0; i < this.folderPathArr.Length; i++)
            {
                if (!Directory.Exists(folderPathArr[i]))
                    Directory.CreateDirectory(folderPathArr[i]);
                this.folderPathArr[i] = folderPathArr[i];
            }
            this.maxSizeInGB = maxSizeInGB;
            InitializeMultiQueue();
        }

        private void InitializeQueue()
        {
            var dirInfo = new DirectoryInfo(folderPath);
            var files = dirInfo.GetFiles().OrderBy(f => f.LastWriteTime).ToList();

            foreach (var fi in files)
            {
                fileQueue.Enqueue(fi);
                totalSizeInBytes += fi.Length;
            }
        }
        private void InitializeMultiQueue()
        {
            for (int i = 0; i < this.folderPathArr.Length; i++)
            {
                var dirInfo = new DirectoryInfo(folderPathArr[i]);
                var files = dirInfo.GetFiles().OrderBy(f => f.LastWriteTime).ToList();
                fileQueueArr[i] = new Queue<FileInfo>();
                foreach (var fi in files)
                {
                    fileQueueArr[i].Enqueue(fi);
                    totalSzInBytesArr[i] += fi.Length;
                }
            }
        }

        public void AddFile(string filePath)
        {
            try
            {
                var fi = new FileInfo(filePath);
                if (!fi.Exists) return;

                fileQueue.Enqueue(fi);
                totalSizeInBytes += fi.Length;

                CleanupIfNeeded();
            }
            catch (Exception ex)
            {
                logger.Create("AddFile error: " + ex.Message, LogLevel.Error);
            }
        }
        public void AddFiles(string filePath)
        {
            try
            {
                var fi = new FileInfo(filePath);
                if (!fi.Exists) return;
                string folderName = new DirectoryInfo(Path.GetDirectoryName(filePath)).Name;
                switch (folderName)
                {
                    case "OK":
                        if (fileQueueArr[1] == null)
                            fileQueueArr[1] = new Queue<FileInfo>();
                        fileQueueArr[1].Enqueue(fi);
                        totalSzInBytesArr[1] += fi.Length;
                        CleanupIfNeeded(fileQueueArr[1], totalSzInBytesArr[1]);
                        break;
                    case "NG":
                        if (fileQueueArr[2] == null)
                            fileQueueArr[2] = new Queue<FileInfo>();
                        fileQueueArr[2].Enqueue(fi);
                        totalSzInBytesArr[2] += fi.Length;
                        CleanupIfNeeded(fileQueueArr[2], totalSzInBytesArr[2]);
                        break;
                    default:
                        if (fileQueueArr[0] == null)
                            fileQueueArr[0] = new Queue<FileInfo>();
                        fileQueueArr[0].Enqueue(fi);
                        totalSzInBytesArr[0] += fi.Length;
                        CleanupIfNeeded(fileQueueArr[0], totalSzInBytesArr[0]);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Create("AddFiles error: " + ex.Message, LogLevel.Error);
            }
        }

        private void CleanupIfNeeded()
        {
            double totalSizeInGB = totalSizeInBytes / (1024d * 1024 * 1024);

            while (totalSizeInGB >= maxSizeInGB && fileQueue.Count > 0)
            {
                var oldestFile = fileQueue.Dequeue();
                try
                {
                    if (oldestFile.Exists)
                    {
                        totalSizeInBytes -= oldestFile.Length;
                        oldestFile.Delete();
                    }
                }
                catch (Exception ex)
                {
                    logger.Create("Delete file error: " + ex.Message, LogLevel.Error);
                }

                totalSizeInGB = totalSizeInBytes / (1024d * 1024 * 1024);
            }
        }
        private void CleanupIfNeeded(Queue<FileInfo> fileQueue, long totalSizeInBytes)
        {
            double totalSizeInGB = totalSizeInBytes / (1024d * 1024 * 1024);

            while (totalSizeInGB >= maxSizeInGB && fileQueue.Count > 0)
            {
                var oldestFile = fileQueue.Dequeue();
                try
                {
                    if (oldestFile.Exists)
                    {
                        totalSizeInBytes -= oldestFile.Length;
                        oldestFile.Delete();
                    }
                }
                catch (Exception ex)
                {
                    logger.Create("Delete file error: " + ex.Message, LogLevel.Error);
                }

                totalSizeInGB = totalSizeInBytes / (1024d * 1024 * 1024);
            }
        }
    }
}
