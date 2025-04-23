using System.Threading.Tasks;
using GemsAi.Core.Models;

namespace GemsAi.Core.Services
{
    public class DummyErpApiClient : IErpApiClient
    {
        public Task<CreateEmployeeResponse> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            return Task.FromResult(new CreateEmployeeResponse
            {
                IsSuccess = true,
                EmployeeId = Guid.NewGuid().ToString(),
                EmployeeName = dto.Name,
                ErrorMessage = null
            });
        }

        public Task<ErpResult> UpdateEmployeeAsync(UpdateEmployeeDto dto)
        {
            return Task.FromResult(new ErpResult
            {
                IsSuccess = true,
                ErrorMessage = null
            });
        }
    }
}