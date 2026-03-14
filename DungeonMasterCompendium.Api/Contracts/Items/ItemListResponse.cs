namespace DungeonMasterCompendium.Api.Contracts.Items
{
    public sealed class ItemListResponse
    {
        public int Count { get; set; }
        public List<ItemListItemResponse> Results { get; set; } = new List<ItemListItemResponse>();
    }
}