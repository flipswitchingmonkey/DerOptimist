using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerOptimist
{
    public class Location
    {
        public DriveInfo Info;
        public string FAIcon { get; set; }
        public string DisplayName
        {
            get
            {
                if (Info.DriveType == DriveType.Network)
                {
                    return $"{Info.Name} ({Helpers.GetUNCPath( Info.RootDirectory.FullName.Substring(0,2))})";
                }
                return Info.Name;
            }
        }

        public Location(DriveInfo di)
        {
            Info = di;
            if (Info.DriveType == DriveType.Network)
            {
                FAIcon = MediaIcons.Network;
            }
            else
            {
                FAIcon = MediaIcons.Drive;
            }
        }
    }
}
