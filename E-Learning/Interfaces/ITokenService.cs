using E_Learning.DTOs;
using E_Learning.Models;

namespace E_Learning.Interfaces;

public interface ITokenService
{
    string CreateToken(User user, IList<string> roles);
}