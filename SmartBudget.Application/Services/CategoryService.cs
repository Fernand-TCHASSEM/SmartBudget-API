using Microsoft.Extensions.Logging;
using SmartBudget.Application.DTOs.Category;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.Application.Services;

public class CategoryService(
    ICategoryRepository categoryRepository,
    ILogger<CategoryService> logger
)
{
    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(string userId, CancellationToken ct = default)
    {
        var categories = await categoryRepository.GetAllForUserAsync(userId, ct);
        return categories.Select(MapToResponse);
    }

    public async Task<PagedResponse<CategoryResponse>> GetPagedAsync(string userId, CategoryQuery query, CancellationToken ct = default)
    {
        var result = await categoryRepository.GetPagedForUserAsync(
            userId, query, query.IsIncome, query.IsDefault, ct);

        return result.Map(MapToResponse);
    }

    public async Task<CategoryResponse?> GetByIdAsync(string categoryId, CancellationToken ct = default)
    {
        var category = await categoryRepository.FindByAsync(c => c.Id == categoryId, ct);
        return category is null ? null : MapToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(string userId, CreateCategoryRequest request, CancellationToken ct = default)
    {
        var category = await categoryRepository.AddAsync(new Category
        {
            Name      = request.Name,
            UserId    = userId,
            Icon      = request.Icon,
            Color     = request.Color,
            IsIncome  = request.IsIncome,
            SortOrder = request.SortOrder
        }, ct);

        return MapToResponse(category);
    }

    public async Task<CategoryResponse?> UpdateAsync(string categoryId, UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var category = await categoryRepository.FindByAsync(c => c.Id == categoryId, ct);
        if (category is null)
        {
            logger.LogWarning("Category not found: {CategoryId}", categoryId);
            return null;
        }

        category.Name      = request.Name;
        category.Icon      = request.Icon;
        category.Color     = request.Color;
        category.IsIncome  = request.IsIncome;
        category.SortOrder = request.SortOrder;

        await categoryRepository.UpdateAsync(category, ct);
        return MapToResponse(category);
    }

    public async Task<bool> DeleteAsync(string categoryId, CancellationToken ct = default)
    {
        var category = await categoryRepository.FindByAsync(c => c.Id == categoryId, ct);
        if (category is null)
        {
            logger.LogWarning("Category not found: {CategoryId}", categoryId);
            return false;
        }

        await categoryRepository.DeleteAsync(category, ct);
        return true;
    }

    private static CategoryResponse MapToResponse(Category c) => new(
        Id:        c.Id,
        UserId:    c.UserId,
        Name:      c.Name,
        Icon:      c.Icon,
        Color:     c.Color,
        IsDefault: c.IsDefault,
        IsIncome:  c.IsIncome,
        SortOrder: c.SortOrder,
        CreatedAt: c.CreatedAt,
        UpdatedAt: c.UpdatedAt,
        DeletedAt: c.DeletedAt
    );
}
