using Core.Interfaces;

namespace Core.BitmapImage
{
    public class BmpSurgery : SurgeryBase
    {
        private FileInfo fileInfo;
        public readonly long FreeBytes;


        public BmpSurgery(string path, int degree)
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