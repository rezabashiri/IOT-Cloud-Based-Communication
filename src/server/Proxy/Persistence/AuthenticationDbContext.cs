using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ReverseProxy.Persistence;

public class AuthenticationDbContext(
    DbContextOptions options) : IdentityDbContext<IdentityUser>(options)
{
}