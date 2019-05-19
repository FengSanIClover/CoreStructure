using TrackableEntities.Common.Core;
using URF.Core.Abstractions.Services;

namespace TP5.Core.NetCore.Interface
{
    public interface ICrudService<TEntity, TSearchModel> :
        IService<TEntity>,
        IDataAccess<TEntity, TSearchModel>
        where TEntity : class, ITrackable, new()
    {
    }
}
