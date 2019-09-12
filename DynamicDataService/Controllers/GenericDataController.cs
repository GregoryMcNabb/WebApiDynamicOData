using DynamicDataService.Extensions;
using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace DynamicDataService.Controllers
{
    public class GenericDataController<TEntity> : ODataController
        where TEntity : class
    {
        private DbContext db { get; set; }
        private PropertyInfo[] _Keys { get; set; }
        
        public GenericDataController(DbContext model)
        {
            db = model;
            var keyNames = db.GetKeyNames<TEntity>();
            _Keys = typeof(TEntity).GetProperties().Where(p => keyNames.Contains(p.Name)).ToArray();
        }

        public GenericDataController(DbContext model, PropertyInfo[] keys)
        {
            db = model;
            _Keys = keys;
        }

        [EnableQuery]
        public IQueryable<TEntity> Get()
        {
            return db.Set<TEntity>();
        }


        [EnableQuery]
        public SingleResult<TEntity> Get([FromODataUri] string key)
        {
            var data = Get();
            return SingleResult.Create(data.DynamicWhere(_Keys, key));
        }

        public async Task<IHttpActionResult> GetProperty([FromODataUri] string key, string propertyName)
        {
            TEntity item = await Get(key).Queryable.FirstOrDefaultAsync();
            PropertyInfo info = typeof(TEntity).GetProperty(propertyName);
            var propValue = info.GetValue(item);
            return Ok(propValue, info.PropertyType);
        }

        private IHttpActionResult Ok(object propValue, Type propertyType)
        {
            var resultType = typeof(OkNegotiatedContentResult<>).MakeGenericType(propertyType);
            return Activator.CreateInstance(resultType, propValue, this) as IHttpActionResult;
        }

        [EnableQuery(PageSize = 500)]
        public IQueryable<TResult> GetRelatedEntities<TResult>([FromODataUri] string key, string navigationPropertyName)
        {
            var baseQuery = Get(key).Queryable;
            return baseQuery.SelectMany<TEntity, TResult>(navigationPropertyName, new object[] { });
        }

        [EnableQuery(PageSize = 500)]
        public SingleResult<TResult> GetRelatedEntity<TResult>([FromODataUri] string key, string navigationPropertyName)
        {
            var baseQuery = Get(key).Queryable;
            return SingleResult.Create(baseQuery.Select<TEntity, TResult>(navigationPropertyName, new object[] { }));
        }

        public async Task<IHttpActionResult> Post(TEntity item)
        {
            Validate(item);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.Set<TEntity>().Add(item);
            await db.SaveChangesAsync();
            return Created(item);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] string key, Delta<TEntity> item)
        {
            Validate(item);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await Get(key).Queryable.SingleOrDefaultAsync();
            if (entity == null)
                return NotFound();
            item.Patch(entity);
            await db.SaveChangesAsync();
            return Updated(entity);
        }

        public async Task<IHttpActionResult> Put([FromODataUri] string key, TEntity item)
        {
            Validate(item);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await Get(key).Queryable.SingleOrDefaultAsync();
            if (entity == null)
                return NotFound();
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Updated(entity);
        }

        public async Task<IHttpActionResult> Delete([FromODataUri] string key)
        {
            TEntity item = await Get(key).Queryable.SingleOrDefaultAsync();
            if (item == null)
                return NotFound();
            db.Set<TEntity>().Remove(item);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
