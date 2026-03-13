using DungeonMasterCompendium.Api.Options;
using Microsoft.Extensions.Options;

namespace DungeonMasterCompendium.Api.Integrations.Open5e
{
    public class Open5eMonsterClient : IOpen5eMonsterClient
    {
        private readonly HttpClient _http;
        private readonly IOptions<Open5eOptions> _options;

        public Open5eMonsterClient(IOptions<Open5eOptions> options, HttpClient http)
        {
            _options = options;
            _http = http;
        }
    }
}
