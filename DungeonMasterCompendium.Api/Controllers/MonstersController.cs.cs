using DungeonMasterCompendium.Api.Integrations.Open5e;
using DungeonMasterCompendium.Api.Services;
using Microsoft.AspNetCore.Mvc;


namespace DungeonMasterCompendium.Api.Controllers
{
    [ApiController]
    [Route("compendium/monsters")]
    public class MonstersController : ControllerBase
    {
        private readonly IOpen5eMonsterClient _open5eMonsterClient;
        private readonly IMonstersService _monstersService;

        public MonstersController(IOpen5eMonsterClient open5eMonsterClient, IMonstersService monstersService)
        {
            _open5eMonsterClient = open5eMonsterClient;
            _monstersService = monstersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBaseMonster([FromQuery] string? name, CancellationToken cancellationToken, [FromQuery] int limit = 20)
        {
            Open5eMonsterListResponse results = await _monstersService.GetMonsters(name, limit, cancellationToken);
            return Ok(results);
        }

        [HttpGet("{externalId}")]
        public async Task<IActionResult> GetMonsterDetails(string externalId, CancellationToken cancellationToken)
        {
            Open5eMonsterDetailItem? monster = await _open5eMonsterClient.FetchMonsterDetails(externalId, cancellationToken);
            if (monster == null)
            {
                return NotFound();
            }
            return Ok(monster);
        }

    }
}
