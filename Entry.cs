using System;
using System.IO;

namespace FileManager
{
    class Entry : IComparable<Entry>
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public EntriesType Type { get; set; }
        public Entry(string path)
        {
            this.Name = System.IO.Path.GetFileName(path);
            this.Path = path;
            if (Directory.Exists(path))
            {
                this.Type = EntriesType.Directory;
            }
            if (File.Exists(path))
            {
                this.Type = EntriesType.File;
            }
        }
        
        public int CompareTo(Entry other)
        {
            if (this.Type == EntriesType.Directory && other.Type == EntriesType.Directory)
            {
                return string.Compare(this.Name, other.Name);
            }
            if (this.Type == EntriesType.File && other.Type == EntriesType.File)
            {
                return string.Compare(this.Name, other.Name);
            }
            if (this.Type == EntriesType.Directory && other.Type == EntriesType.File)
            {
                return -1;
            }
            if (this.Type == EntriesType.File && other.Type == EntriesType.Directory)
            {
                return 1;
            }
            return 0;
        }
    }
}
