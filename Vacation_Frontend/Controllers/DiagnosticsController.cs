using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using VacationBooking.Services;
using System.Collections.Generic;
using System.Text.Json;

namespace VacationBooking.Controllers
{
    public class DiagnosticsController : Controller
    {
        private readonly VacationApiService _apiService;
        private readonly HttpClient _httpClient;

        public DiagnosticsController(VacationApiService apiService, IHttpClientFactory httpClientFactory)
        {
            _apiService = apiService;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> TestApi()
        {
            var viewModel = new Dictionary<string, string>
            {
                ["BaseUrl Config"] = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/vacations",
                ["Time"] = DateTime.Now.ToString()
            };

            try
            {
                // Test 1: Direct Http Request to known API port
                var apiPort = "5075"; // From your http file
                var directUrl = $"http://localhost:{apiPort}/api/vacations";
                try
                {
                    var directResponse = await _httpClient.GetAsync(directUrl);
                    var statusCode = (int)directResponse.StatusCode;
                    var content = await directResponse.Content.ReadAsStringAsync();
                    var shortContent = content.Length > 100 ? content.Substring(0, 100) + "..." : content;
                    
                    viewModel["Direct API Call"] = $"Status: {statusCode}, Content: {shortContent}";
                    
                    if (directResponse.IsSuccessStatusCode)
                    {
                        var vacations = JsonSerializer.Deserialize<List<object>>(content);
                        viewModel["Direct API Vacation Count"] = vacations?.Count.ToString() ?? "0";
                    }
                }
                catch (Exception ex)
                {
                    viewModel["Direct API Call"] = $"ERROR: {ex.Message}";
                }

                // Test 2: Through service
                try
                {
                    var vacations = await _apiService.GetAllVacationsAsync();
                    viewModel["Service API Call"] = $"Success: {vacations != null}, Count: {vacations?.Count ?? 0}";
                }
                catch (Exception ex)
                {
                    viewModel["Service API Call"] = $"ERROR: {ex.Message}";
                }

                // Test 3: Test several potential API URLs
                var portsToTry = new[] { "5000", "5001", "5075", "5125", "7075", "7125", "44300" };
                foreach (var port in portsToTry)
                {
                    try
                    {
                        var url = $"http://localhost:{port}/api/vacations";
                        var response = await _httpClient.GetAsync(url);
                        viewModel[$"Port {port} Test"] = $"Status: {(int)response.StatusCode} {response.StatusCode}";
                    }
                    catch (Exception ex)
                    {
                        viewModel[$"Port {port} Test"] = $"ERROR: {ex.Message}";
                    }
                }
            }
            catch (Exception ex)
            {
                viewModel["Overall Test"] = $"ERROR: {ex.Message}";
            }

            return View(viewModel);
        }
    }
} 