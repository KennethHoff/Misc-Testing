using MediatR;

namespace KHtmx.Domain.Posts.Events;

public sealed record class PostDeletedEvent(Post Post) : INotification;
