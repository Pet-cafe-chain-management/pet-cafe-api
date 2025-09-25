using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PetCafe.Application.Models.ShareModels;

public class OrderParam
{
    public string? OrderColumn { get; set; }
    public string? OrderDir { get; set; } = "ASC";
}

public class FilterQuery
{
    [FromQuery(Name = "page")]
    public int? Page { get; set; } = 0;
    [FromQuery(Name = "limit")]
    public int? Limit { get; set; } = 10;
    [FromQuery(Name = "q")]
    public string? Q { get; set; } = string.Empty;

    [FromQuery(Name = "order_by")]
    [ModelBinder(BinderType = typeof(OrderParamListBinder))]
    public OrderParam[]? OrderBy { get; set; }
}
public class OrderParamListBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var request = bindingContext.HttpContext.Request;
        var orderParams = new List<OrderParam>();

        // Parse query parameters like: order_by[0][order_column]=create_date&order_by[0][order_dir]=ASC
        var orderByKeys = request.Query.Keys
            .Where(k => k.StartsWith("order_by["))
            .ToList();

        // If no order_by parameters found, don't bind anything
        if (orderByKeys.Count == 0)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        // Group by index
        var groupedParams = orderByKeys
            .Select(key =>
            {
                // Extract index from order_by[0][field_name]
                var startIndex = key.IndexOf('[') + 1;
                var endIndex = key.IndexOf(']', startIndex);
                if (startIndex > 0 && endIndex > startIndex)
                {
                    if (int.TryParse(key.Substring(startIndex, endIndex - startIndex), out int index))
                    {
                        var fieldStart = key.IndexOf('[', endIndex) + 1;
                        var fieldEnd = key.IndexOf(']', fieldStart);
                        if (fieldStart > 0 && fieldEnd > fieldStart)
                        {
                            var fieldName = key.Substring(fieldStart, fieldEnd - fieldStart);
                            return new { Index = index, Field = fieldName, Key = key };
                        }
                    }
                }
                return null;
            })
            .Where(x => x != null)
            .GroupBy(x => x!.Index)
            .OrderBy(g => g.Key);

        foreach (var group in groupedParams)
        {
            var orderParam = new OrderParam();

            foreach (var item in group)
            {
                var value = request.Query[item!.Key].FirstOrDefault();
                // Skip empty values - don't process empty order parameters
                if (string.IsNullOrWhiteSpace(value)) continue;

                switch (item.Field.ToLower())
                {
                    case "order_column":
                        orderParam.OrderColumn = value.Trim();
                        break;
                    case "order_dir":
                        orderParam.OrderDir = value.Trim().ToUpper();
                        break;
                }
            }

            // Only add if we have a valid order column (ignore empty ones)
            if (!string.IsNullOrWhiteSpace(orderParam.OrderColumn))
            {
                orderParams.Add(orderParam);
            }
        }

        // If we parsed parameters but none were valid, don't bind
        if (orderParams.Count == 0)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }


        bindingContext.Result = ModelBindingResult.Success(orderParams.ToArray());
        return Task.CompletedTask;
    }
}


// Model Binder Provider
public class OrderParamListBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(OrderParam[]))
        {
            return new OrderParamListBinder();
        }
        return null;
    }
}
public class FilterQueryExtend : FilterQuery
{
    public string FilterString { get; set; } = string.Empty;
}
