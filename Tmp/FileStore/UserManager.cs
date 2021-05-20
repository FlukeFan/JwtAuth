using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tmp.FileStore
{
    public class UserManager : UserManager<AuthExUser>
    {
        public UserManager(IUserStore<AuthExUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<AuthExUser> passwordHasher, IEnumerable<IUserValidator<AuthExUser>> userValidators, IEnumerable<IPasswordValidator<AuthExUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AuthExUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public override bool SupportsUserEmail => true;
    }
}
