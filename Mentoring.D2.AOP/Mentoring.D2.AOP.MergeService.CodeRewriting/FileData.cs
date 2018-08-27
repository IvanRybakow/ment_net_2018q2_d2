namespace Mentoring.D2.AOP.MergeService
{
    public struct FileData
    {
        public FileData(string extension, string name, int number)
        {
            Extension = extension;
            Name = name;
            Number = number;
        }

        public string Name { get; }
        public int Number { get; }
        public string Extension { get; }

        public override bool Equals(object obj)
        {
            if (obj is FileData fd) return Name == fd.Name;

            return false;
        }

        public override int GetHashCode()
        {
            return !string.IsNullOrEmpty(Name) ? Name.GetHashCode() : base.GetHashCode();
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Name) ? Name : base.ToString();
        }
    }
}