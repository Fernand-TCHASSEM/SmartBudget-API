using Microsoft.Extensions.Logging;
using SmartBudget.Application.DTOs.CategoryRule;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.Application.Services;

public class CategoryRuleService(
    ICategoryRuleRepository categoryRuleRepository,
    ILogger<CategoryRuleService> logger
)
{
    public async Task<CategoryRuleResponse?> GetByIdAsync(string ruleId, CancellationToken ct = default)
    {
        var rule = await categoryRuleRepository.FindByAsync(r => r.Id == ruleId, ct);
        return rule is null ? null : MapToResponse(rule);
    }

    public async Task<PagedResponse<CategoryRuleResponse>> GetPagedAsync(string userId, string categoryId, PaginationFilter query, CancellationToken ct = default)
    {
        var result = await categoryRuleRepository.GetPagedForCategoryAsync(
            userId, categoryId, query, ct);

        return result.Map(MapToResponse);
    }

    public async Task<CategoryRuleResponse> CreateAsync(string userId, string categoryId, CreateCategoryRuleRequest request, CancellationToken ct = default)
    {
        var rule = await categoryRuleRepository.AddAsync(new CategoryRule
        {
            Name         = request.Name,
            UserId       = userId,
            CategoryId   = categoryId,
            Keyword      = request.Keyword,
            IsRegex      = request.IsRegex,
            Priority     = request.Priority
        }, ct);

        return MapToResponse(rule);
    }

    public async Task<bool> DeleteAsync(string ruleId, CancellationToken ct = default)
    {
        var rule = await categoryRuleRepository.FindByAsync(r => r.Id == ruleId, ct);
        if (rule is null) return false;

        await categoryRuleRepository.DeleteAsync(rule, ct);
        return true;
    }

    private static CategoryRuleResponse MapToResponse(CategoryRule cr) => new(
        Id:           cr.Id,
        UserId:       cr.UserId,
        CategoryId:   cr.CategoryId,
        Name:         cr.Name,
        Keyword:      cr.Keyword,
        IsRegex:      cr.IsRegex,
        Priority:     cr.Priority,
        Source:       cr.Source, 
        CreatedAt:    cr.CreatedAt,
        DeletedAt:    cr.DeletedAt
    );
}
