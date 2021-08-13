using System;
using System.Collections.Generic;
using System.Text;
using static DocuSign.eSign.Client.Auth.OAuth;

namespace ArcGISApp1.Models
{
    public class ApiCallsInfo
    {
        public OAuthToken AuthToken { get; set; }
        public string AccountId { get; set; }
        public string Baseuri { get; set; }
        public DateTime CreatedTokenTime { get; set; }
    }
}
