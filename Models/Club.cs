using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace partywebapp.Models
{
    public class Club
    {
        public int Id { get; set; }
        public string LocationId { get; set; }
        public string Name { get; set; }
        public List<Party> Parties { get; set; }
    }
}
