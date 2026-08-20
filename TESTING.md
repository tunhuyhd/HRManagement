# Testing Guide

This document explains how to run and extend the HR Management test suite.

## Overview

The project uses:

- xUnit as the test framework.
- `Microsoft.NET.Test.Sdk` to run tests from the .NET CLI and Visual Studio.
- `coverlet.collector` to collect code coverage.
- .NET 8 for both the API and test projects.

Test project structure:

```text
HRManagement.Api.Tests/
|-- Entities/
|   `-- DomainEntityTests.cs
`-- HRManagement.Api.Tests.csproj
```

The current suite verifies encapsulation rules and state transitions for:

- `Employee`
- `PasswordResetRequest`
- `RefreshToken`
- `AppUser`
- `AuditLog`

These are in-memory unit tests. They do not require the API or PostgreSQL to be running.

## Prerequisites

Check the installed .NET SDK:

```powershell
dotnet --version
```

.NET SDK 8 or later is required. Both projects target `net8.0`, even when a newer SDK is installed.

## Running Tests

Open a terminal at the repository root:

```powershell
cd G:\TrainingIntern\HRManagement
```

Run every test in the solution:

```powershell
dotnet test HRManagement.slnx
```

From the repository root, the shorter command also works:

```powershell
dotnet test
```

A successful result looks like this:

```text
Passed! - Failed: 0, Passed: 12, Skipped: 0, Total: 12
```

## Filtering Tests

Run every test in one class:

```powershell
dotnet test --filter "FullyQualifiedName~EmployeeTests"
```

Run one test method:

```powershell
dotnet test --filter "Constructor_NormalizesTextAndStartsActive"
```

Run tests whose names contain a keyword:

```powershell
dotnet test --filter "Name~Constructor"
```

Show detailed console output:

```powershell
dotnet test --logger "console;verbosity=detailed"
```

List tests without running them:

```powershell
dotnet test --list-tests
```

## Code Coverage

Run tests and collect coverage:

```powershell
dotnet test HRManagement.slnx --collect:"XPlat Code Coverage"
```

The result is written to:

```text
HRManagement.Api.Tests/TestResults/<test-run-id>/coverage.cobertura.xml
```

The `TestResults` directory is ignored by Git. Current coverage mainly represents entity tests. Overall API coverage will remain low until the suite includes services, controllers, repositories, and database behavior.

## Running Tests in Visual Studio

1. Open `HRManagement.slnx`.
2. Select `Test` > `Test Explorer`.
3. Select `Run All Tests` to run the full suite.
4. Select an individual test and choose `Run` to run it separately.
5. When a test fails, open its result to inspect the exception and expected and actual values.

## Adding Tests

Add tests to `HRManagement.Api.Tests`. The test directory structure should mirror the API project:

```text
HRManagement.Api.Tests/
|-- Entities/
|-- Services/
|-- Controllers/
`-- Repositories/
```

Use this naming convention:

```text
MethodUnderTest_Scenario_ExpectedResult
```

Example:

```csharp
[Fact]
public void Complete_WhenRequestIsPending_StoresCompletionDetails()
{
    // Arrange
    var request = new PasswordResetRequest(Guid.NewGuid());
    var completedBy = Guid.NewGuid();
    var completedAtUtc = DateTime.UtcNow;

    // Act
    request.Complete(completedBy, completedAtUtc);

    // Assert
    Assert.Equal(PasswordResetStatus.Completed, request.Status);
    Assert.Equal(completedBy, request.CompletedBy);
    Assert.Equal(completedAtUtc, request.CompletedAtUtc);
}
```

Each test should follow three stages:

1. Arrange: create the input data and system under test.
2. Act: invoke one primary behavior.
3. Assert: verify the result or expected exception.

## Fact and Theory

Use `[Fact]` for a test with one data set:

```csharp
[Fact]
public void SetActive_WithFalse_DeactivatesUser()
{
    var user = new AppUser();

    user.SetActive(false);

    Assert.False(user.IsActive);
}
```

Use `[Theory]` when the same behavior must be checked with multiple data sets:

```csharp
[Theory]
[InlineData("")]
[InlineData("   ")]
public void Constructor_WithBlankName_Throws(string firstName)
{
    Assert.Throws<ArgumentException>(() => new Employee(
        firstName,
        "Nguyen",
        new DateOnly(1995, 5, 10),
        Gender.Female,
        null,
        null,
        new DateOnly(2024, 1, 15),
        null));
}
```

Each `InlineData` entry is counted as a separate test case.

## Unit Test Principles

- Test public behavior instead of private methods through reflection.
- Give each test one primary reason to fail.
- Do not depend on test execution order.
- Avoid databases, networks, the current clock, and the file system in unit tests where possible.
- Use fixed timestamps when exact comparisons matter.
- Do not share mutable entities between tests.
- Cover successful behavior, invalid input, and invalid state transitions.

## Unit Tests and Integration Tests

The current unit tests instantiate entities directly and run in memory. They are fast and protect domain invariants.

Integration tests verify that multiple components work together, for example:

- EF Core mapping with PostgreSQL.
- HTTP requests through controllers.
- Authentication and authorization.
- Transactions and unique constraints.
- Auditing and soft deletion through `SaveChangesAsync`.

Integration tests require an isolated test database. Never run automated tests against development or production databases.

## When to Add Tests

Add or update tests when:

- Adding an entity or domain method.
- Changing a business rule.
- Fixing a bug; reproduce it with a failing test first when possible.
- Changing authentication, password reset, or refresh-token flows.
- Changing EF Core configuration, auditing, or soft deletion.

## Troubleshooting

Restore missing packages:

```powershell
dotnet restore HRManagement.slnx
```

Run the build separately for clearer compiler errors:

```powershell
dotnet build HRManagement.slnx
```

If a filter finds no tests, list their full names:

```powershell
dotnet test --list-tests
```

Then use a class or method name exactly as displayed.

If Visual Studio does not display the tests, rebuild the solution and reopen Test Explorer:

```powershell
dotnet clean HRManagement.slnx
dotnet build HRManagement.slnx
```

## Recommended Workflow

Before committing or opening a pull request:

```powershell
dotnet build HRManagement.slnx --no-restore
dotnet test HRManagement.slnx --no-build
```

After cloning the repository or changing packages, omit `--no-restore` and `--no-build` so .NET restores and rebuilds the projects.