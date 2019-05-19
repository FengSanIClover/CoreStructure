
namespace TP5.ModuleResolver.NetCore
{
    /// <summary>
    /// To register all the internal type with unity.
    /// </summary>
    public interface IModule
    {
        void SetUp(IModuleRegister register);
    }
}
