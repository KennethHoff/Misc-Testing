# Khtmx

K(enneth)H(off)TMX - HTMX testing

## Features

- [X] HTMX-based UI
- [X] Server-Sent Events
- [X] Domain-driven design
- [ ] Event sourcing
- [X] CQRS
- [X] Database
  - [X] SQL Server
- [X] ORM
  - [X] EF Core
- [ ] Authentication
  - [ ] ASP.NET Core Identity
- [ ] Authorization
  - [ ] ASP.NET Core Identity
- [ ] Testing
  - [ ] Architecture tests
  - [ ] Unit tests
  - [ ] Integration tests
  - [ ] End-to-end tests
  - [ ] UI tests
- [ ] Telemetry
  - [ ] OpenTelemetry (Logging, Tracing, Metrics)
- [ ] Deployment
  - [ ] Docker
- [ ] CI/CD
  - [ ] GitHub Actions

## What is the Domain?

The domain is a simple blog. It has the following entities:

- [Person](#Person)
  - Someone who can log in and write posts and comments.
  - A person can have zero or more posts.
  - A person can have zero or more comments.

- [Post](#Post)
  - A post is a blog post written by a person.
  - A post can have zero or more tags.
  - A post can have zero or more comments.

- [Comment](#Comment)
  - A comment is a comment written by a person on a post.
  - A comment can be a reply to another comment.

### Person
- [ ] Persons
    - [ ] Create
        - [ ] Username
        - [ ] Password
        - [ ] Email
        - [ ] First name
        - [ ] Last name
        - [ ] Created
    - [ ] Read
        - [ ] Username
        - [ ] Email
        - [ ] First name
        - [ ] Last name
        - [ ] Created
    - [ ] Update
        - [ ] Password
        - [ ] Email
        - [ ] First name
        - [ ] Last name
        - [ ] Modified

### Post
- [ ] Posts
    - [ ] Create
        - [ ] Author (See [Person](#Person))
        - [ ] Title
        - [ ] Content
        - [ ] Tags
        - [ ] Published
    - [ ] Read
        - [ ] Author (See [Person](#Person))
        - [ ] Title
        - [ ] Content
        - [ ] Tags
    - [ ] Update
        - [ ] Title
        - [ ] Content
        - [ ] Tags
        - [ ] Modified


### Comment

- [X] Create
  - [X] Author (See [Person](#Person))
  - [ ] Post (See [Post](#Post))
  - [X] Content
  - [ ] Published
  - [ ] Modified
- [ ] Read
  - [ ] Author (See [Person](#Person))
  - [ ] Content
  - [ ] Published
  - [ ] Modified
- [ ] Update
  - [ ] Content
  - [ ] Modified