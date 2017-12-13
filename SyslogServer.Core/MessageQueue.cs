using SyslogServer.Core.Model;
using SyslogServer.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyslogServer.Core
{
    //Thread-safe inmemory message queue
    public class MessageQueue : IDisposable
    {
        //TODO: Size the list on init ??
        List<ISyslogMessage> messageQueue = new List<ISyslogMessage>();
        private readonly ReaderWriterLockSlim rwLock =
            new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private bool disposed = false;

        //Maximum events in the queue
        private readonly int messageLimit;
        //# to flush when queue is full
        private readonly int flushNum;
        //flush interval in milliseconds
        private readonly int flushInterval;

        //flushing messages on separate thread
        private readonly System.Threading.Timer flushTimer;

        

        private IEnumerable<IStorage> storageHandlers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageLimit">Maximum events in the queue</param>
        /// <param name="flushNum"># to flush when queue is full</param>
        /// <param name="flushInterval">flush interval in minutes</param>
        public MessageQueue(int messageLimit, int flushNum, int flushInterval, IEnumerable<IStorage> storageHandlers)
        {
            this.messageLimit = messageLimit;
            this.flushNum = flushNum;
            this.storageHandlers = storageHandlers;
            this.flushInterval = flushInterval * 1000 * 60;
            this.flushTimer = new System.Threading.Timer(this.FlushTimerCallback, null, this.flushInterval, Timeout.Infinite);
        }

        public void Enqueue(ISyslogMessage message)
        {
            this.rwLock.EnterWriteLock();
            try
            {
                this.messageQueue.Add(message);
                if (this.messageQueue.Count > this.messageLimit)
                {
                    List<ISyslogMessage> messagesToSave = new List<ISyslogMessage>();
                    this.messageQueue.Sort((x, y) => y.Timestamp.CompareTo(x.Timestamp));
                    for (int i = 0; i < this.flushNum; i++)
                    {
                        if(this.messageQueue[this.messageQueue.Count - 1].IsDirty)
                            messagesToSave.Add(this.messageQueue[this.messageQueue.Count - 1]);
                        this.messageQueue.RemoveAt(this.messageQueue.Count - 1);
                    }
                    //store removed messages
                    foreach (IStorage handler in this.storageHandlers)
                        handler.Save(messagesToSave);
                }
            }
            finally
            {
                this.rwLock.ExitWriteLock();
            }
        }

        public void SaveState()
        {
            this.rwLock.EnterReadLock();
            try
            {
                foreach (IStorage handler in this.storageHandlers)
                    handler.Save(this.messageQueue);
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }

        public List<Host> GetHosts()
        {
            this.rwLock.EnterReadLock();
            try
            {
                List<Host> hosts = new List<Host>();
                Host currHost;
                Tag currTag;
                foreach (var msg in this.messageQueue)
                {
                    //check if host exists
                    currHost = hosts.Find(h => h.Name.Equals(msg.Hostname, StringComparison.CurrentCultureIgnoreCase));
                    if (currHost == null)
                    {
                        currHost = new Host(msg.Hostname);
                        hosts.Add(currHost);
                    }
                    //check if tag exists
                    currTag = currHost.Tags.Find(t => t.Name.Equals(msg.Tag, StringComparison.CurrentCultureIgnoreCase));
                    if (currTag == null)
                    {
                        currTag = new Tag(msg.Tag, msg.Facility);
                        currHost.Tags.Add(currTag);
                    }
                    currTag.AddMessage(msg.Severity);
                }
                return hosts;
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }

        public TagMessagesCollection GetTagMessages(string host, string tag)
        {
            this.rwLock.EnterReadLock();
            try
            {
                TagMessagesCollection tagMessages = new TagMessagesCollection(host, tag);
                foreach (var msg in this.messageQueue)
                {
                    if (msg.Hostname.Equals(host, StringComparison.CurrentCultureIgnoreCase) &&
                        msg.Tag.Equals(tag, StringComparison.CurrentCultureIgnoreCase))
                        tagMessages.Messages.Add(msg);
                }
                return tagMessages;
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }

        public Host GetHost(string hostname)
        {
            Host host = this.GetHosts().Where(h => h.Name.Equals(hostname, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
            if (host != null)
                return host;
            else
                return new Host(hostname);
        }

        public int Count()
        {
            return this.messageQueue.Count;
        }

        private void FlushTimerCallback(object state)
        {
            this.rwLock.EnterWriteLock();
            try
            {
                List<ISyslogMessage> messagesToSave = new List<ISyslogMessage>();
                this.messageQueue.Sort((x, y) => y.Timestamp.CompareTo(x.Timestamp));
                for (int i = this.messageQueue.Count - 1; i >= 0; i--)
                {
                    if (this.messageQueue[i].IsDirty)
                    {
                        messagesToSave.Add(this.messageQueue[i]);
                        this.messageQueue[i].IsDirty = false;
                    }
                    if (messagesToSave.Count > this.flushNum)
                        break;   
                }
                foreach (IStorage handler in this.storageHandlers)
                    handler.Save(this.messageQueue);
            }
            finally
            {
                this.rwLock.ExitWriteLock();
                this.flushTimer.Change(this.flushInterval, Timeout.Infinite);
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            if (disposing)
                this.rwLock.Dispose();
            this.disposed = true;
        }
    }
}
