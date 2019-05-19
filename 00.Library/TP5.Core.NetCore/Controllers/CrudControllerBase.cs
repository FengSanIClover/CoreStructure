using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TP5.Core.NetCore.Models;
using TrackableEntities.Common.Core;
using URF.Core.Abstractions;
using URF.Core.Abstractions.Services;
//Order by用
using System.Linq.Dynamic.Core;
using TP5.Core.NetCore.Helper;

namespace TP5.Core.NetCore.Controllers
{
    public abstract class CrudControllerBase<T, TEntity> : BaseApiController where TEntity : class, ITrackable, new()
    //public abstract class CrudControllerBase<T, TEntity> : ControllerBase where TEntity : class, ITrackable, new()
    {
        protected IUnitOfWork _unitOfWork;
        protected IMapper _mapper;
        protected IService<TEntity> _service;

        //public IUnitOfWork unitOfWork { get; set; }

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public CrudControllerBase(IUnitOfWork unitOfWork, IMapper mapper, IService<TEntity> service)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _service = service;
        }

        /// <summary>
        /// 取得所有資料(無分頁)
        /// </summary>
        /// <returns></returns>
        [Route("[action]")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var queryable = _service.Queryable();
            //if (queryable == null) return NotFound();
            var result = _mapper.Map<List<T>>(queryable);
            return Success(result);
        }

        /// <summary>
        /// 取得所有資料(有分頁)
        /// </summary>
        /// <param name="page">分頁 Model</param>
        /// <returns></returns>
        [Route("[action]")]
        [HttpPost]
        public IActionResult GetAllByPage(PaginatorModel page)
        {
            if (page == null) page = new PaginatorModel();
            else
            {
                //if (page.PageIndex == 0) page.PageIndex = 1;
                if (page.PageSize == 0) page.PageIndex = 10;
            }
            var totalCount = _service.Queryable().Count();
            var sort = PaginatorHelp.GetSortString(page);
            IQueryable<TEntity> queryable = null;

            //SQL Server 2008以上版本的分頁寫法
            //if (!string.IsNullOrEmpty(sort))
            //    queryable = _service.Queryable().OrderBy(sort);
            //else
            //    queryable = _service.Queryable();

            //SQL Server 2008以下版本的分頁寫法
            // queryable = _service.Queryable().OrderBy(sort).Skip((page.PageIndex-1) * page.PageSize).Take(page.PageSize);
            queryable = _service.Queryable().OrderBy(sort).Skip((page.PageIndex) * page.PageSize).Take(page.PageSize);

            var result = _mapper.Map<List<T>>(queryable);
            var model = new { total_count = totalCount, data = result };
            return Success(model);
        }

        /// <summary>
        /// 依ID取得資料
        /// </summary>
        /// <param name="id">PK</param>
        /// <returns></returns>
        //[Route("GetAsync/{id}", Name = "GetAsync")]
        [HttpGet("[action]/{id}")]
        public async System.Threading.Tasks.Task<IActionResult> GetAsync(Guid id)
        {
            var entity = await _service.FindAsync(id);
            //if (entity == null) return NotFound();
            var result = _mapper.Map<T>(entity);
            return Success(result);
        }

