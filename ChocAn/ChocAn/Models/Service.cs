using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChocAn.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        // ** Model Properties **
        // set max length after entity and db are interacting correctly i generally do the validation on 
        // this on the client side anyways so this should never error at this point. if it does we have a
        // UI validation problem somewhere else that we need to check
        [StringLength(maximumLength: 50)]
        public string ServiceName { get; set; }
        public decimal Fee { get; set; }
        public bool Enabled { get; set; }

        // ** Not Mapped **
        [NotMapped]
        public string ServiceCode => $"{ServiceId:000000}";

        [NotMapped]
        public string FeeToString => $"{Fee:C}";

        public override string ToString()
        {
            return ServiceName + ": " + ServiceCode;
        }
    }

}
