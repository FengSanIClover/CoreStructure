
namespace TP5.ModuleResolver.NetCore
{
    //public class ModuleAssembliesResolver : DefaultAssembliesResolver
    //{
    //    private readonly string modulePath;

    //    public ModuleAssembliesResolver(string modulePath)
    //    {
    //        this.modulePath = modulePath;
    //    }

    //    public override ICollection<Assembly> GetAssemblies()
    //    {
    //        ICollection<Assembly> baseAssemblies = base.GetAssemblies();
    //        List<Assembly> assemblies = new List<Assembly>(baseAssemblies);

    //        if (Directory.Exists(modulePath))
    //        {
    //            Directory.GetFiles(modulePath).ToList().ForEach(o =>
    //            {
    //                var controllersAssembly = Assembly.LoadFrom(o);
    //                baseAssemblies.Add(controllersAssembly);
    //            });
    //        }

    //        return assemblies;
    //    }
    //}
}
