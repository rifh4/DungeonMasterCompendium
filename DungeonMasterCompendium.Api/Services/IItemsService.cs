using DungeonMasterCompendium.Api.Contracts.Items;

namespace DungeonMasterCompendium.Api.Services
{
    public interface IItemsService
    {
        Task<ItemListResponse> GetItems(string? name, int limit, CancellationToken cancellationToken);
        Task<ItemDetailResponse?> GetItemDetails(string externalId, CancellationToken cancellationToken);
    }
}