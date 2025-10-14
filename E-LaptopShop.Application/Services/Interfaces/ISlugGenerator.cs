using E_LaptopShop.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Interfaces
{
    public interface ISlugGenerator
    {
        Task<string> GenerateSlugAsync(
            string name, 
            string? entitySet, 
            int? excludeId = null, 
            CancellationToken ct = default);
    }
}
