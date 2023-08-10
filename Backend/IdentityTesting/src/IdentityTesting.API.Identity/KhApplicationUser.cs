using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityTesting.API.Identity;

public sealed class KhApplicationUser : IdentityUser;

public sealed class KhApplicationRole : IdentityRole;

public sealed class KhUserManager(IUserStore<KhApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<KhApplicationUser> passwordHasher,
        IEnumerable<IUserValidator<KhApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<KhApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<KhApplicationUser>> logger)
    : AspNetUserManager<KhApplicationUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators,
        keyNormalizer, errors, services, logger);

public sealed class KhRoleManager(IRoleStore<KhApplicationRole> store,
        IEnumerable<IRoleValidator<KhApplicationRole>> roleValidators, ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors, ILogger<RoleManager<KhApplicationRole>> logger,
        IHttpContextAccessor contextAccessor)
    : AspNetRoleManager<KhApplicationRole>(store, roleValidators, keyNormalizer, errors, logger, contextAccessor);

public sealed class KhSignInManager(UserManager<KhApplicationUser> userManager, IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<KhApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<KhApplicationUser>> logger, IAuthenticationSchemeProvider schemes,
        IUserConfirmation<KhApplicationUser> confirmation)
    : SignInManager<KhApplicationUser>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes,
        confirmation);