        /// <summary>
        /// 依ID檢查是否有資料
        /// </summary>
        /// <param name="id">PK</param>
        /// <returns></returns>
        [Route("[action]/{id}")]
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> ExistsAsync(Guid id)
        {
            var entity = await _service.FindAsync(id);
            //if (entity == null) return NotFound();
            var result = entity == null ? false : true;
            return Success(result);
        }

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="bm">BM</param>
        /// <returns></returns>
        [Route("[action]")]
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> AddAsync(T bm)
        {
            var entity = _mapper.Map<TEntity>(bm);

            #region Gid自動給值
            var t = entity.GetType();
            var gid = t.GetProperty("Gid");
            if (gid != null)
            {
                gid.SetValue(entity, Guid.NewGuid());
            }
            #endregion

            entity = SetDefaultValue(entity);

            if (ModelState.IsValid)
            {
                _service.Insert(entity);
                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                //catch (DbEntityValidationException ex)
                //{
                //    throw new DbEntityValidationException(ex.Message, ex.EntityValidationErrors);
                //}
                catch (Exception ex)
                {
                    //堆疊信息的起點不同之議題 https://blog.csdn.net/lidandan2016/article/details/78864798
                    //throw new Exception(ex.Message); //公司原本的寫法
                    //throw; //在底層，是建議直接把例外拋回既可，可保留原始堆疊
                    throw new Exception(ex.Message, ex); //再包裝過的例外，可保留原始堆疊
                }
            }

            return Success();
        }

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="bm">BM</param>
        /// <returns></returns>
        [Route("[action]")]
        [HttpPut]
        public async System.Threading.Tasks.Task<IActionResult> EditAsync(T bm)
        {
            var entity = _mapper.Map<TEntity>(bm);
            entity = SetDefaultValue(entity, false);

            if (ModelState.IsValid)
            {
                _service.Update(entity);

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                //catch (DbEntityValidationException ex)
                //{
                //    throw new DbEntityValidationException(ex.Message, ex.EntityValidationErrors);
                //}
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
            return Success();
            //return Ok();
        }

        /// <summary>
        /// 邏輯刪除
        /// </summary>
        /// <param name="id">PK</param>
        /// <returns></returns>
        [Route("[action]/{id}")]
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> EditIsDeleteAsync(Guid id)
        {
            //var guid = Guid.Parse(id);
            var entity = await _service.FindAsync(id);
            if (entity != null)
            {
                entity = SetDefaultValue(entity, false);
                var t = entity.GetType();
                var isDelete = t.GetProperty("IsDeleted");
                if (isDelete != null)
                {
                    isDelete.SetValue(entity, true);
                }
                _service.Update(entity);

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                //catch (DbEntityValidationException ex)
                //{
                //    throw new DbEntityValidationException(ex.Message, ex.EntityValidationErrors);
                //}
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
            return Success();
        }

        /// <summary>
        /// 實體刪除
        /// </summary>
        /// <param name="id">PK</param>
        /// <returns></returns>
        //[Route("DeleteAsync/{id}", Name = "DeleteAsync")]
        [HttpDelete("[action]/{id}")]
        public async System.Threading.Tasks.Task<IActionResult> DeleteAsync(Guid id)
        {
            var bl = await _service.DeleteAsync(id);

            //var entity = await _service.FindAsync(id);
            //if (entity == null) return Failure();

            //entity.TrackingState = TrackingState.Deleted;
            //_service.Delete(entity);

            try
            {
               var count = await _unitOfWork.SaveChangesAsync();
            }
            //catch (DbEntityValidationException ex)
            //{
            //    throw new DbEntityValidationException(ex.Message, ex.EntityValidationErrors);
            //}
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return Success();
        }

        /// <summary>
        /// 設定4大金鋼的預設值
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        protected virtual TEntity SetDefaultValue(TEntity entity, bool create = true)
        {
            var t = entity.GetType();
            var dateNow = DateTime.Now;

            if (create)
            {
                var createDate = t.GetProperty("CreatedAt");
                if (createDate != null)
                {
                    createDate.SetValue(entity, dateNow);
                }
                
                //Eagle：先註解，因為要再另行測試能否捉到API 的Identity
                //var createdBy = t.GetProperty("CreatedBy");
                //if (createdBy != null)
                //    createdBy.SetValue(entity, CoreWeb.IdentityName);
            }

            var updateDate = t.GetProperty("UpdatedAt");
            if (updateDate != null)
                updateDate.SetValue(entity, dateNow);
            
            //Eagle：先註解，因為要再另行測試能否捉到API 的Identity
            //var updatedBy = t.GetProperty("UpdatedBy");
            //if (updatedBy != null)
            //    updatedBy.SetValue(entity, CoreWeb.IdentityName);

            return entity;
        }

        private IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, PaginatorModel page)
        {
            return query.OrderBy(PaginatorHelp.GetSortString(page));
        }
    }
}
