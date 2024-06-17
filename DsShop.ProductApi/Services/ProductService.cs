using AutoMapper;
using DsShop.ProductApi.DTOs;
using DsShop.ProductApi.Models;
using DsShop.ProductApi.Repositories;

namespace DsShop.ProductApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDTO>> GetProducts()
    {
        var products = await _repository.GetAll();
        return _mapper.Map<IEnumerable<ProductDTO>>(products);
    }

    public async Task<ProductDTO> GetProductById(int id)
    {
        var productDb = await _repository.GetById(id);
        return _mapper.Map<ProductDTO>(productDb);
    }

    public async Task AddProduct(ProductDTO productDTO)
    {
        var productEntity = _mapper.Map<Product>(productDTO);
        await _repository.Create(productEntity);
        productDTO.Id = productEntity.Id;
    }

    public async Task UpdateProduct(ProductDTO productDTO)
    {
        var product = _mapper.Map<Product>(productDTO);
        await _repository.Update(product);
    }
    public async Task RemoveProduct(int id)
    {
        var product = _repository.GetById(id).Result;
        await _repository.Delete(product.Id);
    }
}
