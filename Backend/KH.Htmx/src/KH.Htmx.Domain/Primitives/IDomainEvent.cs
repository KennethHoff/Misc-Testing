using System.Text.Json.Serialization;
using KH.Htmx.Domain.Comments.Events;
using MediatR;

namespace KH.Htmx.Domain.Primitives;

// I cringe a little at how the [JsonDerivedType] attribute works,
// but if I understood the dotnet team correctly, they did it this way for both performance and security reasons
// (https://github.com/dotnet/runtime/issues/63747).
[JsonDerivedType(typeof(CommentCreatedDomainEvent), "CommentCreated")]
public interface IDomainEvent : INotification;
