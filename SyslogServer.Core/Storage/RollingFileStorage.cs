using SyslogServer.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Storage
{
    public class RollingFileStorage : IStorage
    {
        private readonly string rootDir;

        public RollingFileStorage(string rootDir)
        {
            this.rootDir = rootDir;
            if (!Directory.Exists(rootDir))
                Directory.CreateDirectory(rootDir);
        }


        public void Save(IEnumerable<ISyslogMessage> messages)
        {
            Dictionary<string, List<ISyslogMessage>> separatedDict = this.SeparateByFile(messages);
            foreach (KeyValuePair<string, List<ISyslogMessage>> pair in separatedDict)
            {
                using(StreamWriter writer = new StreamWriter(pair.Key, append: true))
                {
                    foreach (SyslogMessage msg in pair.Value)
                        writer.WriteLine(msg.ToString());
                }
            }
        }

        //public void Save(IEnumerable<SyslogMessage> messages)
        //{
        //    string savePath;
        //    foreach (var msg in messages)
        //    {
        //        savePath = this.CalculateSavePath(msg);
        //        File.AppendAllText(savePath, msg.ToString() + Environment.NewLine);
        //    }
        //}

        private string CalculateSavePath(SyslogMessage msg)
        {
            string path = this.rootDir;
            string fileName = this.ClearIllegalPathChars(String.Concat((msg.Tag ?? "unknown"), "-", DateTime.Now.ToString("yyyyMMdd") + ".log"));
            if(!String.IsNullOrEmpty(msg.Hostname))
            {
                string clearedHostName = this.ClearIllegalPathChars(msg.Hostname);
                path = Path.Combine(path, clearedHostName);
                if (!Directory.Exists(Path.Combine(this.rootDir, clearedHostName)))
                    Directory.CreateDirectory(path);
            }
            return Path.Combine(path, fileName);
        }

        private string ClearIllegalPathChars(string path)
        {
            return string.Join("_", path.Split(Path.GetInvalidFileNameChars()));
        }
        
        private Dictionary<string, List<ISyslogMessage>> SeparateByFile(IEnumerable<ISyslogMessage> messages)
        {
            var msgDict = new Dictionary<string, List<ISyslogMessage>>();
            string currPath;
            foreach (SyslogMessage msg in messages)
            {
                currPath = this.CalculateSavePath(msg);
                if (!msgDict.ContainsKey(currPath))
                    msgDict[currPath] = new List<ISyslogMessage>();
                msgDict[currPath].Add(msg);
            }
            return msgDict;
        }
    }
}
