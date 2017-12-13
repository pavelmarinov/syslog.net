using SyslogServer.Core;
using SyslogServer.Core.Protocol;
using SyslogServer.Core.Model;
using SyslogServer.Core.Storage;
using SyslogServer.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer
{
    public class SyslogServer : IDisposable, IStateQuery
    {
        private readonly Socket socket;
        private const int BUFFER_SIZE = 1024;
        private const int SYSLOG_PORT = 514;
        private byte[] receiveBuffer;
        private Dictionary<int, string> facilities;
        private Severity minSeverity;
        
        private MessageQueue messageQueue;
        private ISyslogParser syslogParser;

        private bool disposed = false;

        public SyslogServer(int maxQueueLen,
                            int messagesToFlush,
                            int flushInterval,
                            Dictionary<int, string> facilities,
                            Severity minSeverity,
                            IEnumerable<IStorage> storageHandlers,
                            string parserMode)
        {
            this.facilities = facilities;
            this.messageQueue = new MessageQueue(maxQueueLen, messagesToFlush, flushInterval, storageHandlers);
            this.minSeverity = minSeverity;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.socket.Bind(new IPEndPoint(IPAddress.Any, SYSLOG_PORT));
            switch (parserMode.ToLower())
            {
                case "bsd":
                    this.syslogParser = new BSDSyslogParser(this.facilities);
                    break;
                case "syslog":
                    this.syslogParser = new SyslogProtocolParser(this.facilities);
                    break;
                default:
                    this.syslogParser = new SyslogParser(this.facilities);
                    break;
            }
            this.WaitForClient();
        }


        

        private void WaitForClient()
        {
            EndPoint anyEP = new IPEndPoint(IPAddress.Any, 0);
            this.receiveBuffer = new byte[BUFFER_SIZE];
            IAsyncResult ar = this.socket.BeginReceiveFrom(this.receiveBuffer, 0, BUFFER_SIZE, SocketFlags.None, ref anyEP, this.OnGotRequest, this.socket);
            if (ar.CompletedSynchronously)
                this.GotRequest(ar);
        }

        private void OnGotRequest(IAsyncResult ar)
        {
            if (!ar.CompletedSynchronously)
                this.GotRequest(ar);
        }

        private void GotRequest(IAsyncResult ar)
        {
            try
            {
                EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                int bytesCount = this.socket.EndReceiveFrom(ar, ref remoteEP);
                if (bytesCount <= 0)
                {//back to listening state
                    this.WaitForClient();
                    return;
                }
                //TODO: Encoding?
                string packet = System.Text.Encoding.UTF8.GetString(this.receiveBuffer, 0, bytesCount);
                string remoteIP = ((IPEndPoint)remoteEP).Address != null ? ((IPEndPoint)remoteEP).Address.ToString() : String.Empty;
                SyslogMessage msg; 
                if(this.syslogParser.TryParse(remoteIP, packet, out msg) && msg.Severity <= this.minSeverity)
                    this.messageQueue.Enqueue(msg);//add the message to the inmem queue
            }
            catch (Exception x)
            {
                Logger.Instance.Log(Severity.Error, "Unexpected error when connecting client. Details:{0}", x.Message);
            }
            //back to listening state
            this.WaitForClient();
        }

        public void SaveState()
        {
            this.messageQueue.SaveState();
        }

        #region state queries
        public List<Host> GetHosts()
        {
            return this.messageQueue.GetHosts();
        }

        public TagMessagesCollection GetTagMessages(string host, string tag)
        {
            return this.messageQueue.GetTagMessages(host, tag);
        }

        public Host GetHost(string hostname)
        {
            return this.messageQueue.GetHost(hostname);
        }

        #endregion


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            if (disposing)
                this.socket.Close();
            this.disposed = true;
        }
    }
}
