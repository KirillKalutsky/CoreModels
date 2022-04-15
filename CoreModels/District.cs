using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreModels
{
    public class District
    {
        public District()
        {
            Addresses = new List<Address>();
        }

        public District(string name)
        {
            DistrictName  = name;
            Addresses = new List<Address>();
        }

        [Key]
        public string DistrictName  { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<Event> Events { get; set; }

        public override int GetHashCode()
        {
            return DistrictName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is District))
                return false;

            return ((District)obj).DistrictName.Equals(DistrictName);
        }

        public override string ToString()
        {
            return $"{DistrictName}";
        }
    }
}
