﻿using AngularEshop.DataLayer.context;
using AngularEshop.DataLayer.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularEshop.DataLayer.Ripository
{
    public class GenericRipository<TEntity> : IGenericRipository<TEntity> where TEntity : BaseEntity
    {
        #region constructor
        private AngularEshopDbContext context;
        private DbSet<TEntity> dbSet;
        public GenericRipository(AngularEshopDbContext context)
        {
            this.context = context;
            this.dbSet = this.context.Set<TEntity>();
        }
        #endregion
        public async Task AddEntity(TEntity entity)
        {
            entity.CreateDate = DateTime.Now;
            entity.LastUpdateDate = entity.CreateDate;
            await dbSet.AddAsync(entity);
        }

     

        public IQueryable<TEntity> GetEntitiesQuery()
        {
            return dbSet.AsQueryable();
        }

        public async Task<TEntity> GetEntityById(long entityId)
        {
            return await dbSet.SingleOrDefaultAsync(e=>e.Id==entityId);
        }

        public void RemoveEntity(TEntity entity)
        {
            entity.IsDelete = true;
            UpdateEntity(entity);
        }

        public async Task RemoveEntity(long entityId)
        {
            var entity = await GetEntityById(entityId);
            RemoveEntity(entity);
        }

        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }

        public void UpdateEntity(TEntity entity)
        {
            entity.LastUpdateDate=DateTime.Now;
            dbSet.Update(entity);
        }
        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
