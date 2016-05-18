using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChocAn.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
        public bool Suspended { get; set; }

        // RoleId represents whether the profile is associated with
        // 1 := member
        // 2 := provider
        // 3 := operator
        // 4 := manager
        [Range(1, 4)]
        public int RoleId { get; set; }

        // ** NOT MAPPED TO DB **
        [NotMapped]
        public string LoginId => $"{UserId:000000000}";

        [NotMapped]
        public string RoleName {
            get {
                switch (RoleId)
                {
                    case 1:
                        return "Member";
                    case 2:
                        return "Provider";
                    case 3:
                        return "Operator";
                    case 4:
                        return "Manager";
                    default:
                        return "NOT SET";
                }
            }
        }

        [NotMapped]
        public string FullName => FirstName + " " + LastName;

        public UserProfile() { }

        public override string ToString()
        {
            return
                RoleName + ": " + FirstName + " " + LastName + "\n" + Address;
        }
    }
}