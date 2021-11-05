using CustomDataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace partywebapp.Models
{
    public class Party
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Party Name")]
        [StringLength(25, MinimumLength = 3)]
        [Required]
        public string name { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Event Date")]
        [CurrentDate]
        [Required]
        public DateTime eventDate { get; set; }

        [Display(Name = "Minimal Age")]
        [Range(18, 67)]
        [Required]
        public int minimalAge { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Open Doors")]
        [Required]
        public DateTime startTime { get; set; }
        [Display(Name = "Tickets purchased")]
        public int ticketsPurchased { get; set; }


        //All OneToMany 
        public int genreId { get; set; }
        public Genre genre { get; set; }
        public int areaId { get; set; }
        public Area area { get; set; }
        public int clubId { get; set; }
        [Display(Name = "Club")]
        public Club club { get; set; }
        public int ProducerId { get; set; }


        [Display(Name = "Maximum Capacity")]
        [Required]
        [Range(30, 30000)]
        public int maxCapacity { get; set; }
        [Display(Name = "Price in NIS")]
        [Required]
        [Range(0, 2000)]
        public double price { get; set; }

        //Many To Many 
        public List<Performer> performers { get; set; }
        public List<User> users { get; set; }

        //One To One 
        public PartyImage partyImage { get; set; }
    }
}
