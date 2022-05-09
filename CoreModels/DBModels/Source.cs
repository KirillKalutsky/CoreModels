using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreModels.DBModels
{
    public class Source
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public SourceType SourceType { get; set; }
        public IEnumerable<Incident> Events { get; set; }

        [Column(TypeName = "jsonb")]
        public string SerializedProperties { get; set; }

        public override string ToString()
        {
            return $"Source {Id} - {SourceType}";
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Source))
                return false;

            return ((Source)obj).Id.Equals(Id);
        }

        /* private static int id;
        public Source()
        {
            Id = id;
            id++;
        }*/
    }
}
