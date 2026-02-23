using BestStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Application.DTOs.Order
{
    public class OrderQueryParams
    {
        public string Search { get; set; } = string.Empty;
        public string SortBy { get; set; } = nameof(OrderDto.CreatedAt);
        public bool Ascending { get; set; } = false;
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
    }
}
