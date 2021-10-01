namespace AsanaNet.Objects
{
    public class PageOffset : AsanaObject, IAsanaData
    {
        public string Offset { get; private set; }
        public string Path { get; private set; }
        public string Uri { get; private set; }

        public PageOffset(string offset, string path, string uri)
        {
            Offset = offset;
            Path = path;
            Uri = uri;
        }

        public bool IsObjectLocal => true;
    }
}