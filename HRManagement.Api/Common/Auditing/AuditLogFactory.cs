using System.Text.Json;
using HRManagement.Api.Common.Auth;
using HRManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HRManagement.Api.Common.Auditing;

public static class AuditLogFactory
{
    private static readonly HashSet<string> IgnoredProperties =
    [
        nameof(BaseEntity.CreatedAtUtc),
        nameof(BaseEntity.LastModifiedBy),
        nameof(BaseEntity.LastModifiedAtUtc),
        nameof(BaseEntity.DeletedBy),
        nameof(BaseEntity.DeletedAtUtc)
    ];

    public static IReadOnlyList<AuditLog> Create(
        IEnumerable<EntityEntry<BaseEntity>> entries,
        ICurrentUser currentUser,
        DateTime changedAtUtc)
    {
        var auditLogs = new List<AuditLog>();

        foreach (var entry in entries)
        {
            var changedProperties = entry.Properties
                .Where(property => !IgnoredProperties.Contains(property.Metadata.Name))
                .Where(property => entry.State != EntityState.Modified || property.IsModified)
                .ToArray();

            if (entry.State == EntityState.Modified && changedProperties.Length == 0)
            {
                continue;
            }

            var oldValues = entry.State is EntityState.Modified or EntityState.Deleted
                ? changedProperties.ToDictionary(
                    property => property.Metadata.GetColumnName(),
                    property => property.OriginalValue)
                : null;

            var newValues = entry.State is EntityState.Added or EntityState.Modified
                ? changedProperties.ToDictionary(
                    property => property.Metadata.GetColumnName(),
                    property => property.CurrentValue)
                : null;

            auditLogs.Add(new AuditLog(
                entry.Metadata.GetTableName() ?? entry.Metadata.ClrType.Name,
                GetRecordId(entry),
                IsSoftDelete(entry) ? "Deleted" : entry.State.ToString(),
                JsonSerializer.Serialize(
                    changedProperties.Select(property => property.Metadata.GetColumnName())),
                oldValues is null ? null : JsonSerializer.Serialize(oldValues),
                newValues is null ? null : JsonSerializer.Serialize(newValues),
                currentUser.Id,
                currentUser.Email,
                changedAtUtc));
        }

        return auditLogs;
    }

    private static bool IsSoftDelete(EntityEntry<BaseEntity> entry) =>
        entry.State == EntityState.Modified &&
        entry.Property(nameof(BaseEntity.IsDeleted)).IsModified &&
        entry.Entity.IsDeleted;

    private static string GetRecordId(EntityEntry<BaseEntity> entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key is null)
        {
            return entry.Entity.Id.ToString();
        }

        return string.Join(
            ",",
            key.Properties.Select(property =>
            {
                var value = entry.Property(property.Name).CurrentValue
                    ?? entry.Property(property.Name).OriginalValue;
                return value?.ToString() ?? string.Empty;
            }));
    }
}
