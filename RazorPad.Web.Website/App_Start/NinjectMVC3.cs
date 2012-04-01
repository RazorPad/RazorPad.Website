using System.Web.Mvc;

[assembly: WebActivator.PreApplicationStartMethod(typeof(RazorPad.Web.Website.App_Start.NinjectMVC3), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(RazorPad.Web.Website.App_Start.NinjectMVC3), "Stop")]

namespace RazorPad.Web.Website.App_Start
{
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Mvc;
    using Ninject.Extensions.Conventions;

    public static class NinjectMVC3 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestModule));
            DynamicModuleUtility.RegisterModule(typeof(HttpApplicationInitializationModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Scan(scanner => {
                scanner.FromAssembliesMatching("RazorPad.*");
                scanner.BindWithDefaultConventions();
                scanner.Excluding<EntityFramework.Repository>();
            });

            kernel.Bind<Services.IRepository>()
                .To<EntityFramework.Repository>()
                .InRequestScope()
                .WithConstructorArgument("isSharedContext", false);

            kernel.Bind<EntityFramework.RazorPadContext>()
                .ToSelf()
                .InRequestScope();


            kernel.Bind<Authentication.Facebook.FacebookService>().ToSelf()
                .InSingletonScope()
                .WithPropertyValue("LocalEndpoint", x => x.Kernel.Get<UrlHelper>().ExternalAction("Authorize", "Facebook"));
        }        
    }
}
