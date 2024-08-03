using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Mappers;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Interfaces;
using Sprache;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using static NewsApp.Models.Enum;

namespace NewsApp.Services.Classes
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGoogleOAuthService _googleOAuthService;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IUserRepository userRepository, IGoogleOAuthService googleOAuthService, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _googleOAuthService = googleOAuthService;
            _tokenService = tokenService;
        }
        [ExcludeFromCodeCoverage]
        public async Task<RegisterReturnDTO> CreatePassword(CreatePasswordDTO createPasswordDTO)
        {
            var user = await _userRepository.Get("UserID", createPasswordDTO.userID);
            if(user == null)
            {
                throw new UnableToAddUserException();
            }
            if(user.Password.Length != 0)
            {
                throw new UnableToAddUserException();
            }
            HMACSHA512 hMACSHA = new HMACSHA512();
            user.Password_Hashkey = hMACSHA.Key;
            user.Password = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(createPasswordDTO.Password));
            var result = await _userRepository.Update(user, user.UserID.ToString());
            if (result == null)
            {
                throw new UnableToUpdateItemException();
            }
            return MapCustomerToRegisterReturnDTO(result);
        }

        [ExcludeFromCodeCoverage]
        public async Task<LoginReturnDTO> LoginUser(LoginGetDTO loginGetDTO)
        {
            var payload = await _googleOAuthService.ValidateGoogleTokenAsync(loginGetDTO.oAuthToken);

            if(payload.Subject== null)
            {
                throw new UnauthorizedUserException();
            }
            var user = await _userRepository.Get("Email", loginGetDTO.email);
            if(user == null)
            {
                var user1 = new User
                {
                    Email = payload.Email,
                    Name = payload.Name,
                    OAuthID = payload.Subject,
                    OAuthToken = loginGetDTO.oAuthToken,
                    Role = UserType.Reader,
                    Password = Encoding.UTF8.GetBytes(""),
                    Password_Hashkey = Encoding.UTF8.GetBytes(""),
                };

                var result = await _userRepository.Add(user1);
                if (result.UserID == null)
                {
                    throw new UnableToAddUserException();
                }
                LoginReturnDTO loginReturnDTO = AuthenticationMapper.MapToLoginReturnDTO(result);
                return loginReturnDTO;
            }
            else
            {
                if (user.OAuthToken != loginGetDTO.oAuthToken)
                {
                    user.OAuthToken = loginGetDTO.oAuthToken;
                    var result = await _userRepository.Update(user, user.UserID.ToString());
                    if(result == null)
                    {
                        throw new UnableToUpdateItemException();
                    }
                }
                LoginReturnDTO loginReturnDTO = AuthenticationMapper.MapToLoginReturnDTO(user);
                loginReturnDTO.Token = _tokenService.GenerateToken(user);
                return loginReturnDTO;
            }
        }
        [ExcludeFromCodeCoverage]
        public async Task LogoutUser(string userId)
        {

            var user = await _userRepository.Get("UserID", userId);
            if (user == null)
            {
                throw new UnableToAddUserException();
            }
            user.OAuthToken = null;
            var result = await _userRepository.Update(user, userId);
            if (result == null)
            {
                throw new UnableToUpdateItemException();
            }

        }

        private byte[] EncryptPassword(string password, byte[] passwordHash)
        {
            HMACSHA512 hMACSHA = new HMACSHA512(passwordHash);
            var encrypterPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(password));
            return encrypterPass;
        }

        private bool ComparePassword(byte[] encrypterPass, byte[] password)
        {
            for (int i = 0; i < encrypterPass.Length; i++)
            {
                if (encrypterPass[i] != password[i])
                {
                    return false;
                }
            }
            return true;
        }
        private LoginReturnDTO MapUserToLoginReturn(User user)
        {
            LoginReturnDTO returnDTO = new LoginReturnDTO();
            returnDTO.UserID = user.UserID.ToString();
            returnDTO.Role = user.Role;
            returnDTO.Email = user.Email;
            returnDTO.Name = user.Name;
            returnDTO.Token = _tokenService.GenerateToken(user);
            return returnDTO;
        }

        public async Task<LoginReturnDTO> UserLogin(LoginGetDTO1 loginDTO)
        {
            var user = await _userRepository.Get("Email", loginDTO.Email);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            var encryptedPassword = EncryptPassword(loginDTO.Password, user.Password_Hashkey);
            bool isPasswordSame = ComparePassword(encryptedPassword, user.Password);
            if (isPasswordSame)
            {
                LoginReturnDTO loginReturnDTO = MapUserToLoginReturn(user);
                return loginReturnDTO;
            }
            throw new UnauthorizedUserException();
        }

        public async Task<RegisterReturnDTO> UserRegister(RegisterGetDTO userRegisterDTO)
        {

            var user = await _userRepository.Get("Email", userRegisterDTO.Email);
            if(user != null)
            {
                throw new UserAlreadyExistsException();
            }
            HMACSHA512 hMACSHA = new HMACSHA512();
            User newuser = new User()
            {
                Name = userRegisterDTO.Name,
                Email = userRegisterDTO.Email,
                Role = UserType.Reader,
                Password_Hashkey = hMACSHA.Key,
                Password = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(userRegisterDTO.Password)),
                OAuthID = string.Empty,
                OAuthToken = string.Empty,  
            };
            var data = await _userRepository.Add(newuser);

            if (data.UserID == 0)
            {
                throw new UnableToRegisterException();
            }
            RegisterReturnDTO registerReturnDTO = MapCustomerToRegisterReturnDTO(data);
            return registerReturnDTO;
        }

        private RegisterReturnDTO MapCustomerToRegisterReturnDTO(User user)
        {
            RegisterReturnDTO registerReturnDTO = new RegisterReturnDTO();
            registerReturnDTO.UserID = user.UserID.ToString();
            registerReturnDTO.Name = user.Name;
            registerReturnDTO.Email = user.Email;
            return registerReturnDTO;
        }
    }
}
