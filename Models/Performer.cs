using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using SpotifyAPI.Web;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace partywebapp.Models
{
    public class Performer
    {
        public int Id { get; set; }
        [Display(Name = "ID in Spotify")]
        public string SpotifyId { get; set; }
        public List<Party> parties { get; set; }
    }
}
