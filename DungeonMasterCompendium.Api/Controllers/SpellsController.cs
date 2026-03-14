using DungeonMasterCompendium.Api.Contracts.Spells;
using DungeonMasterCompendium.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonMasterCompendium.Api.Controllers
{
    [ApiController]
    [Route("compendium/spells")]
    public sealed class SpellsController : ControllerBase
    {
        private readonly ISpellsService _spellsService;

        public SpellsController(ISpellsService spellsService)
        {
            _spellsService = spellsService;
        }

        [HttpGet]
        public async Task<ActionResult<SpellListResponse>> GetSpells(
            [FromQuery] string? name,
            [FromQuery] int limit = 20,
            CancellationToken cancellationToken = default)
        {
            if (limit < 1 || limit > 100)
            {
                return BadRequest("Limit must be between 1 and 100.");
            }

            if (!string.IsNullOrWhiteSpace(name) && name.Trim().Length > 50)
            {
                return BadRequest("Name must be 50 characters or less.");
            }

            SpellListResponse response =
                await _spellsService.GetSpells(name, limit, cancellationToken);

            return Ok(response);
        }

        [HttpGet("{externalId}")]
        public async Task<ActionResult<SpellDetailResponse>> GetSpellDetails(
            string externalId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                return BadRequest("ExternalId is required.");
            }

            SpellDetailResponse? response =
                await _spellsService.GetSpellDetails(externalId, cancellationToken);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
