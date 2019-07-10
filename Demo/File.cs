namespace Demo
{
    public class File
    {
        public string Path { get; set; }

        public override string ToString()
        {
            var path = System.IO.Path.GetFileName(Path);

            if (string.IsNullOrEmpty(path))
            {
                path = Path.Replace("/", string.Empty).Replace("\\", string.Empty);
            }

            return path;
        }
    }
}
