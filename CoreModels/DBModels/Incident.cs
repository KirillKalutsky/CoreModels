using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreModels.DBModels
{
    public class Incident
    {
        [Key]
        public string Link { get; set; }
        public int IdSource { get; set; }
        public string Title { get; set; }
        public IncidentCategory IncidentCategory {get;set;}
        public DateTime DateOfDownload { get; set; }
        public string DistrictName { get; set; }

        public override int GetHashCode()
        {
            return Link.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Incident))
                return false;
            
            return ((Incident)obj).Link.Equals(Link);
        }

        public override string ToString()
        {
            return $"{Link}";
        }
    }
}
