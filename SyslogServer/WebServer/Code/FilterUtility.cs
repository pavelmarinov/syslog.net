using SyslogServer.Core.Protocol;
using SyslogServer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.WebServer.Code
{
    public static class FilterUtility
    {
        public static SearchModel ApplyFilters(SearchQuery query, TagMessagesCollection messages)
        {
            SearchModel searchModel = new SearchModel();
            //init messages
            searchModel.MessagesCollection = new TagMessagesCollection(messages.Host, messages.Tag);
            //init filters
            searchModel.FilterModel = new FilterModel()
            {
                Severity = !String.IsNullOrEmpty(query.Severity) ? query.Severity : String.Empty,
                Text = !String.IsNullOrEmpty(query.Text) ? query.Text : String.Empty,
                StartTime = query.StartTime,
                EndTime = query.EndTime,
                MinSeverity = !String.IsNullOrEmpty(query.MinSeverity) ? query.MinSeverity : String.Empty
            };
            FilterUtility.ApplyQueryFilters(searchModel, messages.Messages);
            //calculate page options
            searchModel.PageOptions = new PageOptions();
            //TODO: Hard-coded page size
            searchModel.PageOptions.PageCount = (searchModel.MessagesCollection.Messages.Count / 50) + 1;
            if (query.Page <= 0 || query.Page > searchModel.PageOptions.PageCount)
                searchModel.PageOptions.Page = 1;
            else
                searchModel.PageOptions.Page = query.Page;
            //filter by page
            searchModel.MessagesCollection.Messages = searchModel.MessagesCollection.Messages.Skip((searchModel.PageOptions.Page - 1) * 50).Take(50).ToList();
            return searchModel;
        }

        private static void ApplyQueryFilters(SearchModel searchModel, List<ISyslogMessage> allMessages)
        {
            bool severityPass = false, minSeverityPass = false;
            bool textPass = false, startTimePass = false, endTimePass = false;
            foreach (SyslogMessage msg in allMessages)
            {
                //apply severity filter
                if (String.IsNullOrEmpty(searchModel.FilterModel.Severity) ||
                   msg.Severity == (Severity)Enum.Parse(typeof(Severity), searchModel.FilterModel.Severity))
                    severityPass = true;
                //apply text filter
                if (String.IsNullOrEmpty(searchModel.FilterModel.Text) ||
                   msg.Content.ToLowerInvariant().Contains(searchModel.FilterModel.Text.ToLowerInvariant()))
                    textPass = true;
                //apply start time filter
                if (!searchModel.FilterModel.StartTime.HasValue ||
                    msg.Timestamp >= searchModel.FilterModel.StartTime.Value)
                    startTimePass = true;
                //apply end time filter
                if (!searchModel.FilterModel.EndTime.HasValue ||
                    msg.Timestamp <= searchModel.FilterModel.EndTime.Value)
                    endTimePass = true;
                //apply min severity filter
                if (String.IsNullOrEmpty(searchModel.FilterModel.MinSeverity) ||
                    msg.Severity <= (Severity)Enum.Parse(typeof(Severity), searchModel.FilterModel.MinSeverity))
                    minSeverityPass = true;
                //check filter markers
                if (severityPass && textPass && startTimePass && endTimePass && minSeverityPass)
                    searchModel.MessagesCollection.Messages.Add(msg);
                //reset filter markers
                severityPass = false;
                textPass = false;
                startTimePass = false;
                endTimePass = false;
                minSeverityPass = false;
            }
        }
    }
}
