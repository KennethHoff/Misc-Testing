using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KH.Orleans.API.Identity;

public sealed class KhDbContext(DbContextOptions<KhDbContext> options) : IdentityDbContext<KhApplicationUser, KhApplicationRole, string>(options);
