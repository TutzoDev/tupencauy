using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tupencauymobile.Models;

namespace tupencauymobile.Services
{
    public interface IServicioGoogleAuth
    {
        public Task<GoogleUser> AuthenticateAsync();
        public Task<GoogleUser> GetCurrentUserAsync();
        public Task LogoutAsync();
    }
}
