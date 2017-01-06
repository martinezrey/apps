using Common.Repository;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityProviders.SqlServerProvider
{
    public partial class User : IUser<int>, IDisposable
    {
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

            //Claims.Add(ravenUserClaim);
        }

        public virtual void RemoveClaim(UserClaim ravenUserClaim)
        {
            if (ravenUserClaim == null)
                throw new ArgumentNullException("ravenUserClaim");

            //_claims.Remove(ravenUserClaim);
        }

        public virtual void AddLogin(UserLogin ravenUserLogin)
        {
            if (ravenUserLogin == null)
                throw new ArgumentNullException("ravenUserLogin");

            //_logins.Add(ravenUserLogin);
        }

        public virtual void RemoveLogin(UserLogin ravenUserLogin)
        {
            if (ravenUserLogin == null)
                throw new ArgumentNullException("ravenUserLogin");

            //_logins.Remove(ravenUserLogin);
        }

        public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, int> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }


        //public int Id { get; set; }
        //public string UserName { get; set; }
        //public string Email { get; set; }
        //public string PhoneNumber { get; set; }
        //public string PasswordHash { get; set; }
        //public string SecurityStamp { get; set; }
        //public bool IsLockoutEnabled { get; set; }
        //public bool IsTwoFactorEnabled { get; set; }
        //public int AccessFailedCount { get; set; }
        //public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        //public string Claims { get; set; }
        //public string Logins { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public string Suffix { get; set; }
        //public bool IsActive { get; set; }
        //public bool IsDeleted { get; set; }

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
}
