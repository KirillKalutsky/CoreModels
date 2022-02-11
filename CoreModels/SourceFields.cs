using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreModels
{
    public class SourceFields
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "jsonb")]
        public string Properties { get; set; }
        public int SourceId { get; set; }
        public Source Source { get; set; }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SourceFields))
                return false;

            return ((SourceFields)obj).Id.Equals(Id);
        }

        public override string ToString()
        {
            return $"{Id} - SourceId: {SourceId}";
        }
    }
}
