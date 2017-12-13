using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.WebServer.Code
{
    public class ResourceCache
    {
        private Dictionary<string, DateTime> lastModification =
            new Dictionary<string, DateTime>();
        private Dictionary<string, byte[]> resources =
            new Dictionary<string, byte[]>();


        private static readonly Lazy<ResourceCache> inst =
            new Lazy<ResourceCache>(() => new ResourceCache());

        private ResourceCache()
        {

        }

        static ResourceCache()
        { }

        public static ResourceCache Instance
        {
            get
            {
                return ResourceCache.inst.Value;
            }
        }

        public MemoryStream GetResource(string path)
        {
            if (File.Exists(path))
            {
                DateTime modifiedOn = File.GetLastWriteTime(path);
                DateTime lastDate;
                if(this.lastModification.TryGetValue(path, out lastDate) &&
                   lastDate < modifiedOn)
                {//expired
                    this.lastModification.Remove(path);
                    this.resources.Remove(path);
                }

                byte[] rsrc;
                if (this.resources.TryGetValue(path, out rsrc))
                    return new MemoryStream(rsrc);//from cache
                //caching the file
                rsrc = File.ReadAllBytes(path);
                this.lastModification[path] = modifiedOn;
                this.resources[path] = rsrc;
                return new MemoryStream(rsrc);

            }
            else
            {
                return null;
            }

        }
    }
}
