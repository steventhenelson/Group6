using System.ComponentModel.DataAnnotations.Schema;

namespace ChocAn.Models
{
    [ComplexType]
    public class Address
    {
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Zip { get; set; }

        [NotMapped]
        public string ZipToString => $"{Zip:00000}";

        public override string ToString()
        {
            return
                AddressLineOne +
                (!string.IsNullOrEmpty(AddressLineTwo) ? "\n" + AddressLineTwo : "\n") +
                City + ", " + State + " " + ZipToString;
        }
    }

}