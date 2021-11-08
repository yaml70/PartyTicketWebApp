using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using partywebapp.Data;
using partywebapp.Models;
using Microsoft.AspNetCore.Identity;
using partywebapp.Services;
using Microsoft.AspNetCore.Http;

namespace partywebapp.Services
{
    public class PartiesService
    {
        private readonly PartyWebAppContext _context;

        public PartiesService(PartyWebAppContext context)
        {
            _context = context;
        }

        public List<Party> convertContextPartyToList()
        {
            return _context.Party
                .Include(p => p.partyImage)
                .Include(p => p.area)
                .Include(p => p.genre)
                .Include(p => p.club)
                .ToList();
        }
        public List<Performer> convertContextPerformerToList()
        {
            return _context.Performer
                .Include(p => p.parties)
                .ToList();
        }
        public List<Club> convertContextClubToList()
        {
            return _context.Club
                .Include(c => c.Parties)
                .ToList();
        }

        public void homePageInit(HomePage homePage)
        {
            homePage.clubs = new List<Club>();
            homePage.parties = new List<Party>();
            homePage.performers = new List<Performer>();
        }
        public HomePage indexInitNumOfObject(HomePage homePage, int numOfObject)
        {

            homePageInit(homePage);

            List<Party> allParties = convertContextPartyToList();
            List<Performer> allPerformers = convertContextPerformerToList();
            List<Club> allClubs = convertContextClubToList();


            for (int i = 0; i < 5; i++)
            {
                homePage.parties.Add(allParties[i]);
                homePage.performers.Add(allPerformers[i]);
                homePage.clubs.Add(allClubs[i]);
            }

            return homePage;
        }

        const int numOfObjectToReturnInIndex = 3;
        public HomePage getDataForHomePage(HomePage homePage)
        {
            homePageInit(homePage);
            return indexInitNumOfObject(homePage, numOfObjectToReturnInIndex);
        }

        public void addPerformersToParty(Party party, List<string> perfomersSpotifyIds)
        {
            foreach (string spotifyId in perfomersSpotifyIds)
            {
                if (party.performers == null) party.performers = new List<Performer>();
                var existingPerformer = _context.Performer.FirstOrDefault(performer => performer.SpotifyId == spotifyId);
                bool isPerformerExistInDb = existingPerformer != null;
                if (isPerformerExistInDb)
                {
                    addExistingPerformerToParty(party, existingPerformer);
                }
                else
                {
                    var newPerformer = new Performer
                    {
                        SpotifyId = spotifyId,
                        parties = new List<Party>()
                    };
                    addNewPerformerToParty(party, newPerformer);
                }
            }
        }

        public void addNewPerformerToParty(Party party, Performer performer)
        {
            performer.parties.Add(party);
            party.performers.Add(performer);
            _context.Performer.Add(performer);
            _context.Add(party);
        }

        public void addExistingPerformerToParty(Party party, Performer performer)
        {
            if (performer.parties != null) performer.parties.Add(party);
            else
            {
                performer.parties = new List<Party>();
                performer.parties.Add(party);
            }
            party.performers.Add(performer);
            _context.Update(performer);
            _context.Add(party);
        }

        const string imageIsNull = "https://media.istockphoto.com/vectors/no-image-available-sign-vector-id1138179183?k=6&m=1138179183&s=612x612&w=0&h=prMYPP9mLRNpTp3XIykjeJJ8oCZRhb2iez6vKs8a8eE=";
        public string defaultImageIfIsNull(string Url)
        {
            if (String.IsNullOrEmpty(Url))
            {
                Url = imageIsNull;
            }
            return Url;
        }
        public void addImageToParty(Party party, string Url)
        {
            Url = defaultImageIfIsNull(Url);
            PartyImage partyImage = new PartyImage();
            partyImage.imageUrl = Url;
            partyImage.PartyId = party.Id;
            partyImage.Party = party;
            _context.PartyImage.Add(partyImage);
            party.partyImage = partyImage;
            _context.Party.Add(party);
        }





        const int numOfMostPopularPartyToReturn = 5;
        public IEnumerable<Party> mostPopularParties()
        {
            List<Party> mostPopularParties = convertContextPartyToList();
            return mostPopularParties.OrderByDescending(u => u.users.Count).Take(numOfMostPopularPartyToReturn);
        }

        const int numOfSortPartiesAreaTypeForEach = 3;
        public IEnumerable<Party> sortByAreaType(int AreaType)
        {
            List<Party> sotyByArea = convertContextPartyToList();
            return sotyByArea.OrderByDescending(p => p.area.Id == AreaType).Take(numOfMostPopularPartyToReturn);
        }


        public void addProducerId(Party party, User user)
        {
            var p = _context.Party.FirstOrDefault(p => p.Id == party.Id);
            var u = _context.User.FirstOrDefault(u => u.Id == user.Id);
            if (p != null && u != null)
            {
                if (u.parties == null)
                {
                    u.parties = new List<Party>();
                }
                p.ProducerId = u.Id;
                u.parties.Add(p);
            }
        }


        public string areaTypeToString(AreaType type)
        {
            if (type.Equals(AreaType.Center))
            {
                return "Center";
            }
            if (type.Equals(AreaType.Hasharon))
            {
                return "Hasharon";
            }
            if (type.Equals(AreaType.North))
            {
                return "North";
            }
            if (type.Equals(AreaType.South))
            {
                return "South";
            }

            return null;
        }
        public bool isAvailableTickets(Party party, User user, int numOfTickets)
        {
            if (party != null)
            {
                int availebleTickets = party.maxCapacity - party.ticketsPurchased;
                if (availebleTickets >= numOfTickets)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public Party addTicketsCountToParty(Party currentParty, int numOfTickets, User user)
        {
            if (currentParty != null)
            {
                if (isAvailableTickets(currentParty, user, numOfTickets))
                {
                    if (currentParty.users == null)
                    {
                        currentParty.users = new List<User>();
                    }
                    if (user.parties == null)
                    {
                        user.parties = new List<Party>();
                    }
                    if (currentParty.users.Contains(user) || user.parties.Contains(currentParty))
                    {
                        currentParty.ticketsPurchased += numOfTickets;
                    }
                    else
                    {
                        currentParty.ticketsPurchased += numOfTickets;
                        if (!currentParty.users.Contains(user))
                        {
                            currentParty.users.Add(user);
                        }
                        if (!user.parties.Contains(currentParty))
                        {
                            user.parties.Add(currentParty);
                        }
                    }
                }
            }
            return currentParty;
        }

        public int CalculateAge(DateTime dateOfBirth)
        {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
            {
                age = age - 1;
            }
            return age;
        }

    }
}
