using Buddham.SharedLib.DTOs;
using static Buddham.SharedLib.DTOs.ServiceResponses;

namespace Buddham.SharedLib.Contracts;

public interface IUserAccount
{
    Task<GeneralResponse> RegisterUser(UserDTO userDTO);
    Task<LoginResponse> LoginUser(SignInDTO loginDTO);
}
