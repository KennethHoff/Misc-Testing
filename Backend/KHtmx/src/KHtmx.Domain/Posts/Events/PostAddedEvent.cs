using MediatR;

namespace KHtmx.Domain.Posts.Events;

public sealed record class PostAddedEvent(Post Post) : INotification;
