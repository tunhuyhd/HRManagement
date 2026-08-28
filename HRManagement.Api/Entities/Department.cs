namespace HRManagement.Api.Entities;

public sealed class Department : BaseEntity
{
    public string DepartmentCode { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid? ParentDepartmentId { get; private set; }
    public Department? ParentDepartment { get; private set; }
    public ICollection<Department> ChildDepartments { get; private set; } = new List<Department>();
    public bool IsActive { get; private set; } = true;

    private Department() { }

    public Department(string departmentCode, string name, string? description = null, Guid? parentDepartmentId = null)
    {
        Update(departmentCode, name, description);
        SetParent(parentDepartmentId);
    }

    public void Update(string departmentCode, string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(departmentCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        DepartmentCode = departmentCode.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void SetParent(Guid? parentDepartmentId)
    {
        if (parentDepartmentId == Id)
            throw new InvalidOperationException("A department cannot be its own parent.");
        ParentDepartmentId = parentDepartmentId;
    }

    public void SetActive(bool isActive) => IsActive = isActive;
}
