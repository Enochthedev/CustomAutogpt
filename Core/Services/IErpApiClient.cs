using System.Threading.Tasks;
using GemsAi.Core.Models;

namespace GemsAi.Core.Services
{
    public interface IErpApiClient
    {
        Task<CreateEmployeeResponse> CreateEmployeeAsync(CreateEmployeeDto dto);
        // Add more methods for other ERP operations as you scale!
    }
}