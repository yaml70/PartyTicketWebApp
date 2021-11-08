using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace partywebapp.Services
{
    public interface IManageService
    {
        Dictionary<string, int> CalculatePartiesByGenre();
        Dictionary<string, int> InitGenreDictionary();

        Dictionary<string, int> getGenreStats(Dictionary<string, int> initDictonary);

        string GetPartiesInGenre();

        Dictionary<string, int> CalculatePartiesByClub();

        Dictionary<string, int> InitClubDictionary();

        Dictionary<string, int> getClubStats(Dictionary<string, int> initDictonary);

        string GetPartiesInClub();

        Dictionary<string, int> CalculatePartiesByArea();

        Dictionary<string, int> InitAreaDictionary();

        Dictionary<string, int> getAreaStats(Dictionary<string, int> initDictonary);

        string GetPartiesInArea();
    }
}
