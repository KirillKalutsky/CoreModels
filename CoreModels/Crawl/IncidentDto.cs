using CoreModels.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Crawl
{
    public class IncidentDto
    {
        public IncidentDto()
        {
            DateOfDownload = DateTime.Now;
        }

        public string Link { get; set; }
        public string Title { get; set; }
        public DateTime DateOfDownload { get; set; }
        public string Body { get; set; }
    }
}
