using System.Linq.Expressions;
using HRManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Api.Data;

public static class ModelBuilderExtensions
{
    public static void ApplySoftDeleteQueryFilters(this ModelBuilder builder)
    {
        var entityTypes = builder.Model.GetEntityTypes()
            .Where(entityType => typeof(BaseEntity).IsAssignableFrom(entityType.ClrType));

        foreach (var entityType in entityTypes)
        {
            var parameter = Expression.Parameter(entityType.ClrType, "entity");
            var isDeleted = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
            var filter = Expression.Lambda(Expression.Not(isDeleted), parameter);

            builder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }
    }
}
