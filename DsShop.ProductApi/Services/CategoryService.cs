using AutoMapper;
using DsShop.ProductApi.DTOs;
using DsShop.ProductApi.Models;
using DsShop.ProductApi.Repositories;

namespace DsShop.ProductApi.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDTO>> GetCategories()
    {
        var categoriesEntity = await _categoryRepository.GetCategories();
        return _mapper.Map<IEnumerable<CategoryDTO>>(categoriesEntity);
    }

    public async Task<IEnumerable<CategoryDTO>> GetCategoriesProducts()
    {
        var categoriesEntity = await _categoryRepository.GetCategoriesProducts();
        return _mapper.Map<IEnumerable<CategoryDTO>>(categoriesEntity);
    }

    public async Task<CategoryDTO> GetCategoryById(int id)
    {
        var categoryDto = await _categoryRepository.GetById(id);
        return _mapper.Map<CategoryDTO>(categoryDto);

    }

    public async Task AddCategory(CategoryDTO categoryDTO)
    {
        var category = _mapper.Map<Category>(categoryDTO);
        await _categoryRepository.Create(category);
        categoryDTO.CategoryId = category.CategoryId;
    }

    public async Task UpdateCategory(CategoryDTO categoryDTO)
    {
        var category = _mapper.Map<Category>(categoryDTO);
        await _categoryRepository.Update(category);
    }

    public async Task RemoveCategory(int id)
    {
        var category = _categoryRepository.GetById(id).Result;
        await _categoryRepository.Delete(category.CategoryId);
    }
}
