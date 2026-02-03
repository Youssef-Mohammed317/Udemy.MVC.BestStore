using BestStore.Application.Interfaces.Repositories;
using BestStore.Infrastructure.Contexts;
using BestStore.Shared.Result;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IProductRepository _productRepository;
        private ICategoryRepository _categoryRepository;
        private IOrderRepository _orderRepository;
        private IOrderItemRepository _orderItemRepository;
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            this._context = context;
        }
        public IProductRepository ProductRepository
        {
            get
            {

                if (_productRepository == null)
                {
                    _productRepository = new ProductRepository(_context);
                }
                return _productRepository;
            }
        }
        public ICategoryRepository CategoryRepository
        {
            get
            {

                if (_categoryRepository == null)
                {
                    _categoryRepository = new CategoryRepository(_context);
                }
                return _categoryRepository;
            }
        }
        public IOrderRepository OrderRepository
        {
            get
            {

                if (_orderRepository == null)
                {
                    _orderRepository = new OrderRepository(_context);
                }
                return _orderRepository;
            }
        }
        public IOrderItemRepository OrderItemRepository
        {
            get
            {

                if (_orderItemRepository == null)
                {
                    _orderItemRepository = new OrderItemRepository(_context);
                }
                return _orderItemRepository;
            }
        }

        public async Task<Result> SaveChangesAsync()
        {
            try
            {

                await _context.SaveChangesAsync();
                return Result.Success();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Failure(
                    Error.Failure(
                        "concurrency_error",
                        "A concurrency error occurred while saving changes to the database."
                    )
                );
            }
            catch (DbUpdateException)
            {
                return Result.Failure(
                    Error.Failure(
                        "database_update_error",
                        "An error occurred while updating the database."
                    )
                );
            }

        }
    }
}
