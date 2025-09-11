using System.Text.Json.Serialization;

namespace PetCafe.Domain.Models;

public class Pagination
{
    [JsonPropertyName("total_items_count")]
    public int TotalItemsCount { get; set; }

    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    [JsonPropertyName("total_pages_count")]
    public int TotalPagesCount
    {
        get
        {
            var temp = TotalItemsCount / PageSize;
            return TotalItemsCount % PageSize == 0 ? temp : temp + 1;
        }
    }

    [JsonPropertyName("page_index")]
    public int PageIndex { get; set; }

    /// <summary>
    /// Page number starts from 0
    /// </summary>
    [JsonPropertyName("has_next")]
    public bool Next => PageIndex + 1 < TotalPagesCount;

    [JsonPropertyName("has_previous")]
    public bool Previous => PageIndex > 0;

}
