using DungeonMasterCompendium.Api.Contracts.Monsters;
using DungeonMasterCompendium.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonMasterCompendium.Api.Controllers
{
    [ApiController]
    [Route("compendium/monsters")]
    public sealed class MonstersController : ControllerBase
    {
        private readonly IMonstersService _monstersService;

        public MonstersController(IMonstersService monstersService)
        {
            _monstersService = monstersService;
        }

        [HttpGet]
        public async Task<ActionResult<MonsterListResponse>> GetMonsters(
            [FromQuery] string? name,
            [FromQuery] int limit = 20,
            CancellationToken cancellationToken = default)
        {
            // Keep query validation consistent across compendium endpoints so the API fails fast
            // before reaching service or integration layers.
            if (limit < 1 || limit > 100)
            {
                return BadRequest("Limit must be between 1 and 100.");
            }

            if (!string.IsNullOrWhiteSpace(name) && name.Trim().Length > 50)
            {
                return BadRequest("Name must be 50 characters or less.");
            }

            MonsterListResponse results = await _monstersService.GetMonsters(name, limit, cancellationToken);
            return Ok(results);
        }

        [HttpGet("{externalId}")]
        public async Task<ActionResult<MonsterDetailResponse>> GetMonsterDetails(
            string externalId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                return BadRequest("ExternalId is required.");
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