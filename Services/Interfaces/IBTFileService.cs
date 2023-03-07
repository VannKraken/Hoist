namespace Hoist.Services.Interfaces
{
    public interface IBTFileService
    {
        public Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file); //Type before method is its return type, type before parameter name is just its type

        public string ConvertByteArrayToFile(byte[] fileData, string extension, int defaultImage);

        public string GetFileIcon(int fileId);


        public string FormatFileSize(long bytes);

    }
}
