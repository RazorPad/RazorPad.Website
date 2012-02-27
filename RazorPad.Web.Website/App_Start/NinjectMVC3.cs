using Ninject.Extensions.Conventions;
using RazorPad.Web.Services;

[assembly: WebActivator.PreApplicationStartMethod(typeof(RazorPad.Web.Website.App_Start.NinjectMVC3), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(RazorPad.Web.Website.App_Start.NinjectMVC3), "Stop")]

namespace RazorPad.Web.Website.App_Start
{
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Mvc;

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
            });

            kernel.Bind<IRepository>()
                .To<RavenDb.RavenDbRepository>()
                .InRequestScope();

            kernel.Bind<Raven.Client.IDocumentSession>()
                .ToMethod(ctx => ctx.Kernel.Get<RavenDb.DocumentSessionFactory>().Create())
                .InRequestScope()
                .OnDeactivation(session => {
                    session.SaveChanges();
                    session.Dispose();
                });

            kernel.Bind<Raven.Client.IDocumentStore>()
#if(AppHarbor)
                .To<Raven.Client.Document.DocumentStore>()
#else
                .To<Raven.Client.Embedded.EmbeddableDocumentStore>()
#endif
                .InSingletonScope()
                .WithPropertyValue("ConnectionStringName", "RavenDB")
                .OnActivation(RavenDb.DocumentStoreFactory.Initialize);

        }        
    }
}
