
namespace TP5.Core.NetCore.Controllers
{
    public abstract class BaseMvcCrudController<TEntity, TSearchModel> :
        BaseMvcController
        where TEntity : class, ITrackable, new()
    {
        public abstract string Title { get; }

        public abstract string FuncName { get; }

        private Logger logger = LogManager.GetCurrentClassLogger(); 

        #region 初始化

        protected readonly IUnitOfWorkAsync unitOfWork;
        protected readonly IService<TEntity> service;

        protected IDataAccess<TEntity, TSearchModel> daObj
        {
            get => this.service as IDataAccess<TEntity, TSearchModel>;
        }

        public BaseMvcCrudController(
            IUnitOfWorkAsync unitOfWork,
            IService<TEntity> service)
        {
            this.unitOfWork = unitOfWork;
            this.service = service;
        }

        #endregion

        #region MVC Actions(列表)

        public virtual ActionResult Index()
        {
            ViewBag.Title = $"{this.Title} / 列表";
            return View();
        }

        [HttpPost]
        public virtual ActionResult Index_Read(DataTablesRequest dtRequest, TSearchModel searchModel)
        {
            var result = this.daObj.Search(searchModel);

            var grid = new DataTablesResponse<TEntity>(dtRequest, result);
            return Json(grid);
        }

        #endregion

        #region MVC Actions(新增)

        public virtual ViewResult Create()
        {
            ViewBag.Title = $"{this.Title} / 新增";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(TEntity entity)
        {
            entity = SetDefaultValue(entity);

            ViewBag.Title = $"{this.Title} / 新增";

            if (ModelState.IsValid)
            {
                this.service.Insert(entity);

                try
                {
                    this.unitOfWork.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    throw new DbEntityValidationException(ex.Message, ex.EntityValidationErrors);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                TempData["Msg_i"] = $"新增 {this.FuncName} 成功。";
                return RedirectToAction(nameof(this.Index));
            }

            return View(entity);
        }

        #endregion

        #region MVC Actions(修改)

        public virtual ActionResult EditBySeqNo(int seqNo)
        {
            var entity = this.service.Find(seqNo);
            return this.Edit(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditBySeqNo(int seqNo, FormCollection formCollection)
        {
            var entity = this.service.Find(seqNo);
            return this.Edit(entity, formCollection);
        }

        public virtual ActionResult EditByGid(Guid gid)
        {
            var entity = this.service.Find(gid);
            return this.Edit(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditByGid(Guid gid, FormCollection formCollection)
        {
            var entity = this.service.Find(gid);
            return this.Edit(entity, formCollection);
        }

        public virtual ActionResult Edit(object id)
        {
            var entity = this.service.Find(id);
            return this.Edit(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(object id, FormCollection formCollection)
        {
            var entity = this.service.Find(id);
            return this.Edit(entity, formCollection);
        }

        protected virtual ActionResult Edit(TEntity entity)
        {
            ViewBag.Title = $"{this.Title} / 修改";

            return View("Edit", entity);
        }

        protected virtual ActionResult Edit(TEntity entity, FormCollection formCollection)
        {
            entity = SetDefaultValue(entity, false);

            ViewBag.Title = $"{this.Title} / 修改";

            if (TryUpdateModel(entity, formCollection.AllKeys))
            {
                this.service.Update(entity);

                try
                {
                    this.unitOfWork.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    throw new DbEntityValidationException(ex.Message, ex.EntityValidationErrors);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                TempData["Msg_i"] = $"修改 {this.FuncName} 成功。";
                return RedirectToAction(nameof(this.Index));
            }

            return View("Edit", entity);
        }

        #endregion

        #region MVC Actions(刪除)

        [HttpPost]
        public virtual ActionResult DeleteBySeqNo(int seqNo)
        {
            var entity = this.service.Find(seqNo);
            return this.Delete(entity);
        }

        [HttpPost]
        public virtual ActionResult DeleteByGid(Guid gid)
        {
            var entity = this.service.Find(gid);
            return this.Delete(entity);
        }

        [HttpPost]
        public virtual ActionResult Delete(object id)
        {
            var entity = this.service.Find(id);
            return this.Delete(entity);
        }

        protected virtual ActionResult Delete(TEntity entity)
        {
            var ajaxResult = new AjaxRequestResult($"刪除 {this.FuncName} 成功");

            if (entity != null)
            {
                this.service.Delete(entity);

                try
                {
                    this.unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    ajaxResult.Msg = $"刪除 {this.FuncName} 失敗";
                    ajaxResult.Obj = ex;
                    logger.Error(ex.Message);
                }
            }

            return Json(ajaxResult);
        }

        #endregion

        protected virtual TEntity SetDefaultValue(TEntity entity, bool create = true)
        {
            var t = entity.GetType();
            var dateNow = DateTime.Now;

            if (create)
            {
                var createDate = t.GetProperty("CreateDate");
                if (createDate != null)
                    createDate.SetValue(entity, dateNow);

                var createUserId = t.GetProperty("CreateUserId");
                if (createUserId != null)
                    createUserId.SetValue(entity, CoreWeb.IdentityName);
            }

            var updateDate = t.GetProperty("UpdateDate");
            if (updateDate != null)
                updateDate.SetValue(entity, dateNow);

            var updateUserId = t.GetProperty("UpdateUserId");
            if (updateUserId != null)
                updateUserId.SetValue(entity, CoreWeb.IdentityName);

            return entity;
        }
    }
}
