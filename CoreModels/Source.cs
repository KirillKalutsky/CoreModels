using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreModels
{
    public class Source
    {
        private static int id;
        public Source()
        {
            Id = id;
            id++;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public SourceType SourceType { get; set; }
        public IEnumerable<Event> Events { get; set; }
        public SourceFields Fields { get; set; }

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

    }
}
