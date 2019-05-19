
using Unity.Injection;

namespace TP5.ModuleResolver.NetCore
{
    /// <summary>
    /// Responsible for registering types in unity configuration by implementing IModule
    /// </summary>
    public interface IModuleRegister
    {
        void RegisterType<TFrom, TTo>(bool withInterception = false) where TTo : TFrom;

        void RegisterType<TFrom, TTo>(string name, bool withInterception = false) where TTo : TFrom;

        void RegisterTypeWithSingleton<TFrom, TTo>(bool withInterception = false) where TTo : TFrom;

        void RegisterTypeWithScoped<TFrom, TTo>(bool withInterception = false) where TTo : TFrom;

        void RegisterTypeWithTransient<TFrom, TTo>(bool withInterception = false) where TTo : TFrom;

        #region URF Framework Register

        void RegisterDataContext<TFrom, TTo>(string name = "") where TTo : TFrom;

        void RegisterUnitOfWork<TFrom, TTo>(string name = "") where TTo : TFrom;

        void RegisterRepository<TFrom, TTo>(string name = "") where TTo : TFrom;

        void RegisterController<T>(params InjectionMember[] injectionMembers) where T : class;
        #endregion
    }
}
