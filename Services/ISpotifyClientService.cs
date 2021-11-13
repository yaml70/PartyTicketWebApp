using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;


namespace partywebapp.Services
{
    public interface ISpotifyClientService
    {
        
        Task<List<FullArtist>> GetArtists(List<string> artists);

        Task<FullArtist> GetArtist(string artistId);

        Task<string> getArtistIdBySearchParams(string searchParams);
        
    }
}
