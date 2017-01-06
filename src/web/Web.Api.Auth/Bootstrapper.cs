using Autofac;
using Autofac.Core.Lifetime;
using Common.Logging;
using Extensions.Owin;
using Owin;
using Common.Repository;
using Newtonsoft.Json;
using Common.Notification;
using System.Web;
using System.Threading;
using Microsoft.Owin;
using IdentityProviders.SqlServerProvider;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Effort.Provider;
using System;

namespace Web.Api.Auth
{
    //public static class Bootstrapper
    //{
    //    public static void RegisterAutofac(ContainerBuilder builder)
    //    {
    //        /*services*/
    //        builder.Register(c => LogServiceAsync<CustomLogServiceOptions>.Instance)
    //            .As<ILogServiceAsync<ILogServiceSettings>>()
    //            .SingleInstance();
    //        builder.Register(c => DefaultNotificationClient.Instance)
    //            .As<INotificationClient>()
    //            .OnActivated(i => i.Instance.OnFailedToNotify += Bootstrapper.HandleOnFailedToNotify)
    //            .SingleInstance();

    //        //builder.Register(c => new FileUploadRepo(new AppFrameworkEntities())).As<RepoBase<DocumentUpload>>()
    //        //       .InstancePerRequest();

    //        //builder.Register(c => new DepartmentRepo(new ITGovEntities()))
    //        //       .AsSelf()
    //        //       .As<RepoBase<Department>>()
    //        //       .InstancePerRequest();
    //        //builder.Register(c => new CoreSystemRepo(new ITGovEntities()))
    //        //       .AsSelf()
    //        //       .As<RepoBase<CoreSystem>>()
    //        //       .InstancePerRequest();
    //        //builder.Register(c => new SystemUserRepo(new ITGovEntities()))
    //        //       .AsSelf()
    //        //       .As<RepoBase<SystemUser>>()
    //        //       .InstancePerRequest();
    //        //builder.Register(c => new VendorRepo(new ITGovEntities()))
    //        //       .AsSelf()
    //        //       .As<RepoBase<Vendor>>()
    //        //       .InstancePerRequest();
    //        //builder.Register(c => new IssueRepo(new ITGovEntities()))
    //        //       .AsSelf()
    //        //       .As<RepoBase<Tracking>>()
    //        //       .InstancePerRequest();
    //        //builder.Register(c => new IssueViewRepo(new ITGovEntities()))
    //        //       .AsSelf()
    //        //       .As<RepoBase<vwTracking>>()
    //        //       .InstancePerRequest();
    //        //builder.Register(c => new CommentRepo(new ITGovEntities()))
    //        //       .AsSelf()
    //        //       .As<RepoBase<Comment>>()
    //        //       .InstancePerRequest();
    //        //builder.Register(c => new IssueDeptRepo(new ITGovEntities()))
    //        //       .AsSelf()
    //        //       .As<RepoBase<TrackingDepartment>>()
    //        //       .InstancePerRequest();
    //        //builder.Register(c => new AttachmentRepo(new ITGovEntities()))
    //        //       .AsSelf()
    //        //       .As<RepoBase<Attachment>>()
    //        //       .InstancePerRequest();
    //        //builder.Register(c => new IncidentStatusRepo(new ITGovEntities()))
    //        //       .AsSelf()
    //        //       .As<RepoBase<IncidentStatus>>()
    //        //       .InstancePerRequest();
    //    }

    //    public static void RegisterOwin(IAppBuilder app)
    //    {
    //        app.CreatePerOwinContext<ILogServiceAsync<ILogServiceSettings>>((options, owinContext) =>
    //        {
    //            var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");
    //            var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;
    //            return logService;
    //        });

    //        //app.CreatePerOwinContext<ITGovEntities>(() => { return new ITGovEntities(); });
    //    }

    //    public static void HandleOnDbError(object source, string serializedArgs)
    //    {
    //        dynamic dbEx = JsonConvert.DeserializeObject(serializedArgs);

    //        try
    //        {
    //            LogServiceAsync<CustomLogServiceOptions>.Instance.LogMessage(serializedArgs);

    //            NotificationException notification = dbEx.notification.ToObject<NotificationException>();
    //            notification.Username = HttpContext.Current.User.Identity.Name;
    //            notification.RawMessage = serializedArgs;
    //            DefaultNotificationClient.Instance.Notify(notification);
    //        }
    //        catch { }
    //    }

    //    public static void HandleOnFailedToNotify(object source, string serializedArgs)
    //    {
    //        try
    //        {
    //            LogServiceAsync<CustomLogServiceOptions>.Instance.LogMessage(serializedArgs);
    //        }
    //        catch { }
    //    }
    //}

