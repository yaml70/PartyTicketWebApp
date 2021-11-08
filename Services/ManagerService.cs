using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using partywebapp.Data;
using partywebapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace partywebapp.Services
{
    public class ManagerService : IManageService
    {
        private readonly PartyWebAppContext _context;
        private readonly DateTime relevantTimeRange;
        public ManagerService(PartyWebAppContext context)
        {
            _context = context;
            int HalfYearInDays = 180;
            relevantTimeRange = DateTime.Now - TimeSpan.FromDays(HalfYearInDays);
        }


        public Dictionary<string, int> CalculatePartiesByGenre()
        {
            Dictionary<string, int> initGenreDictionary = InitGenreDictionary();
            return getGenreStats(initGenreDictionary);

        }

        public Dictionary<string, int> InitGenreDictionary()
        {
            Dictionary<string, int> initGenreDictionary = new Dictionary<string, int>();

            var allExistingGenres = from genre in _context.Genre
                                    select new
                                    {
                                        key = genre.Type.ToString(),
                                        count = 0

                                    };

            foreach (var genreGroup in allExistingGenres)
            {
                initGenreDictionary.Add(genreGroup.key, genreGroup.count);
            }

            return initGenreDictionary;
        }

        public Dictionary<string, int> getGenreStats(Dictionary<string, int> initDictonary)
        {
            var quertResults = from party in _context.Party
                               where party.eventDate >= relevantTimeRange
                               join genre in _context.Genre
                               on party.genreId equals genre.Id
                               select new
                               {
                                   genre.Type,
                                   party.Id
                               } into partyGenre
                               group partyGenre by partyGenre.Type into genreCategories
                               select new
                               {
                                   key = genreCategories.Key.ToString(),
                                   count = genreCategories.Count()
                               };

            foreach (var genreGroup in quertResults)
            {
                initDictonary[genreGroup.key] = genreGroup.count;
            }

            return initDictonary;
        }

        public string GetPartiesInGenre()
        {
            Dictionary<string, int> PartiesInGenre = CalculatePartiesByGenre();
            return JsonConvert.SerializeObject(PartiesInGenre);
        }

        public Dictionary<string, int> CalculatePartiesByClub()
        {
            Dictionary<string, int> initClubDictionary = InitClubDictionary();
            return getClubStats(initClubDictionary);
        }

        public Dictionary<string, int> InitClubDictionary()
        {
            Dictionary<string, int> initClubDictionary = new Dictionary<string, int>();

            var allExistingClubs = from club in _context.Club
                                   select new
                                   {
                                       key = club.Name,
                                       count = 0

                                   };

            foreach (var clubGroup in allExistingClubs)
            {
                initClubDictionary.Add(clubGroup.key, clubGroup.count);
            }

            return initClubDictionary;
        }

        public Dictionary<string, int> getClubStats(Dictionary<string, int> initDictonary)
        {
            var quertResults = from party in _context.Party
                               where party.eventDate >= relevantTimeRange
                               join club in _context.Club
                               on party.clubId equals club.Id
                               select new
                               {
                                   club.Name,
                                   party.Id
                               } into partyClub
                               group partyClub by partyClub.Name into Clubs
                               select new
                               {
                                   key = Clubs.Key,
                                   count = Clubs.Count()
                               };

            foreach (var genreGroup in quertResults)
            {
                initDictonary[genreGroup.key] = genreGroup.count;
            }

            return initDictonary;
        }
        public string GetPartiesInClub()
        {
            Dictionary<string, int> PartiesInClub = CalculatePartiesByClub();
            return JsonConvert.SerializeObject(PartiesInClub);
        }
        public Dictionary<string, int> CalculatePartiesByArea()
        {

            Dictionary<string, int> initAreaDictionary = InitAreaDictionary();
            return getAreaStats(initAreaDictionary);
        }

        public Dictionary<string, int> InitAreaDictionary()
        {
            Dictionary<string, int> initAreaDictionary = new Dictionary<string, int>();

            var allExistingArea = from area in _context.Area
                                  select new
                                  {
                                      key = area.Type.ToString(),
                                      count = 0

                                  };

            foreach (var areaGroup in allExistingArea)
            {
                initAreaDictionary.Add(areaGroup.key, areaGroup.count);
            }

            return initAreaDictionary;
        }

        public Dictionary<string, int> getAreaStats(Dictionary<string, int> initDictonary)
        {
            var quertResults = from party in _context.Party
                               where party.eventDate >= relevantTimeRange
                               join area in _context.Area
                               on party.areaId equals area.Id
                               select new
                               {
                                   area.Type,
                                   party.Id
                               } into partyArea
                               group partyArea by partyArea.Type into areaCategories
                               select new
                               {
                                   key = areaCategories.Key.ToString(),
                                   count = areaCategories.Count()
                               };

            foreach (var areaGroup in quertResults)
            {
                initDictonary[areaGroup.key] = areaGroup.count;
            }

            return initDictonary;
        }

        public string GetPartiesInArea()
        {
            Dictionary<string, int> PartiesInArea = CalculatePartiesByArea();
            return JsonConvert.SerializeObject(PartiesInArea);
        }
    }
}
