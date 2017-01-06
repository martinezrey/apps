using Common.Repository;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityProviders.SqlServerProvider
{
    public class ClaimsGenerator
    {
        private ClaimsIdentity _oAuthIdentity;
        private User _user;
        private OrgUserRepo _orgUserRepo;
        private GlobalUserRoleRepo _orgGlobalUserRoleRepo;
        private OrgUserRoleRepo _orgUserRoleRepo;
        private OrgAppUserAuthIpRepo _orgAppUserAuthIpRepo;
        private OrgAppUserRoleRepo _orgAppUserRoleRepo;
        private OrgAppUserMetadataRepo _orgAppUserMetadata;
        public ClaimsGenerator(User user,
                                        ClaimsIdentity aAuthIdentity,
                                        IRepository<OrgUser> orgUserRepo,
                                        IRepository<OrgUserRole> orgUserRoleRepo,
                                        IRepository<OrgGlobalUserRole> orgGlobalUserRoleRepo,
                                        IRepository<OrgAppUserAuthIp> orgAppUserAuthIpRepo,
                                        IRepository<OrgAppUserRole> orgAppUserRoleRepo,
                                        IRepository<OrgAppUserMetadata> orgAppUserMetadata)
        {
            _orgGlobalUserRoleRepo = (GlobalUserRoleRepo)orgGlobalUserRoleRepo;
            _orgUserRepo = (OrgUserRepo)orgUserRepo;
            _orgUserRoleRepo = (OrgUserRoleRepo)orgUserRoleRepo;
            _orgAppUserAuthIpRepo = (OrgAppUserAuthIpRepo)orgAppUserAuthIpRepo;
            _orgAppUserRoleRepo = (OrgAppUserRoleRepo)orgAppUserRoleRepo;
            _user = user;
            _oAuthIdentity = aAuthIdentity;
            _orgAppUserMetadata = (OrgAppUserMetadataRepo)orgAppUserMetadata;
        }
        public async Task GenerateOrgUsersAsync()
        {
            var orgUsers = await _orgUserRepo.GetAsync(_user.Id);
            _oAuthIdentity.AddClaim(new Claim("orgUsers", JsonConvert.SerializeObject(orgUsers.Select(i => new
            {
                OrgnName = i.Org.OrgName,
                UserName = i.User.UserName,
                IsActive = !i.Org.IsActive ? false : i.IsActive,
                IsDeleted = i.Org.IsDeleted ? true : i.IsDeleted
            }))));
        }
        public async Task GenerateGlobalUserRolesAsync()
        {
            var userGlobalRoles = await _orgGlobalUserRoleRepo.GetAsync(_user.Id);
            var items = userGlobalRoles.GroupBy(gur => new
            {
                gur.OrgUser.Org.OrgName,
                gur.OrgUser.Org.IsActive,
                gur.OrgUser.Org.IsDeleted,
            })
            .Select(gur => new
            {
                gur.Key.OrgName,
                gur.Key.IsActive,
                gur.Key.IsDeleted,
                GlobalRoles = gur.Select(i => new
                {
                    Name = i.OrgGlobalRole.Name,
                    IsActive = !i.OrgGlobalRole.IsActive ? false : i.IsActive,
                    IsDeleted = i.OrgGlobalRole.IsDeleted ? true : i.IsDeleted
                }).ToList()
            }).ToList();
            _oAuthIdentity.AddClaim(new Claim("orgGlobalRoles", JsonConvert.SerializeObject(items)));
        }
        public async Task GenerateOrgAppUserRolesAsync()
        {
            var userOrgAppRoles = await _orgAppUserRoleRepo.GetAsync(_user.Id);
            var items = userOrgAppRoles.GroupBy(oaur => new
            {
                oaur.OrgAppUser.OrgApp.Org.OrgName,
                IsActive = oaur.OrgAppUser.OrgApp.Org.IsActive,
                IsDeleted = oaur.OrgAppUser.OrgApp.Org.IsDeleted,
            }).Select(oaur => new
            {
                oaur.Key.OrgName,
                oaur.Key.IsActive,
                oaur.Key.IsDeleted,
                Apps = oaur.GroupBy(i => new
                {
                    i.OrgAppUser.OrgApp.AppName,
                    IsActive = !i.OrgAppUser.OrgApp.IsActive ? false : i.OrgAppUser.IsActive,
                    IsDeleted = i.OrgAppUser.OrgApp.IsDeleted ? true : i.OrgAppUser.IsDeleted
                }).Select(ar => new
                {
                    ar.Key.AppName,
                    ar.Key.IsActive,
                    ar.Key.IsDeleted,
                    Roles = ar.Select(role => new
                    {
                        Name = role.OrgAppRole.Name,
                        IsActive = !role.OrgAppRole.IsActive ? false : role.IsActive,
                        IsDeleted = role.OrgAppRole.IsDeleted ? true : role.IsDeleted
                    }).ToList(),
                }).ToList(),
            }).ToList();
            //http://stackoverflow.com/questions/14654056/group-list-of-objects-based-on-property-using-linq
            _oAuthIdentity.AddClaim(new Claim("orgAppRoles", JsonConvert.SerializeObject(items)));
        }
        public async Task GenerateUserAuthorizedIpsAsync()
        {
            var userAuthorizedIps = await _orgAppUserAuthIpRepo.GetAsync(_user.Id);
            var items = userAuthorizedIps.GroupBy(oauai => new
            {
                oauai.OrgAppUser.OrgApp.Org.OrgName,
                IsActive = oauai.OrgAppUser.OrgApp.Org.IsActive,
                IsDeleted = oauai.OrgAppUser.OrgApp.Org.IsDeleted,
            })
                    .Select(oauai => new
                    {
                        oauai.Key.OrgName,
                        IsOrgActive = oauai.Key.IsActive,
                        IsOrgDelted = oauai.Key.IsDeleted,
                        OrgApps = oauai.GroupBy(i => new
                        {
                            i.OrgAppUser.OrgApp.AppName,
                            IsActive = !i.OrgAppUser.OrgApp.IsActive ? false : i.OrgAppUser.IsActive,
                            IsDeleted = i.OrgAppUser.OrgApp.IsDeleted ? true : i.OrgAppUser.IsDeleted
                        }).Select(i => new
                        {
                            i.Key.AppName,
                            i.Key.IsActive,
                            i.Key.IsDeleted,
                            Ips = i.Select(x => new
                            {
                                x.Ip,
                                IsActive = x.IsActive,
                                isDeleted = x.IsDeleted,
                            }).ToList()
                        }).ToList()
                    }).ToList();
            _oAuthIdentity.AddClaim(new Claim("orgAppAuthorizedIps", JsonConvert.SerializeObject(items)));
        }
        public async Task GenerateOrganizationUserRolesAsync()
        {
            var orgUserRoles = await _orgUserRoleRepo.GetAsync(_user.Id);
            var items = orgUserRoles.GroupBy(i => new
            {
                i.OrgUser.Org.OrgName,
                i.OrgUser.Org.IsActive,
                i.OrgUser.Org.IsDeleted,
            })
                    .Select(i => new
                    {
                        i.Key.OrgName,
                        i.Key.IsActive,
                        i.Key.IsDeleted,
                        OrgRoles = i.Select(our => new
                        {
                            our.OrgRole.Name,
                            IsActive = !our.OrgRole.IsActive ? false : our.IsActive,
                            IsDeleted = our.OrgRole.IsDeleted ? true : our.IsDeleted,
                        }).ToList()
                    }).ToList();
            _oAuthIdentity.AddClaim(new Claim("orgUserRoles", JsonConvert.SerializeObject(items)));
        }

        public async Task GenerateClaims()
        {
            await GenerateOrganizationUserRolesAsync();
            await GenerateGlobalUserRolesAsync();
            await GenerateOrgAppUserRolesAsync();
            await GenerateUserAuthorizedIpsAsync();
            await GenerateOrgUsersAsync();
        }
    }
}
