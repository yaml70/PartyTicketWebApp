using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using partywebapp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace partywebapp.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = new PartyWebAppContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<PartyWebAppContext>>());
            AreaInitialize(context);
            GenreInitialize(context);
            ClubInitialize(context);
            AdminInitialize(context);
        }
        public static void AreaInitialize(PartyWebAppContext context)
        {
            if (context.Area.Any())
            {
                return;
            }

            context.Area.AddRange(
                new Area
                {
                    Type = AreaType.Center,
                    Parties = new List<Party>(),
                },
                new Area
                {
                    Type = AreaType.Hasharon,
                    Parties = new List<Party>(),
                },
                new Area
                {
                    Type = AreaType.North,
                    Parties = new List<Party>(),
                },
                 new Area
                 {
                     Type = AreaType.South,
                     Parties = new List<Party>(),
                 }
                );
            context.SaveChanges();
        }
        public static void GenreInitialize(PartyWebAppContext context)
        {
            if (context.Genre.Any())
            {
                return;
            }

            context.Genre.AddRange(
                new Genre
                {
                    Type = GenreType.None,
                    parties = new List<Party>(),
                },
                new Genre
                {
                    Type = GenreType.HipHop,
                    parties = new List<Party>(),
                },
                new Genre
                {
                    Type = GenreType.House,
                    parties = new List<Party>(),
                },
                new Genre
                {
                    Type = GenreType.Pop,
                    parties = new List<Party>(),
                },
                new Genre
                {
                    Type = GenreType.Rock,
                    parties = new List<Party>(),
                },
                new Genre
                {
                    Type = GenreType.Techno,
                    parties = new List<Party>(),
                }
                );
            context.SaveChanges();
        }

        public static void ClubInitialize(PartyWebAppContext context)
        {
            if (context.Club.Any())
            {
                return;
            }

            context.Club.AddRange(
                new Club
                {
                    LocationId = "ChIJBdJQhApNHRURA_y3o6MaYqg",
                    Name = "Shalvata",
                    Parties = new List<Party>(),
                },
                new Club
                {
                    LocationId = "ChIJW3DOmb1MHRURnwvGlr87HfM",
                    Name = "Clara",
                    Parties = new List<Party>(),
                },
                new Club
                {
                    LocationId = "ChIJW_8bnPJIHRURrOnaOzuwV0w",
                    Name = "Madison",
                    Parties = new List<Party>(),
                },
                new Club
                {
                    LocationId = "ChIJh9YxyvdLHRURTkZY5b2btxc",
                    Name = "Litzman",
                    Parties = new List<Party>(),
                },
                new Club
                {
                    LocationId = "ChIJC-Zldp5oAhURuE3k6iV_1-U",
                    Name = "Forum",
                    Parties = new List<Party>(),
                },
                new Club
                {
                    LocationId = "ChIJv6OW7YayHRUR5P3mNoicPUE",
                    Name = "Mirpeset",
                    Parties = new List<Party>(),
                },
                new Club
                {
                    LocationId = "ChIJs5cR1qI_HBUROHKYwj1juZ8",
                    Name = "Selina Tiberius",
                    Parties = new List<Party>(),
                },
                new Club
                {
                    LocationId = "ChIJs0wtygNuABURy9ZGm-iVVpc",
                    Name = "Selina Eilat",
                    Parties = new List<Party>(),
                }
                );
            context.SaveChanges();
        }
        public static void AdminInitialize(PartyWebAppContext context)
        {
            if (context.User.Any())
            {
                return;
            }

            context.User.AddRange(
                new User
                {
                    firstName = "Yam",
                    lastName = "Libman",
                    password = "123456",
                    email = "yamlibman70@gmail.com",
                    birthDate = new DateTime(1995, 4, 1),
                    Type = UserType.Admin,
                    parties = new List<Party>(),

                },
                new User
                {
                    firstName = "admin",
                    lastName = "admin",
                    password = "admin",
                    email = "Admin@gmail.com",
                    birthDate = new DateTime(),
                    Type = UserType.Admin,
                    parties = new List<Party>(),

                }
                );
            context.SaveChanges();
        }
    }
}
