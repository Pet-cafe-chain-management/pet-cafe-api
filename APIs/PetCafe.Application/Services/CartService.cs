using PetCafe.Application.Models.OrderModels;
using PetCafe.Application.Repositories;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface ICartService
{
    Task<Cart?> GetCartAsync(Guid customerId);
    Task<Cart> AddOrUpdateCartAsync(Guid customerId, OrderCreateModel model);
    Task<bool> ClearCartAsync(Guid customerId);
    Task<Cart> AddProductToCartAsync(Guid customerId, ProductOrderModel product);
    Task<Cart> RemoveProductFromCartAsync(Guid customerId, Guid productId);
    Task<Cart> AddServiceToCartAsync(Guid customerId, ServiceOrderModel service);
    Task<Cart> RemoveServiceFromCartAsync(Guid customerId, Guid slotId);
    Task<Cart> UpdateCartInfoAsync(Guid customerId, CartInfoUpdateModel model);
}

internal static class CartExtensions
{
    public static CartProductItem ToCartProductItem(this ProductOrderModel model)
    {
        return new CartProductItem
        {
            ProductId = model.ProductId,
            Quantity = model.Quantity,
            Notes = model.Notes
        };
    }

    public static CartServiceItem ToCartServiceItem(this ServiceOrderModel model)
    {
        return new CartServiceItem
        {
            SlotId = model.SlotId,
            Notes = model.Notes,
            BookingDate = model.BookingDate
        };
    }
}

public class CartService(ICartRepository _cartRepository) : ICartService
{
    public async Task<Cart?> GetCartAsync(Guid customerId)
    {
        return await _cartRepository.GetByCustomerIdAsync(customerId);
    }

    public async Task<Cart> AddOrUpdateCartAsync(Guid customerId, OrderCreateModel model)
    {
        var existingCart = await _cartRepository.GetByCustomerIdAsync(customerId);

        if (existingCart != null)
        {
            // Update existing cart
            existingCart.FullName = model.FullName;
            existingCart.Address = model.Address;
            existingCart.Phone = model.Phone;
            existingCart.Notes = model.Notes;
            existingCart.PaymentMethod = model.PaymentMethod;

            // Update products
            if (model.Products != null && model.Products.Count > 0)
            {
                existingCart.Products = model.Products.Select(p => p.ToCartProductItem()).ToList();
            }

            // Update services
            if (model.Services != null && model.Services.Count > 0)
            {
                existingCart.Services = model.Services.Select(s => s.ToCartServiceItem()).ToList();
            }

            await _cartRepository.UpdateAsync(existingCart);
            return existingCart;
        }
        else
        {
            // Create new cart
            var cart = new Cart
            {
                CustomerId = customerId,
                FullName = model.FullName,
                Address = model.Address,
                Phone = model.Phone,
                Notes = model.Notes,
                PaymentMethod = model.PaymentMethod,
                Products = model.Products?.Select(p => p.ToCartProductItem()).ToList() ?? [],
                Services = model.Services?.Select(s => s.ToCartServiceItem()).ToList() ?? []
            };

            await _cartRepository.AddAsync(cart);
            return cart;
        }
    }

    public async Task<bool> ClearCartAsync(Guid customerId)
    {
        return await _cartRepository.DeleteByCustomerIdAsync(customerId);
    }

    public async Task<Cart> AddProductToCartAsync(Guid customerId, ProductOrderModel product)
    {
        var cart = await GetOrCreateCartAsync(customerId);

        // Check if product already exists, update quantity if found
        var existingProduct = cart.Products.FirstOrDefault(p => p.ProductId == product.ProductId);
        if (existingProduct != null)
        {
            existingProduct.Quantity += product.Quantity;
            existingProduct.Notes = product.Notes ?? existingProduct.Notes;
        }
        else
        {
            cart.Products.Add(product.ToCartProductItem());
        }

        await _cartRepository.UpdateAsync(cart);
        return cart;
    }

    public async Task<Cart> RemoveProductFromCartAsync(Guid customerId, Guid productId)
    {
        var cart = await GetOrCreateCartAsync(customerId);
        cart.Products.RemoveAll(p => p.ProductId == productId);
        await _cartRepository.UpdateAsync(cart);
        return cart;
    }

    public async Task<Cart> AddServiceToCartAsync(Guid customerId, ServiceOrderModel service)
    {
        var cart = await GetOrCreateCartAsync(customerId);

        // Check if service (slot) already exists, replace it if found
        var existingServiceIndex = cart.Services.FindIndex(s => s.SlotId == service.SlotId);
        if (existingServiceIndex >= 0)
        {
            cart.Services[existingServiceIndex] = service.ToCartServiceItem();
        }
        else
        {
            cart.Services.Add(service.ToCartServiceItem());
        }

        await _cartRepository.UpdateAsync(cart);
        return cart;
    }

    public async Task<Cart> RemoveServiceFromCartAsync(Guid customerId, Guid slotId)
    {
        var cart = await GetOrCreateCartAsync(customerId);
        cart.Services.RemoveAll(s => s.SlotId == slotId);
        await _cartRepository.UpdateAsync(cart);
        return cart;
    }

    public async Task<Cart> UpdateCartInfoAsync(Guid customerId, CartInfoUpdateModel model)
    {
        var cart = await GetOrCreateCartAsync(customerId);

        if (model.FullName != null) cart.FullName = model.FullName;
        if (model.Address != null) cart.Address = model.Address;
        if (model.Phone != null) cart.Phone = model.Phone;
        if (model.Notes != null) cart.Notes = model.Notes;
        if (model.PaymentMethod != null) cart.PaymentMethod = model.PaymentMethod;

        await _cartRepository.UpdateAsync(cart);
        return cart;
    }

    private async Task<Cart> GetOrCreateCartAsync(Guid customerId)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
        if (cart == null)
        {
            cart = new Cart
            {
                CustomerId = customerId,
                Products = [],
                Services = []
            };
            await _cartRepository.AddAsync(cart);
        }
        return cart;
    }
}

