using Renci.SshNet;

namespace DotNetSsh
{
    public class SecureSession : ISecureSession
    {
        public SecureSession(ConnectionInfo info)
        {
            Ssh = new SshClient(info);
            Sftp = new SftpClient(info);
            Ssh.Connect();
            Sftp.Connect();
        }

        public SftpClient Sftp { get; }

        public SshClient Ssh { get; }

        public void Dispose()
        {
            Sftp.Dispose();
            Ssh.Dispose();           
        }
    }
}