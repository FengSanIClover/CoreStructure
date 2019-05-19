
using System.Collections.Generic;
using TrackableEntities.Common.Core;

namespace TP5.Core.NetCore.Interface
{
    public interface IDataAccess<TEntity, TSearchModel>
        where TEntity : class, ITrackable, new()
    {
        IEnumerable<TEntity> Search(TSearchModel searchModel);

        bool Exists(TEntity entity);
    }
}
