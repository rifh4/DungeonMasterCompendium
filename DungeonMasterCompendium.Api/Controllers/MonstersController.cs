using DungeonMasterCompendium.Api.Contracts.Monsters;
using DungeonMasterCompendium.Api.Services;
using Microsoft.AspNetCore.Mvc;


namespace DungeonMasterCompendium.Api.Controllers
{
    [ApiController]
    [Route("compendium/monsters")]
    public class MonstersController : ControllerBase
    {
        private readonly IMonstersService _monstersService;

        public MonstersController(IMonstersService monstersService)
        {
            _monstersService = monstersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBaseMonster([FromQuery] string? name, CancellationToken cancellationToken, [FromQuery] int limit = 20)
        {
            if (limit < 1 || limit > 100)
            {
                return BadRequest("limit must be between 1 and 100.");
            }
            else if (name != null && name.Length > 50)
            {
                return BadRequest("name must be 50 characters or fewer.");
            }

            MonsterListResponse results = await _monstersService.GetMonsters(name, limit, cancellationToken);
            return Ok(results);
        }

        [HttpGet("{externalId}")]
        public async Task<IActionResult> GetMonsterDetails(string externalId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                return BadRequest("externalId is required.");
            }

            MonsterDetailResponse? monster = await _monstersService.GetMonsterDetails(externalId, cancellationToken);
            if (monster == null)
            {
                return NotFound();
            }
            return Ok(monster);
        }

    }
}
