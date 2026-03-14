using DungeonMasterCompendium.Api.Contracts.Items;
using DungeonMasterCompendium.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonMasterCompendium.Api.Controllers
{
    [ApiController]
    [Route("compendium/items")]
    public sealed class ItemsController : ControllerBase
    {
        private readonly IItemsService _itemsService;

        public ItemsController(IItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        [HttpGet]
        public async Task<ActionResult<ItemListResponse>> GetItems(
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

            ItemListResponse response =
                await _itemsService.GetItems(name, limit, cancellationToken);

            return Ok(response);
        }

        [HttpGet("{externalId}")]
        public async Task<ActionResult<ItemDetailResponse>> GetItemDetails(
            string externalId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                return BadRequest("ExternalId is required.");
            }

            ItemDetailResponse? response =
                await _itemsService.GetItemDetails(externalId, cancellationToken);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}