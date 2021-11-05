using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace partywebapp.Data
{
    public class PartyWebAppContext : DbContext
    {
        public PartyWebAppContext(DbContextOptions<PartyWebAppContext> options)
: base(options)
        {
        }
        public DbSet<partywebapp.Models.Party> Party { get; set; }
        public DbSet<partywebapp.Models.Performer> Performer { get; set; }
        public DbSet<partywebapp.Models.PartyImage> PartyImage { get; set; }
        public DbSet<partywebapp.Models.User> User { get; set; }
        public DbSet<partywebapp.Models.Area> Area { get; set; }
        public DbSet<partywebapp.Models.Genre> Genre { get; set; }
        public DbSet<partywebapp.Models.Club> Club { get; set; }
    }
}
