using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using BestStore.Application.Services;
using BestStore.Application.Interfaces.Services;
using AutoMapper;

namespace BestStore.Application
{
    public static class ApplicationServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddHttpClient<IEmailSender, BrevoEmailSender>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
