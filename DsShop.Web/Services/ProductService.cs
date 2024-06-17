using DsShop.Web.Models;
using DsShop.Web.Services.Contracts;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DsShop.Web.Services;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _clientFactory;
    private const string _apiEndpoint = "/api/products/";
    private readonly JsonSerializerOptions _options;

    // Tipos de retorno do métodos
    private ProductViewModel _productVM;
    private IEnumerable<ProductViewModel> _productsVM;

    private string _httpClientName = "ProductApi";

    public ProductService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    private HttpClient CreateHttpClient()
    {
        return _clientFactory.CreateClient(_httpClientName);
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProducts(string token)
    {
        var client = CreateHttpClient();
        PutTokenInHeaderAuthorization(token, client);

        using (var response = await client.GetAsync(_apiEndpoint))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();

                _productsVM = await JsonSerializer
                                .DeserializeAsync<IEnumerable<ProductViewModel>>(apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        return _productsVM;
    }

    public async Task<ProductViewModel> FindProductById(int id, string token)
    {
        var client = CreateHttpClient();
        PutTokenInHeaderAuthorization(token, client);

        using (var response = await client.GetAsync(_apiEndpoint + id))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();

                _productVM = await JsonSerializer
                                .DeserializeAsync<ProductViewModel>(apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        return _productVM;
    }

    public async Task<ProductViewModel> CreateProduct(ProductViewModel productVM, string token)
    {
        var client = CreateHttpClient();
        PutTokenInHeaderAuthorization(token, client);

        // Conteúdo HTTP baseado em uma string
        StringContent content = new StringContent(JsonSerializer.Serialize(productVM),
                                Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync(_apiEndpoint, content))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();

                _productVM = await JsonSerializer
                                .DeserializeAsync<ProductViewModel>(apiResponse, _options);
            }
            else
            {
                return null;
            }
        }

        return _productVM;
    }

    public async Task<ProductViewModel> UpdateProduct(ProductViewModel productVM, string token)
    {
        var client = CreateHttpClient();
        PutTokenInHeaderAuthorization(token, client);

        ProductViewModel productUpdated = new ProductViewModel();

        using (var response = await client.PutAsJsonAsync(_apiEndpoint, productVM))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();

                _productVM = await JsonSerializer
                             .DeserializeAsync<ProductViewModel>(apiResponse, _options);
            }
            else
            {
                return null;
            }
        }

        return productUpdated;

    }

    public async Task<bool> DeleteProductById(int id, string token)
    {
        var client = CreateHttpClient();
        PutTokenInHeaderAuthorization(token, client);

        using (var response = await client.DeleteAsync(_apiEndpoint + id))
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }

        return false;
    }

    private void PutTokenInHeaderAuthorization(string token, HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
