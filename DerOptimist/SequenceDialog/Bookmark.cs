using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerOptimist
{
    [Serializable]
    public class Bookmark
    {
        public string Name { get; set; }
        public DirectoryInfo Info { get; }
        public string FAIcon { get; set; }
        public string DisplayName
        {
            get
            {
                return $"{Info.Name} ({Info.FullName})";
            }
        }

        public Bookmark(string path)
        {
            if (Directory.Exists(path))
            {
                Info = new DirectoryInfo(path);
            }
            FAIcon = MediaIcons.Bookmark;
        }
    }
}
