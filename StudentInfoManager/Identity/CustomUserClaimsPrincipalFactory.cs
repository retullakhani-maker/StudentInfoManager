using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.Model
{
    public class CustomUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public CustomUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor) { }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // ✅ Add custom claims
            if (!string.IsNullOrEmpty(user.FullName))
                identity.AddClaim(new Claim(CustomClaimTypes.FullName, user.FullName));

            if (!string.IsNullOrEmpty(user.City))
                identity.AddClaim(new Claim(CustomClaimTypes.City, user.City));

            if (!string.IsNullOrEmpty(user.State))
                identity.AddClaim(new Claim(CustomClaimTypes.State, user.State));

            return identity;
        }
    }
}
