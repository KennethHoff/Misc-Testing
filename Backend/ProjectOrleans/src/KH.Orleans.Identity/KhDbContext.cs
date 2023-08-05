using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KH.Orleans.Identity;

public sealed class KhDbContext(DbContextOptions<KhDbContext> options) : IdentityDbContext<ApplicationUser>(options);
