using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using PetCafe.Application.Models.PayOsModels;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IPayOsService
{
    Task SendMail(Guid id);
    Task<PaymentResponseModel> CreatePaymentAsync(double price, double code);
    Task<string> CreatePaymentAsyncV2(double price, double code);
}


public class PayOsService(AppSettings _appSettings) : IPayOsService
{
    private readonly HttpClient _httpClient = new();
    private readonly string apiUrl = "https://api-merchant.payos.vn/v2/payment-requests";


    public async Task<PaymentResponseModel> CreatePaymentAsync(double price, double code)
    {
        long expire_unixTime = ((DateTimeOffset)DateTime.Now.AddMinutes(15)).ToUnixTimeSeconds();
        string signatureHMACSHA256 = GenerateHMACSHA256Signature(code, price);
        // Set headers
        _httpClient.DefaultRequestHeaders.Add("x-client-id", _appSettings.PayOSConfig.ClientId);
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _appSettings.PayOSConfig.ApiKey);

        // Create JSON data
        var data = new
        {
            orderCode = code,
            amount = price,
            description = code,
            cancelUrl = _appSettings.PayOSConfig.CancelURL,
            returnUrl = _appSettings.PayOSConfig.SuccessURL,
            expiredAt = expire_unixTime,
            signature = signatureHMACSHA256
        };
        string requestBody = JsonConvert.SerializeObject(data);

        // Create a new StringContent with the JSON data
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        // Send POST request
        var response = await _httpClient.PostAsync(apiUrl, content);
        if (!response.IsSuccessStatusCode) throw new Exception("Fail to get QR code");

        string responseBody = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<PaymentResponseModel>(responseBody)!;
    }
    public async Task<string> CreatePaymentAsyncV2(double price, double code)
    {
        long expire_unixTime = ((DateTimeOffset)DateTime.Now.AddMinutes(15)).ToUnixTimeSeconds();
        string signatureHMACSHA256 = GenerateHMACSHA256Signature(code, price);
        // Set headers
        _httpClient.DefaultRequestHeaders.Add("x-client-id", _appSettings.PayOSConfig.ClientId);
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _appSettings.PayOSConfig.ApiKey);

        // Create JSON data
        var data = new
        {
            orderCode = code,
            amount = price,
            description = code,
            cancelUrl = _appSettings.PayOSConfig.CancelURL,
            returnUrl = _appSettings.PayOSConfig.SuccessURL,
            expiredAt = expire_unixTime,
            signature = signatureHMACSHA256
        };
        string requestBody = JsonConvert.SerializeObject(data);

        // Create a new StringContent with the JSON data
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        // Send POST request
        var response = await _httpClient.PostAsync(apiUrl, content);
        if (!response.IsSuccessStatusCode) throw new Exception("Fail to get QR code");

        return await response.Content.ReadAsStringAsync();

    }



    public Task SendMail(Guid id)
    {
        throw new NotImplementedException();
    }


    private string GenerateHMACSHA256Signature(double code, double amount)
    {
        var data = $"amount={amount}&cancelUrl={_appSettings.PayOSConfig.CancelURL}&description={code}&orderCode={code}&returnUrl={_appSettings.PayOSConfig.SuccessURL}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_appSettings.PayOSConfig.ChecksumKey));
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var signature = Convert.ToHexStringLower(signatureBytes);
        return signature;
    }
}