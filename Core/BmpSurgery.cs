using Core.Interfaces;

namespace Core
{
    public class BmpSurgery : ISurgery
    {
        private FileInfo fileInfo;
        public readonly long FreeBytes;
        

        public BmpSurgery(string path)
        {
            fileInfo = new FileInfo(path);
            FreeBytes = fileInfo.Length;
        }

        public long GetFreeBytes()
        {
            var file = fileInfo.OpenRead();
            return 0;
        }

    }
}