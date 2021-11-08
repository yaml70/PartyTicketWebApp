using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace partywebapp.Services
{
    public class SpotifyClientService : ISpotifyClientService
    {
        private static readonly string SPOTIFY_CLIENT_ID = "02efcc1c80524bd0b74618b88ae6e3fb";
        private static readonly string SPOTIFY_CLIENT_SECRET = "7803305bcfdb43b2a0a879e63a188a81";
        private readonly SpotifyClient _spotify;

        public SpotifyClientService()
        {
            var config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(new ClientCredentialsAuthenticator(SPOTIFY_CLIENT_ID, SPOTIFY_CLIENT_SECRET));

            _spotify = new SpotifyClient(config);
        }

        public async Task<string> getArtistIdBySearchParams(string searchParams)
        {
            var searchResponse = await _spotify.Search.Item(new SearchRequest(SearchRequest.Types.Artist, searchParams)
            {
                Limit = 1,

            });
            if (searchResponse.Artists.Items.Count > 0) return searchResponse.Artists.Items.First().Id;
            else return "NO_RESULT";

        }


        public async Task<FullArtist> GetArtist(string artistId)
        {
            var artist = await _spotify.Artists.Get(artistId);
            return artist;
        }

        public async Task<List<FullArtist>> GetArtists(List<string> artistsId)
        {
            var artists = await _spotify.Artists.GetSeveral(new ArtistsRequest(artistsId));
            return artists.Artists;
        }
    }
}
