using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViaCepAPI.Integration;
using ViaCepAPI.Integration.Interfaces;
using ViaCepAPI.Integration.Response;

namespace ViaCepAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CepController : ControllerBase
    {
        private readonly ICepIntegration _cepIntegration;

        public CepController(ICepIntegration cepIntegration)
        {
            _cepIntegration = cepIntegration;
        }

        [HttpGet("{cep}")]
        public async Task<ActionResult<CepResponse>> ListAddressData(string cep) 
        {
            var responseData = await _cepIntegration.GetCEPData(cep);
            if(responseData == null)
            {
                return BadRequest("CEP not found");
            }
            return Ok(responseData);
        }
    }
}
