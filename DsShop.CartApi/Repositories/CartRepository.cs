using AutoMapper;
using DsShop.CartApi.Context;
using DsShop.CartApi.DTOs;
using DsShop.CartApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DsShop.CartApi.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CartRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CartDTO> GetCartByUserIdAsync(string userId)
    {
        Cart cart = new Cart
        {
            CartHeader = await _context.CartHeader.FirstOrDefaultAsync(c => c.UserId == userId)
        };

        // Obter os itens do cart
        cart.CartItems = _context.CartItems.Where(c => c.CartHeaderId == cart.CartHeader.Id).Include(c => c.Product);

        return _mapper.Map<CartDTO>(cart);
    }

    public async Task<bool> DeleteItemCartAsync(int cartItemId)
    {
        try
        {
            CartItem cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId);

            int total = _context.CartItems.Where(c => c.CartHeaderId == cartItem.CartHeaderId).Count();

            _context.CartItems.Remove(cartItem);

            // Remove o haeder do carrinho quando não tiver mais itens
            if (total == 1)
            {
                var cartHeaderRemove = await _context.CartHeader.FirstOrDefaultAsync(
                                                    c => c.Id == cartItem.CartHeaderId);

                _context.CartHeader.Remove(cartHeaderRemove);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CleanCartAsync(string userId)
    {
        var cartHeader = await _context.CartHeader.FirstOrDefaultAsync(c => c.UserId == userId);

        if (cartHeader is not null)
        {
            _context.CartItems.RemoveRange(
                _context.CartItems.Where(c => c.CartHeaderId == cartHeader.Id));

            _context.CartHeader.Remove(cartHeader);

            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<CartDTO> UpdateCartAsync(CartDTO cartDto)
    {
        var cart = _mapper.Map<Cart>(cartDto);

        //salva o produto no BD se não existir
        await SaveProductInDatabase(cartDto, cart);

        // Verifica se o CartHeader é null
        var cartHeader = await _context.CartHeader.AsNoTracking().FirstOrDefaultAsync(
                                c => c.UserId == cart.CartHeader.UserId);

        if (cartHeader is null)
        {
            //criar o Header e os itens
            await CreateCartHeaderAndItems(cart);
        }
        else
        {
            //atualiza a quantidade e os itens
            await UpdateQuantityAndItems(cartDto, cart, cartHeader);
        }

        return _mapper.Map<CartDTO>(cart);
    }

    private async Task UpdateQuantityAndItems(CartDTO cartDto, Cart cart, CartHeader? cartHeader)
    {
        //Se CartHeader não é null
        //verifica se CartItems possui o mesmo produto
        var cartItem = await _context.CartItems.AsNoTracking().FirstOrDefaultAsync(
                               p => p.ProductId == cartDto.CartItems.FirstOrDefault()
                               .ProductId && p.CartHeaderId == cartHeader.Id);

        if (cartItem is null)
        {
            //Cria o CartItems
            cart.CartItems.FirstOrDefault().CartHeaderId = cartHeader.Id;
            cart.CartItems.FirstOrDefault().Product = null;
            _context.CartItems.Add(cart.CartItems.FirstOrDefault());
            await _context.SaveChangesAsync();
        }
        else
        {
            //Atualiza a quantidade e o CartItems
            cart.CartItems.FirstOrDefault().Product = null;
            cart.CartItems.FirstOrDefault().Quantity += cartItem.Quantity;
            cart.CartItems.FirstOrDefault().Id = cartItem.Id;
            cart.CartItems.FirstOrDefault().CartHeaderId = cartItem.CartHeaderId;
            _context.CartItems.Update(cart.CartItems.FirstOrDefault());
            await _context.SaveChangesAsync();
        }
    }

    private async Task CreateCartHeaderAndItems(Cart cart)
    {
        //Cria o CartHeader e o CartItems
        _context.CartHeader.Add(cart.CartHeader);
        await _context.SaveChangesAsync();

        cart.CartItems.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
        cart.CartItems.FirstOrDefault().Product = null;

        _context.CartItems.Add(cart.CartItems.FirstOrDefault());

        await _context.SaveChangesAsync();
    }

    private async Task SaveProductInDatabase(CartDTO cartDto, Cart cart)
    {
        // Verifica se já foi salvo senão salva
        var product = await _context.Products.FirstOrDefaultAsync(
                                    p => p.Id == cartDto.CartItems.FirstOrDefault().ProductId);

        if (product is not null)
        {
            _context.Products.Add(cart.CartItems.FirstOrDefault().Product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ApplyCouponAsync(string userId, string couponCode)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteCouponAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
