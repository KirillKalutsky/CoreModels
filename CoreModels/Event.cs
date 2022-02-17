using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreModels
{
    public class Event
    {
        [Key]
        public string Link { get; set; }
        public int IdSource { get; set; }
        public string Title { get; set; }
        public string IncidentCategory {get;set;}
        public DateTime DateOfDownload { get; set; }
        public string DistrictName { get; set; }

        [NotMapped]
        [JsonIgnore]
        private string body;

        [NotMapped]
        [JsonIgnore]
        public string Body 
        {
            get { return body; }
            set
            {
                if (value.Length > 1000)
                    body = value.Substring(0, 1000);
                else
                    body = value;
            }

        }

        public override int GetHashCode()
        {
            return Link.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Event))
                return false;
            
            return ((Event)obj).Link.Equals(Link);
        }

        public override string ToString()
        {
            return $"{Link}\n{Body}";
        }
    }
}
