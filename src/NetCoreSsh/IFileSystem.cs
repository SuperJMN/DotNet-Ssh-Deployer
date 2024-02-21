﻿using System.Threading.Tasks;

namespace DotNetSsh
{
    public interface IFileSystem
    {
        Task Copy(string source, string destination);
        Task CopyDirectory(string source, string destination, string fileSearchPattern = null);
        Task DeleteDirectory(string path);
        bool DirectoryExists(string path);
        bool FileExists(string path);
        void CreateDirectory(string path);
        void EnsureDirectoryExists(string directoryPath);
        Task DeleteFile(string filePath);
    }
}