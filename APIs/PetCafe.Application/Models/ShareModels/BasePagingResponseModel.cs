using PetCafe.Domain.Models;
using System.Text.Json.Serialization;

namespace PetCafe.Application.Models.ShareModels;

public class BasePagingResponseModel<TData>(List<TData> datas, Pagination pagination)
{
    [JsonPropertyName("data")]
    public List<TData> Datas { get; set; } = datas;
    [JsonPropertyName("pagination")]
    public Pagination Pagination { get; set; } = pagination;

    public static BasePagingResponseModel<TData> CreateInstance(List<TData> datas, Pagination pagination)
        => new(datas, pagination);

}
