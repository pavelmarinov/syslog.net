using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Model
{
    public class PageOptions
    {
        private int page;
        public int Page
        {
            get { return this.page; }
            set { this.page = value; }
        }

        private int pageCount;
        public int PageCount
        {
            get { return this.pageCount; }
            set { this.pageCount = value; }
        }


        public bool IsFirstPage
        {
            get
            {
                return this.page == 1 && this.pageCount > 1;
            }
        }

        public bool IsLastPage
        {
            get
            {
                return this.page == this.pageCount && this.pageCount > 1;
            }
        }

        public bool IsSinglePage
        {
            get
            {
                return this.page == this.pageCount && this.pageCount == 1;
            }
        }

    }
}
