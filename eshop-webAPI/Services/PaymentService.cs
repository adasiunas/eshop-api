using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using eshopAPI.Requests;
using eshopAPI.Response;
using eshopAPI.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace eshopAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
        {
            var jsonContent = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
               
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", EncodeHelper.Base64Encode(_configuration["PaymentServiceCredentials"]));
            var httpResponse = await httpClient.PostAsync(new Uri(_configuration["PaymentServiceDomain"]), stringContent);

            var response =
                JsonConvert.DeserializeObject<PaymentResponse>(await httpResponse.Content.ReadAsStringAsync());

            response.ResponseCode = (int) httpResponse.StatusCode;
            response.IsSuccessfullResponse = httpResponse.IsSuccessStatusCode;
            
            return response;
        }
    }

    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
    }
}