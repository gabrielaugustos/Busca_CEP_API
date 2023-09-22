using ViaCepAPI.Integration.Interfaces;
using ViaCepAPI.Integration.Refit;
using ViaCepAPI.Integration.Response;

namespace ViaCepAPI.Integration
{
    public class CepIntegration : ICepIntegration
    {
        private readonly ICepIntegrationRefit _cepIntegrationRefit;
        public CepIntegration(ICepIntegrationRefit cepIntegrationRefit)
        {
            _cepIntegrationRefit = cepIntegrationRefit;
        }
        public async Task<CepResponse> GetCEPData(string cep)
        {    
            var responseData = await _cepIntegrationRefit.GetCEPData(cep);
            if (responseData != null && responseData.IsSuccessStatusCode) 
            { 
                return responseData.Content; 
            }
            return null;
        }
        
    }
}
