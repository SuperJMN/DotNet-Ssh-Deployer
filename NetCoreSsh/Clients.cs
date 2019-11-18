using System;
using Renci.SshNet;

namespace NetCoreSsh
{
    internal class Clients : IDisposable
    {
        public Clients(SftpClient sftpClient, SshClient sshClient)
        {
            SftpClient = sftpClient;
            SshClient = sshClient;
        }

        public SftpClient SftpClient { get; }

        public SshClient SshClient { get; }


        public void Dispose()
        {
            SftpClient.Dispose();
            SshClient.Dispose();           
        }

        public void Connect()
        {
            SftpClient.Connect();
            SshClient.Connect();
        }
    }
}