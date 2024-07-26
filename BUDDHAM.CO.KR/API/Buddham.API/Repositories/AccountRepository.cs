using Buddham.API.Data;
using Buddham.SharedLib.Contracts;
using Buddham.SharedLib.DTOs;
using Microsoft.AspNetCore.Identity;
using static Buddham.SharedLib.DTOs.ServiceResponses;

namespace Buddham.API.Repositories;
public class AccountRepository
{
    // TODO

}

// public class AccountRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config) : IUserAccount
// {

//     public async Task<ServiceResponses.LoginResponse> LoginUser(LoginDTO userDTO)
//     {

//     }

//     public async Task<ServiceResponses.GeneralResponse> CreateAccount(UserDTO loginDTO)
//     {
//         if (loginDTO == null)
//         {
//             return new GeneralResponse(false, "Invalid user data");
//         }
//     }
// }
