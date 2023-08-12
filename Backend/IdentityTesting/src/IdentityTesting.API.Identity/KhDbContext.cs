using IdentityTesting.API.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityTesting.API.Identity;

public sealed class KhDbContext(DbContextOptions<KhDbContext> options) : IdentityDbContext<KhApplicationUser, KhApplicationRole, string>(options);
