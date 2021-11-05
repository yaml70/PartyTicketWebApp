using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace partywebapp.Models
{

    public enum GenreType
    {
        None = 0x0,
        [Display(Name = "Hip Hop")]
        HipHop = 1,
        [Display(Name = "Rock")]
        Rock = 2,
        [Display(Name = "Techno")]
        Techno = 3,
        [Display(Name = "House")]
        House = 4,
        [Display(Name = "Pop")]
        Pop = 5,
    }

    public class Genre
    {
        public int Id { get; set; }
        public GenreType Type { get; set; }
        public List<Party> parties { get; set; }
    }
}
