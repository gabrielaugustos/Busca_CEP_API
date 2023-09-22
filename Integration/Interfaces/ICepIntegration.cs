using ViaCepAPI.Integration.Response;

namespace ViaCepAPI.Integration.Interfaces
{
    public interface ICepIntegration
    {
        Task<CepResponse> GetCEPData(string cep);
    }
}
 