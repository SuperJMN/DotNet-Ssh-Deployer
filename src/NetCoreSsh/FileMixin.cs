using System.IO;

namespace DotNetSsh
{
    public static class FileMixin
    {
        public static string ConvertToRelative(this DirectoryInfo root, FileSystemInfo child)
        {
            return child.FullName.Replace(root.FullName + Path.DirectorySeparatorChar, "");
        }

        public static string ToLinuxPath(this string str)
        {
            return str.Replace("\\", "/");
        }
    }
}