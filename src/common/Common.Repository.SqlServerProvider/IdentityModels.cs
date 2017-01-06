using Common.Repository;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repository.SqlServerProvider
{
    public partial class SystemUser : IUser<int>, IEntity<int>, IDisposable
    {
        public SystemUser()
        {
            this.Roles = new HashSet<string>();
        }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        private List<UserClaim> _claims;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public IEnumerable<UserClaim> Claimsss
        {
            get
            {
                return _claims;
            }

            private set
            {
                if (_claims == null)
                    _claims = new List<UserClaim>();

                if (value != null)
                    _claims.AddRange(value);
            }
        }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        protected List<UserLogin> _logins;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public IEnumerable<UserLogin> Loginssssss
        {
            get
            {
                return _logins;
            }

            private set
            {
                if (_logins == null)
                    _logins = new List<UserLogin>();

                if (value != null)
                    _logins.AddRange(value);
            }
        }

        public virtual void EnableTwoFactorAuthentication()
        {
            IsTwoFactorEnabled = true;
        }

        public virtual void DisableTwoFactorAuthentication()
        {
            IsTwoFactorEnabled = false;
        }

        public virtual void EnableLockout()
        {
            IsLockoutEnabled = true;
        }

        public virtual void DisableLockout()
        {
            IsLockoutEnabled = false;
        }

        public virtual void SetEmail(string email)
        {
            Email = email;
        }

        public virtual void SetPhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }

        public virtual void SetPasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
        }

        public virtual void SetSecurityStamp(string securityStamp)
        {
            SecurityStamp = securityStamp;
        }

        public virtual void IncrementAccessFailedCount()
        {
            AccessFailedCount++;
        }

        public virtual void ResetAccessFailedCount()
        {
            AccessFailedCount = 0;
        }

        public virtual void LockUntil(DateTimeOffset lockoutEndDate)
        {
            LockoutEndDateUtc = lockoutEndDate.DateTime;
        }

        public virtual void AddClaim(Claim claim)
        {
            if (claim == null)
                throw new ArgumentNullException("claim");

            AddClaim(new UserClaim(claim));
        }

        public virtual void AddClaim(UserClaim ravenUserClaim)
        {
            if (ravenUserClaim == null)
                throw new ArgumentNullException("ravenUserClaim");

            _claims.Add(ravenUserClaim);
        }

        public virtual void RemoveClaim(UserClaim ravenUserClaim)
        {
            if (ravenUserClaim == null)
                throw new ArgumentNullException("ravenUserClaim");

            _claims.Remove(ravenUserClaim);
        }

        public virtual void AddLogin(UserLogin ravenUserLogin)
        {
            if (ravenUserLogin == null)
                throw new ArgumentNullException("ravenUserLogin");

            _logins.Add(ravenUserLogin);
        }

        public virtual void RemoveLogin(UserLogin ravenUserLogin)
        {
            if (ravenUserLogin == null)
                throw new ArgumentNullException("ravenUserLogin");

            _logins.Remove(ravenUserLogin);
        }

        public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<SystemUser, int> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }


        public int Id
        {
            get
            {
                return this.SystemUserID;
            }
            set
            {
                this.SystemUserID = value;
            }
        }
        public string UserName
        {
            get
            {
                return this.DomainLogin;
            }
            set
            {
                this.DomainLogin = value;
            }
        }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public bool IsLockoutEnabled { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public string Claims { get; set; }
        public string Logins { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<string> Roles { get; set; }

        public void Dispose()
        {

        }
    }

    public class UserLogin
    {
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }

        public UserLogin() : base() { }

        public UserLogin(string userId, string loginProvider, string providerKey)
            : this(userId, new UserLoginInfo(loginProvider, providerKey))
        {
        }

        public UserLogin(string userId, UserLoginInfo loginInfo)
        {
            if (userId == null) throw new ArgumentNullException("userId");
            if (loginInfo == null) throw new ArgumentNullException("loginInfo");

            //Id = GenerateKey(loginInfo.LoginProvider, loginInfo.ProviderKey);
            UserId = userId;
            LoginProvider = loginInfo.LoginProvider;
            ProviderKey = loginInfo.ProviderKey;
        }
    }

    public class UserClaim
    {
        public string ClaimType { get; private set; }
        public string ClaimValue { get; private set; }

        public UserClaim(Claim claim)
        {
            if (claim == null) throw new ArgumentNullException("claim");

            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }

        public UserClaim(string claimType, string claimValue)
        {
            if (claimType == null) throw new ArgumentNullException("claimType");
            if (claimValue == null) throw new ArgumentNullException("claimValue");

            ClaimType = claimType;
            ClaimValue = claimValue;
        }
    }

    public class ConfirmationRecord
    {
        public DateTimeOffset ConfirmedOn { get; set; }

        public ConfirmationRecord()
            : this(DateTimeOffset.UtcNow)
        {
        }

        public ConfirmationRecord(DateTimeOffset confirmedOn)
        {
            ConfirmedOn = confirmedOn;
        }
    }

    public class UserEmail
    {
        public string UserId { get; set; }
        public string Email { get; set; }

        public UserEmail() : base() { }

        public UserEmail(string email, string userId)
        {
            if (email == null) throw new ArgumentNullException("email");
            if (userId == null) throw new ArgumentNullException("userId");

            //Id = GenerateKey(email);
            UserId = userId;
            Email = email;
        }

        public ConfirmationRecord ConfirmationRecord { get; set; }

        public void SetConfirmed()
        {
            if (ConfirmationRecord == null)
            {
                ConfirmationRecord = new ConfirmationRecord();
            }
        }

        public void SetUnconfirmed()
        {
            ConfirmationRecord = null;
        }
    }

    public class UserPhoneNumber
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public ConfirmationRecord ConfirmationRecord { get; set; }

        public UserPhoneNumber() : base() { }

        public UserPhoneNumber(string phoneNumber, string userId)
        {
            if (phoneNumber == null) throw new ArgumentNullException("phoneNumber");

            //Id = GenerateKey(phoneNumber);
            UserId = userId;
            PhoneNumber = phoneNumber;
        }

        public void SetConfirmed()
        {
            if (ConfirmationRecord == null)
            {
                ConfirmationRecord = new ConfirmationRecord();
            }
        }

        public void SetUnconfirmed()
        {
            ConfirmationRecord = null;
        }
    }

    public class SqlServerUserStore : //IUserRoleStore<SqlUser>,
        IUserStore<SystemUser, int>,
        //IUserLoginStore<User, int>,
        //IUserClaimStore<SqlUser, int>,
        IUserPasswordStore<SystemUser, int>,
        IUserSecurityStampStore<SystemUser, int>,
        IQueryableUserStore<SystemUser, int>,
        //IUserTwoFactorStore<SqlUser, int>,
        //IUserLockoutStore<User, int>,
        IUserEmailStore<SystemUser, int>,
        IDisposable
    {
        RepoBase<SystemUser> _userRepo;

        public SqlServerUserStore(RepoBase<SystemUser> userRepo)
        {
            _userRepo = userRepo;
        }


        /*IQueryableUserStore*/

        public IQueryable<SystemUser> Users
        {
            get
            {
                return (IQueryable<SystemUser>)_userRepo.Table.AsQueryable<SystemUser>();
            }
        }

        /*IUserStore*/

        public async Task CreateAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.DomainLogin == null)
            {
                throw new InvalidOperationException("Cannot create user as the 'UserName' property is null on user parameter.");
            }

            //user.Id = userRepository.GetKey(user.Email);

            await _userRepo.CreateAsync(user);

            //user.Id = userRepository.GetKey(user.Email);

            //user.Id = userRepository.GetKey(user.Email);
            //await emailRepository.SaveAsync(new CouchbaseUserEmail
            //    {
            //        Email = user.Email,
            //        UserId = user.UserName,
            //        ConfirmationRecord = new ConfirmationRecord
            //        {
            //             ConfirmedOn = DateTime.UtcNow,
            //        },
            //        Id = user.Email
            //    });
            //await phoneNumberRepository.SaveAsync(new CouchbaseUserPhoneNumber
            //    {
            //        UserId = user.Id,
            //        PhoneNumber = user.PhoneNumber
            //    });
            //await _couchbaseClient.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<SystemUser> FindByIdAsync(int userId)
        {
            return await _userRepo.FindAsync(userId);
        }

        public Task<SystemUser> FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<SystemUser> FindByNameAsync(string userName)
        {
            var items = _userRepo.Entities.Where(i => i.DomainLogin.Equals("KainT")).ToList();
            var item = _userRepo.Entities.FirstOrDefault(i => i.DomainLogin == userName);
            var itemmmmmmmmmmmmms = _userRepo.Entities.FirstOrDefault(i => i.DomainLogin.ToLower().Equals(userName.ToLower()));

            return Task.FromResult<SystemUser>(_userRepo.Entities.FirstOrDefault(i => i.DomainLogin.ToLower().Equals(userName.ToLower())));
        }

        /// <remarks>
        /// This method assumes that incomming User parameter is tracked in the session. So, this method literally behaves as SaveChangeAsync
        /// </remarks>
        public async Task UpdateAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            await _userRepo.UpdateAsync(user);
        }

        public Task DeleteAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return _userRepo.DeleteAsync(user);
        }

        /*IUserPasswordStore*/

        public Task<string> GetPasswordHashAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<string>(String.Empty);//(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<bool>(true);//(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(SystemUser user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.SetPasswordHash(passwordHash);
            return Task.FromResult(0);
        }

        /*IUserSecurityStampStore*/

        public Task<string> GetSecurityStampAsync(SystemUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult<string>(user.SecurityStamp.ToString());
        }

        public Task SetSecurityStampAsync(SystemUser user, string stamp)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        /*IUserEmailStore*/

        public async Task<SystemUser> FindByEmailAsync(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException("email");
            }

            SystemUser user = _userRepo.Entities.FirstOrDefault(i => i.EmailAddress.ToLower().Equals(email));

            if (user == null)
                return default(SystemUser);

            return await Task.FromResult<SystemUser>(user);
        }

        public async Task<string> GetEmailAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return await Task.FromResult(user.Email);
        }

        public async Task<bool> GetEmailConfirmedAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.Email == null)
            {
                throw new InvalidOperationException("Cannot get the confirmation status of the e-mail because user doesn't have an e-mail.");
            }

            //ConfirmationRecord confirmation = await GeUserEmailConfirmationAsync(user.Email)
            //    .ConfigureAwait(false);

            return await Task.FromResult<bool>(false);// confirmation != null;
        }

        public Task SetEmailAsync(SystemUser user, string email)
        {
            user.Email = email;

            return _userRepo.UpdateAsync(user);
        }

        public async Task SetEmailConfirmedAsync(SystemUser user, bool confirmed)
        {
            await Task.FromResult<int>(0);
            //if (user == null)
            //{
            //    throw new ArgumentNullException("user");
            //}

            //if (user.Email == null)
            //{
            //    throw new InvalidOperationException("Cannot set the confirmation status of the e-mail because user doesn't have an e-mail.");
            //}

            //UserEmail userEmail = await GeUserEmailAsync(user.Email).ConfigureAwait(false);
            //if (userEmail == null)
            //{
            //    throw new InvalidOperationException("Cannot set the confirmation status of the e-mail because user doesn't have an e-mail as RavenUserEmail document.");
            //}

            //if (confirmed)
            //{
            //    userEmail.SetConfirmed();
            //}
            //else
            //{
            //    userEmail.SetUnconfirmed();
            //}
        }

        protected void Dispose(bool disposing)
        {
            //if (_disposeDocumentSession && disposing && _couchbaseClient != null)
            //{
            //    _couchbaseClient.Dispose();
            //}
        }

        public void Dispose()
        {
            //Dispose(true);
            //GC.SuppressFinalize(this);
        }
    }
}