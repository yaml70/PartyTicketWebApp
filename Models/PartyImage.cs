using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace partywebapp.Models
{
    public class PartyImage
    {
        public int Id { get; set; }
        public string imageUrl { get; set; }
        //One To One
        public int PartyId { get; set; }
        public Party Party { get; set; }
    }
}
