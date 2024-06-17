using DsShop.IdentityServer.Data;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DsShop.IdentityServer.Services;

public class ProfileAppService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;

    public ProfileAppService(UserManager<ApplicationUser> userManager,
                             RoleManager<IdentityRole> roleManager,
                             IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        // Id de usuário no IS
        string id = context.Subject.GetSubjectId();

        ApplicationUser user = await _userManager.FindByIdAsync(id);

        // Cria ClaimsPrincipal para o usuário
        ClaimsPrincipal userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);


        List<Claim> claims = userClaims.Claims.ToList();
        claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
        claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));


        if (_userManager.SupportsUserRole)
        {
            // Obtém a lista dos nomes das roles para o usuario
            IList<string> roles = await _userManager.GetRolesAsync(user);

            foreach (string role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));

                if (_roleManager.SupportsRoleClaims)
                {
                    //localiza o perfil
                    IdentityRole identityRole = await _roleManager.FindByNameAsync(role);

                    //inclui o perfil
                    if (identityRole != null)
                    {
                        //inclui as claims associada com a role
                        claims.AddRange(await _roleManager.GetClaimsAsync(identityRole));
                    }
                }
            }
        }

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        // Id de usuário no IS
        string id = context.Subject.GetSubjectId();

        ApplicationUser user = await _userManager.FindByIdAsync(id);

        context.IsActive = user is not null;
    }
}
