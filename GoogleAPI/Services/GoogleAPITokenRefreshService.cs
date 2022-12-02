using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Google.Apis.Auth.OAuth2;

namespace GoogleAPI.Services
{
    public class GoogleAPITokenRefreshService
    {
        public void RefreshToken(UserCredential credential)
        {
            credential.RefreshTokenAsync(CancellationToken.None);
        }
    }
}
