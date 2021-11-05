using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace partywebapp.Models
{
    public enum AreaType
    {
        [Display(Name = "Center")]
        Center = 1,
        [Display(Name = "North")]
        North = 2,
        [Display(Name = "South")]
        South = 3,
        [Display(Name = "Hashron")]
        Hasharon = 4,
    }
    public class Area
    {
        public int Id { get; set; }
        public AreaType Type { get; set; }
        public List<Party> Parties { get; set; }
    }
}
