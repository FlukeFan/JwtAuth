using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Tmp.FileStore
{
    public class UserStore :
        IUserEmailStore<AuthExUser>,
        IUserPasswordStore<AuthExUser>
    {
        public async Task<IdentityResult> CreateAsync(AuthExUser user, CancellationToken cancellationToken)
        {
            await UsingUsers(users =>
            {
                users.Add(user);
                return Task.CompletedTask;
            });

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(AuthExUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
        }

        public async Task<AuthExUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            AuthExUser user = null;
            await UsingUsers(users => user = users.SingleOrDefault(u => u.NormalizedEmail == normalizedEmail));
            return user;
        }

        public Task<AuthExUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<AuthExUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            AuthExUser user = null;
            await UsingUsers(users => user = users.SingleOrDefault(u => u.NormalizedUserName == normalizedUserName));
            return user;
        }

        public Task<string> GetEmailAsync(AuthExUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(AuthExUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(AuthExUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(AuthExUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(AuthExUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(AuthExUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(AuthExUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(AuthExUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(AuthExUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(AuthExUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(AuthExUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(AuthExUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(AuthExUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(AuthExUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(AuthExUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        private Task UsingUsers(Action<IList<AuthExUser>> usersAction)
        {
            return UsingUsers(users => { usersAction(users); return Task.CompletedTask; });
        }

        private async Task UsingUsers(Func<IList<AuthExUser>, Task> usersAction)
        {
            var users = new List<AuthExUser>();

            if (File.Exists("users.json"))
                users = JsonConvert.DeserializeObject<List<AuthExUser>>(await File.ReadAllTextAsync("users.json"));

            await usersAction(users);

            await File.WriteAllTextAsync("users.json", JsonConvert.SerializeObject(users, Formatting.Indented));
        }
    }
}
