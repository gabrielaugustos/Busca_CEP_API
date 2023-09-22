# Busca_CEP_API
Através da nossa API criada faremos um consumo para uma outra API. Essa API será a VIACEP que iremos informar um CEP e ela irá trazer os dados do CEP informado. Utilizaremos o formato JSON.

Inicialmente faremos a estrutura de integração da nossa API com a outra API.

Então criaremos uma pasta, criada para deixar a estrutura mais organizada, chamada *Integration* que dentro dela haverá outras duas pastas, uma chamada *Response* e outra chamada *Refit*.

A *Response* será a pasta que cadastraremos nossa classe de reposta, ou seja, a classe que irá receber os atributos da API, com os mesmos nomes definidos.

Então ficará da seguinte forma.

```csharp
public class CepResponse
    {
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Localidade { get; set; }
        public string? Uf { get; set; }
        public string? Ddd { get; set; }
    }
```

O “?” indica que o dado poderá ser nulo.

Agora, vamos trabalhar com a Refit que será basicamente uma ferramenta que utilizaremos para facilitar nossa comunicação com outras APIs. Ela é uma interface, faz toda a mágica para fazer uma conexão direta via HTTP, como se tivesse carregando os dados na API e trazendo para a aplicação. Há outras formas de fazer a comunicação, porém a Refit é uma das formas mais elegantes.

Teremos nossa Task, a qual dentro dela usaremos a ApiResponse, do próprio Refit, que dentro dela passaremos o que queremos como resposta para esse método, que será o *CepResponse*, que é a nossa classe com todos os atributos. O nome do método será GetCEPData, o qual conterá um “cep” como parâmetro de entrada.

Precisamos decorar essa nossa assinatura com o Refit, explicitando o método que queremos, será um *******get******* que irá buscar na API, passaremos para ele o *cep* via parâmetro, que é o mesmo parâmetro da assinatura.

```csharp
public interface ICepIntegrationRefit
    {
        [Get("/ws/{cep}/json")]
        Task<ApiResponse<CepResponse>> GetCEPData(string cep);
    }
```

Feito isso, temos que fazer o fluxo de configuração do Refit na **********Program.cs**********. O *AddRefitClient* chama o *ICepIntegrationRefit* e o método *ConfigureHttpClient* e dentro do *BaseAddress* passamos o endereço base da API

```csharp
builder.Services.AddRefitClient<ICepIntegrationRefit>().ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri("https://viacep.com.br");
});
```

Basicamente é essa a configuração que fazemos para o Refit carregar a nossa API Via Cep.

Agora, precisamos criar um serviço dentro da *Integration*, a qual chamará de fato a interface *ICepIntegrationRefit* e invocar o método *GetCEPData(string cep)*.

Então dentro de Integration teremos nossa pasta chamada *Interfaces*, que terá a Interface *****ICepIntegration***** dentro dela teremos um *task* que retornará um CepResponse 

```csharp
public interface ICepIntegration
    {
        Task<CepResponse> GetCEPData(string cep);
    }
```

Agora, ainda na pasta Integration criaremos uma classe para implementar essa interface. Implementaremos a chamada do nosso Refit para dentro de nosso serviço de integração. 

No método construtor iremos injetar nossa ICepIntegrationRefit, criaremos a variável e injetaremos ela no construtor e resolver ela. 

Então, charemos a *_cepIntegrationRefit* e ela chamará o GetCEPData que está dentro da interface Refit, que irá bater na API. Faremos um *****await***** pois ele é assíncrono e esse resultado estará armazenado na variável chamada *responseData*. Ou seja, irá nos retornar um objeto do tipo ApiResponse que dentro dela haverá CepResponse. E, faremos um if a qual se nosso responseData for diferente de nulo e deu sucesso ao se comunicar com a API então retornaremos o conteúdo. Ou seja, dentro do responseData temos o conteúdo que ele está retornando, que são os dados da API. Ele entende que o conteúdo é o CepResponse. Mas, se não deu certo retornará nulo.

```csharp
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
            if (responseData != null && responseData.IsSuccessStatusCode) { return responseData.Content; }
            return null;
        }
    }
```

Em suma, criamos a interface ICepIntegration, criamos a implementação CepIntegration mas ainda não está sendo resolvida em lugar nenhum. Então na nossa *Program.cs* resolveremos nossa integração, nossa ICepIntegration agora vai passar a chamar a nossa CepIntegration.

`builder.Services.AddScoped<ICepIntegration, ICepIntegration>();`

Com isso, já configuramos as dependências do CepIntegrations, as dependências do Refit, ou seja, para qual API ele vai carregar no Refit, e dentro da nossa interface do Refit *ICepIntegrationRefit* qual rota ele irá carregar através do *Get()*.

Feito tudo isso, na nossa controller agora podemos criar nosso método propriamente dito. Será um *HttpGet*, que passaremos um parâmetro *cep* que receberá na nossa API. Então podemos injetar nosso serviço.  Faremos somente uma integração que se responseData for nulo o retorno será um *BadRequest “*CEP não encontrado”, mas se encontrar daremos um *Ok* passando o responseData.

Agora podemos rodar
