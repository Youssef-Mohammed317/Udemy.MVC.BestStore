using BestStore.Application.Interfaces.Repositories;
using BestStore.Infrastructure.Contexts;
using BestStore.Domain.Entities.Base;
using BestStore.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BestStore.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<Result> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return Result.Success();
        }

        public Task<Result> DeleteAsync(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);

            _dbSet.Remove(entity);
            return Task.FromResult(Result.Success());
        }

        public Task<Result> UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            return Task.FromResult(Result.Success());
        }

        public async Task<Result<TEntity>> GetByIdAsync(int id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!)
        {
            IQueryable<TEntity> query = _dbSet;

            if (include != null)
                query = include(query);


            var entity = await query.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return Result<TEntity>.Failure(
                    Error.Failure(
                        $"{typeof(TEntity).Name}.NotFound",
                        $"{typeof(TEntity).Name} with id {id} not found"
                    )
                );
            }

            return Result<TEntity>.Success(entity);
        }

        public async Task<Result<IEnumerable<TEntity>>> GetAllAsync(
            Expression<Func<TEntity, bool>> filter = null!,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
                query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include(query);

            var result = orderBy != null
                ? await orderBy(query).ToListAsync()
                : await query.ToListAsync();


            return Result<IEnumerable<TEntity>>.Success(result);
        }
        public async Task<Result<PaginatedResult<TEntity>>> GetPaginatedAsync(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          int pageNumber = 1,
          int pageSize = 10,
          bool disableTracking = true
            )
        {

            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
                query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include(query);

            int totalCount = await query.CountAsync();

            if (orderBy != null)
                query = orderBy(query);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paginatedResult = new PaginatedResult<TEntity>(items, totalCount, pageNumber, pageSize);
            return Result<PaginatedResult<TEntity>>.Success(paginatedResult);

        }

    }
}
