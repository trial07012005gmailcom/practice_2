using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ClinicManagement.PatientManager.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.PatientManager.Services
{
    /// <summary>
    /// Implementation of the gift service
    /// </summary>
    public class GiftService : IGiftService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GiftService> _logger;
        private const string GiftApiUrl = "https://api.restful-api.dev/objects";

        /// <summary>
        /// Constructor with dependencies
        /// </summary>
        /// <param name="httpClient">HTTP client</param>
        /// <param name="logger">Logger instance</param>
        public GiftService(HttpClient httpClient, ILogger<GiftService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<List<Gift>> GetGiftsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching gifts from external API");
                
                // Get gifts from the external API
                var response = await _httpClient.GetAsync(GiftApiUrl);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                // Parse the response
                var giftsFromApi = JsonSerializer.Deserialize<List<JsonElement>>(content, options);
                var gifts = new List<Gift>();
                
                // Convert to our Gift model
                foreach (var giftElement in giftsFromApi)
                {
                    try
                    {
                        var gift = new Gift
                        {
                            Id = giftElement.GetProperty("id").GetString(),
                            Name = giftElement.GetProperty("name").GetString(),
                            Data = new Dictionary<string, object>()
                        };
                        
                        // Try to get description if it exists
                        if (giftElement.TryGetProperty("data", out var dataElement))
                        {
                            // Add all properties from data
                            foreach (var property in dataElement.EnumerateObject())
                            {
                                if (property.Name.Equals("description", StringComparison.OrdinalIgnoreCase))
                                {
                                    gift.Description = property.Value.GetString();
                                }
                                
                                // Add to data dictionary
                                if (property.Value.ValueKind == JsonValueKind.String)
                                {
                                    gift.Data[property.Name] = property.Value.GetString();
                                }
                                else if (property.Value.ValueKind == JsonValueKind.Number)
                                {
                                    gift.Data[property.Name] = property.Value.GetDouble();
                                }
                                else if (property.Value.ValueKind == JsonValueKind.True || 
                                         property.Value.ValueKind == JsonValueKind.False)
                                {
                                    gift.Data[property.Name] = property.Value.GetBoolean();
                                }
                            }
                        }
                        
                        gifts.Add(gift);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error parsing gift element");
                    }
                }
                
                _logger.LogInformation("Successfully fetched {Count} gifts", gifts.Count);
                return gifts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching gifts from external API");
                throw;
            }
        }
    }
}