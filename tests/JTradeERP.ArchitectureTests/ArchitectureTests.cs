using System.Reflection;
using JTradeERP.Application;
using JTradeERP.Domain.Events;
using ApplicationDependencyInjection = JTradeERP.Application.DependencyInjection;
using InfrastructureDependencyInjection = JTradeERP.Infrastructure.DependencyInjection;

namespace JTradeERP.ArchitectureTests;

public sealed class ArchitectureTests
{
    [Fact]
    public void Domain_DoesNotReference_ApplicationInfrastructureOrApi()
    {
        var references = ReferencedAssemblyNames(typeof(IDomainEvent).Assembly);

        Assert.DoesNotContain("JTradeERP.Application", references);
        Assert.DoesNotContain("JTradeERP.Infrastructure", references);
        Assert.DoesNotContain("JTradeERP.Api", references);
    }

    [Fact]
    public void Application_DoesNotReference_InfrastructureOrApi()
    {
        var references = ReferencedAssemblyNames(typeof(ApplicationDependencyInjection).Assembly);

        Assert.DoesNotContain("JTradeERP.Infrastructure", references);
        Assert.DoesNotContain("JTradeERP.Api", references);
    }

    [Fact]
    public void Infrastructure_DoesNotReference_Api()
    {
        var references = ReferencedAssemblyNames(typeof(InfrastructureDependencyInjection).Assembly);

        Assert.DoesNotContain("JTradeERP.Api", references);
    }

    [Fact]
    public void DapperReportImplementation_IsInInfrastructureOnly()
    {
        var applicationReferences = ReferencedAssemblyNames(typeof(ApplicationDependencyInjection).Assembly);
        var infrastructureReferences = ReferencedAssemblyNames(typeof(InfrastructureDependencyInjection).Assembly);

        Assert.DoesNotContain("Dapper", applicationReferences);
        Assert.Contains("Dapper", infrastructureReferences);
    }

    private static HashSet<string> ReferencedAssemblyNames(Assembly assembly) =>
        assembly.GetReferencedAssemblies().Select(reference => reference.Name!).ToHashSet(StringComparer.Ordinal);
}
