using System;

namespace AutoHistory
{
    public class HistoryViewModel
    {
        public DateTime? DateTime { get; set; }
        public string State { get; set; }

        public string Device { get; set; }


        public string AgentIp { get; set; }
        public string AgentOs { get; set; }


        public string AgentBrowser { get; set; }
    }



}
