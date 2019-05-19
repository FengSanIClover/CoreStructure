using Microsoft.EntityFrameworkCore;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using URF.Core.Abstractions;

namespace TP5.ModuleResolver.NetCore
{
    internal class ModuleRegister : IModuleRegister
    {
        private readonly IUnityContainer _container;

        public ModuleRegister(IUnityContainer container)
        {
            this._container = container;
            //Register interception behaviour if any
        }

        public void RegisterType<TFrom, TTo>(bool withInterception = false) where TTo : TFrom
        {
            if (withInterception)
            {
                //register with interception 
            }
            else
            {
                this._container.RegisterType<TFrom, TTo>(new PerResolveLifetimeManager());
            }
        }

        public void RegisterType<TFrom, TTo>(string name, bool withInterception = false) where TTo : TFrom
        {
            if (_container.IsRegistered<TFrom>())
                return;

            if (withInterception)
            {
                this._container.RegisterType<TFrom, TTo>(new PerResolveLifetimeManager());
            }
            else
            {
                this._container.RegisterType<TFrom, TTo>();
            }
        }

        public void RegisterTypeWithSingleton<TFrom, TTo>(bool withInterception = false) where TTo : TFrom
        {
            //ServiceLifetime.Singleton
            this._container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
        }

        public void RegisterTypeWithScoped<TFrom, TTo>(bool withInterception = false) where TTo : TFrom
        {
            //ServiceLifetime.Scoped
            this._container.RegisterType<TFrom, TTo>(new HierarchicalLifetimeManager());
        }

        public void RegisterTypeWithTransient<TFrom, TTo>(bool withInterception = false) where TTo : TFrom
        {
            // this._container.RegisterType<TFrom, TTo>(new PerRequestLifetimeManager());

            //ServiceLifetime.Transient
            this._container.RegisterType<TFrom, TTo>(new TransientLifetimeManager());
        }

        #region URF Framework Register

        public void RegisterDataContext<TFrom, TTo>(string name = "") where TTo : TFrom
        {
            this._container.RegisterType<TFrom, TTo>(name , new PerResolveLifetimeManager());
        }

        public void RegisterUnitOfWork<TFrom, TTo>(string name = "") where TTo : TFrom
        {
            this._container.RegisterType<TFrom, TTo>(name, 
                //new HierarchicalLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<DbContext>(name)));
        }

        public void RegisterRepository<TFrom, TTo>(string name = "") where TTo : TFrom
        {
            if (_container.IsRegistered<TFrom>(name))
                return;

            this._container.RegisterType<TFrom, TTo>(new InjectionConstructor(new ResolvedParameter<DbContext>(name)));
        }

        public void RegisterController<T>(params InjectionMember[] injectionMembers) where T : class
        {
            this._container.RegisterType<T>(injectionMembers);
        }

        #endregion
    }
}
