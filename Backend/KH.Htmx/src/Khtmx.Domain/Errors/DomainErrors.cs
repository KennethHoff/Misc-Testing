using Khtmx.Domain.Shared;

namespace Khtmx.Domain.Errors;

public static class DomainErrors
{
    public static class Person
    {
        public static Error NotFound => new("Person.NotFound", "The specified person was not found.");
    }
}
