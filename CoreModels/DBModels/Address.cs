using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreModels.DBModels
{
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AddressName { get; set; }
        public District District { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Address))
                return false;

            return ((Address)obj).Id.Equals(Id);
        }

        public override string ToString()
        {
            return $"{Id} - {AddressName} - {District?.DistrictName}";
        }
    }
}
