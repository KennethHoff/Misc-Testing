using MediatR;

namespace KHtmx.Domain.Posts.Events;

public sealed record class PostUpdatedEvent(Post Post) : INotification;
