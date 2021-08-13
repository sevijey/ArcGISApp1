using System;
using System.Collections.Generic;
using System.Text;

namespace ArcGISApp1.Models
{
    public class SignProcess
    {
        public string EnvelopeId { get; set; }
        public List<Stakeholder> StakeholderList { get; set; }
        public string State { get; set; }
        public string SendDateTime { get; set; }
        public string CreatedMapDate { get; set; }
        public string ModifiedMapDate { get; set; }
        public bool IsDownloaded { get; set; }
    }
}
