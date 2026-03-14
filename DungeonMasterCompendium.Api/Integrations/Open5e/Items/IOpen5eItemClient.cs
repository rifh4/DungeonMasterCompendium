namespace DungeonMasterCompendium.Api.Integrations.Open5e.Items
{
    public interface IOpen5eItemClient
    {
        Task<Open5eItemListResponse> FetchItemList(string? name, int limit, CancellationToken cancellationToken);
        Task<Open5eItemDetailItem?> FetchItemDetails(string externalId, CancellationToken cancellationToken);
    }
}