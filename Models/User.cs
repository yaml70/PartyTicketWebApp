using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace partywebapp.Models
{

    public enum UserType
    {
        Client,
        producer,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string firstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string email { get; set; }
        [DataType(DataType.Date)]
        [Required]
        [Display(Name = "Birth Date")]
        public DateTime birthDate { get; set; }
        public UserType Type { get; set; } = UserType.Client;
        public List<Party> parties { get; set; }
    }
}
