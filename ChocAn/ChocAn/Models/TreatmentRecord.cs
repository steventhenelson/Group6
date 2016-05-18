using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChocAn.Models
{
    [Table("TreatmentRecords")]
    public class TreatmentRecord
    {
        [Key]
        public int TreatmentRecordId { get; set; }

        // ** Foreign Keys **
        public virtual Service Service { get; set; }
        public virtual UserProfile Member { get; set; }
        public virtual UserProfile Provider { get; set; }

        // ** Model Properties
        public DateTime TreatmentDate { get; set; }
        public DateTime EntryDate { get; set; }

        [NotMapped]
        public string EntryDateString => $"{EntryDate:MM-dd-yyy}";

        [StringLength(maximumLength: 255)]
        public string Comments { get; set; }

        public override string ToString()
        {
            return Member + "\n" + Service + "\n" + EntryDateString;
        }
    }

}