    public static class Bootstrapper
    {
        public static void RegisterAutofac(ContainerBuilder builder)
        {
            /*services*/
            builder.Register(c => LogServiceAsync<WebLogServiceOptions>.Instance)
                .As<ILogServiceAsync<ILogServiceSettings>>()
                .SingleInstance();

            /*settings*/
            builder.Register(c => new ApiSettings()).SingleInstance();

            /*sql repositories*/

            builder.Register(c => new SystemUserRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<User>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new OrgRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<Org>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new OrgRoleRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<OrgRole>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new OrgUserRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<OrgUser>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new OrgUserRoleRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<OrgUserRole>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new OrgAppRoleRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<OrgAppRole>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new OrgAppRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<OrgApp>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new OrgAppUserAuthIpRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<OrgAppUserAuthIp>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new OrgAppUserRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<OrgAppUser>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new OrgAppUserRoleRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<OrgAppUserRole>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new OrgGlobalRoleRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<OrgGlobalRole>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new GlobalUserRoleRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<OrgGlobalUserRole>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new UserRefreshTokenRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<UserRefreshToken>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
            builder.Register(c => new OrgAppUserMetadataRepo(c.Resolve<AppFrameworkEntities>())).As<IRepository<OrgAppUserMetadata>>().OnActivated(i => i.Instance.OnDbError += HandleOnDbError).InstancePerRequest();
        }

        public static void RegisterOwin(IAppBuilder app)
        {
            app.CreatePerOwinContext<AppFrameworkEntities>((options, owinContext) =>
            {
                return new AppFrameworkEntities();
            });

            app.CreatePerOwinContext<RepoBase<User>>((options, owinContext) =>
            {

                var logService = owinContext.Get<ILogServiceAsync<ILogServiceSettings>>();
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logSeddrvice = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new SystemUserRepo(owinContext.Get<AppFrameworkEntities>());
            });

            app.CreatePerOwinContext<RepoBase<Org>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new OrgRepo(owinContext.Get<AppFrameworkEntities>());
            });

            app.CreatePerOwinContext<RepoBase<OrgRole>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new OrgRoleRepo(owinContext.Get<AppFrameworkEntities>());
            });
            app.CreatePerOwinContext<RepoBase<OrgUser>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new OrgUserRepo(owinContext.Get<AppFrameworkEntities>());
            });
            app.CreatePerOwinContext<RepoBase<OrgUserRole>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new OrgUserRoleRepo(owinContext.Get<AppFrameworkEntities>());
            });
            app.CreatePerOwinContext<RepoBase<OrgAppRole>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new OrgAppRoleRepo(owinContext.Get<AppFrameworkEntities>());
            });
            app.CreatePerOwinContext<RepoBase<OrgApp>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new OrgAppRepo(owinContext.Get<AppFrameworkEntities>());
            });
            app.CreatePerOwinContext<RepoBase<OrgAppUserAuthIp>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new OrgAppUserAuthIpRepo(owinContext.Get<AppFrameworkEntities>());
            });
            app.CreatePerOwinContext<RepoBase<OrgAppUser>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new OrgAppUserRepo(owinContext.Get<AppFrameworkEntities>());
            });
            app.CreatePerOwinContext<RepoBase<OrgAppUserRole>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new OrgAppUserRoleRepo(owinContext.Get<AppFrameworkEntities>());
            });
            app.CreatePerOwinContext<RepoBase<OrgGlobalRole>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new OrgGlobalRoleRepo(owinContext.Get<AppFrameworkEntities>());
            });
            app.CreatePerOwinContext<RepoBase<OrgGlobalUserRole>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new GlobalUserRoleRepo(owinContext.Get<AppFrameworkEntities>());
            });
            app.CreatePerOwinContext<RepoBase<UserRefreshToken>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new UserRefreshTokenRepo(owinContext.Get<AppFrameworkEntities>());
            });
            app.CreatePerOwinContext<RepoBase<OrgAppUserMetadata>>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                return new OrgAppUserMetadataRepo(owinContext.Get<AppFrameworkEntities>());
            });
            app.CreatePerOwinContext<UserStore>((options, owinContext) =>
            {
                return new UserStore(owinContext.Get<RepoBase<User>>());
            });

            app.CreatePerOwinContext<ApplicationUserManager>((options, owinContext) =>
            {
                var iocContainer = owinContext.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");

                var userStore = owinContext.Get<UserStore>();

                var manager = new ApplicationUserManager(userStore);

                var apm = new ApplicationUserManager(userStore);

                // Configure validation logic for usernames
                manager.UserValidator = new UserValidator<User, int>(manager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = true
                };
                // Configure validation logic for passwords
                manager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 6,
                    RequireNonLetterOrDigit = true,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = true,
                };

                var dataProtectionProvider = options.DataProtectionProvider;

                if (dataProtectionProvider != null)
                {
                    manager.UserTokenProvider = new DataProtectorTokenProvider<User, int>(dataProtectionProvider.Create("ASP.NET Identity"));
                }

                return manager;
            });

            app.CreatePerOwinContext<UserStore>((options, owinContext) =>
            {
                return new UserStore(owinContext.Get<RepoBase<User>>());
            });

            var context = new OwinContext(app.Properties);

            var token = context.Get<CancellationToken>("host.OnAppDisposing");

            if (token != CancellationToken.None)
            {
                token.Register(() =>
                {
                    var iocContainer = context.ToLifetimeScope<LifetimeScope>("autofac:OwinLifetimeScope");
                    var logService = iocContainer.GetService(typeof(ILogServiceAsync<ILogServiceSettings>)) as ILogServiceAsync<ILogServiceSettings>;

                    logService.LogMessage(new
                    {
                        msg = "owin cancellation logservice dispose",
                    });
                    logService.Dispose();
                });
            }
        }

        public static void HandleOnDbError(object source, string serializedArgs)
        {
            dynamic dbEx = JsonConvert.DeserializeObject(serializedArgs);
            try
            {
                LogServiceAsync<CustomLogServiceOptions>.Instance.LogMessage(serializedArgs);
                NotificationException notification = dbEx.notification.ToObject<NotificationException>();
                notification.Username = HttpContext.Current.User.Identity.Name;
                notification.RawMessage = serializedArgs;
                DefaultNotificationClient.Instance.Notify(notification);
            }
            catch { }
        }
        public static void HandleOnFailedToNotify(object source, string serializedArgs)
        {
            try
            {
                LogServiceAsync<CustomLogServiceOptions>.Instance.LogMessage(serializedArgs);
            }
            catch { }
        }
    }
}