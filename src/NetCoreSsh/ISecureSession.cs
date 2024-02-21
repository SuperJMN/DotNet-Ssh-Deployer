using System;
using Renci.SshNet;

namespace DotNetSsh
{
    public interface ISecureSession : IDisposable
    {
        SftpClient Sftp { get; }
        SshClient Ssh { get; }
    }
}