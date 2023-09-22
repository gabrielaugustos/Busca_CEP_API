using Refit;
using ViaCepAPI.Integration.Response;

namespace ViaCepAPI.Integration.Refit
{
    public interface ICepIntegrationRefit
    {
        [Get("/ws/{cep}/json")]
        Task<ApiResponse<CepResponse>> GetCEPData(string cep);
    }
}
