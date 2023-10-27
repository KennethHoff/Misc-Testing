// TODO: https://www.youtube.com/watch?v=_D6Kai4RdGY&list=PLYpjLpq5ZDGstQ5afRz-34o_0dexr1RGa&index=2

// using Xunit;
// using Assembly = System.Reflection.Assembly;
//
// namespace Architecture.Tests;
//
// public class ArchitectureTests
// {
//     private const string RootNamespace = "Khtmx";
//     private const string DomainNamespace = RootNamespace + ".Domain";
//     private const string ApplicationNamespace = RootNamespace + ".Application";
//     private const string InfrastructureNamespace = RootNamespace + ".Infrastructure";
//     private const string PersistenceNamespace = RootNamespace + ".Persistence";
//     private const string PresentationNamespace = RootNamespace + ".Presentation";
//     private const string WebNamespace = RootNamespace + ".Web";
//
//     private static readonly Assembly DomainAssembly = typeof(Khtmx.Domain.AssemblyReference).Assembly;
//     private static readonly Assembly ApplicationAssembly = typeof(Khtmx.Application.AssemblyReference).Assembly;
//     private static readonly Assembly InfrastructureAssembly = typeof(Khtmx.Infrastructure.AssemblyReference).Assembly;
//     private static readonly Assembly PersistenceAssembly = typeof(Khtmx.Persistence.AssemblyReference).Assembly;
//     private static readonly Assembly PresentationAssembly = typeof(Khtmx.Presentation.AssemblyReference).Assembly;
//     private static readonly Assembly WebAssembly = typeof(Khtmx.Web.AssemblyReference).Assembly;
//
//     [Fact]
//     public void DomainLayer_ShouldNotHaveDependencyOnAnyOtherLayer()
//     {
//         // Arrange
//         var otherProjects = [
//             ApplicationNamespace,
//             InfrastructureNamespace,
//             PersistenceNamespace,
//             PresentationNamespace,
//             WebNamespace
//             ];
//
//         // Act
//         Types().That().ResideInNamespace(DomainNamespace)
//             .Should().NotDependOnAny(otherProjects)
//             .Check(DomainAssembly);
//
//
//         // Assert
//     }
// }
