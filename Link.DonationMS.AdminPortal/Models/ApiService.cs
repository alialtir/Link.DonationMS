using Application.Dtos.UserDTOs;
using DTOs.CampaignDTOs;
using DTOs.CategoryDTOs;
using DTOs.DonationDTOs;
using DTOs.UserDTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using DTOs.DashboardDTOs;

namespace Link.DonationMS.AdminPortal.Models
{
    public class CampaignsResponse
    {
        public IEnumerable<CampaignResultDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<IEnumerable<CampaignResultDto>> GetCampaignsAsync(int page = 1)
        {
            AddAuthHeader();
            try
            {
                var endpoint = _configuration["ApiEndpoints:Campaigns:GetAll"];
                var response = await _httpClient.GetAsync($"{endpoint}?page={page}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<CampaignsResponse>();
                return result?.Items ?? Enumerable.Empty<CampaignResultDto>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"API is not accessible. Please ensure the API is running on {_httpClient.BaseAddress}. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while getting campaigns: {ex.Message}");
            }
        }

        public async Task<int> GetCampaignsCountAsync()
        {
            AddAuthHeader();
            try
            {
                var endpoint = _configuration["ApiEndpoints:Campaigns:Count"];
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<int>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"API is not accessible. Please ensure the API is running on {_httpClient.BaseAddress}. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while getting campaigns count: {ex.Message}");
            }
        }

        public async Task<IEnumerable<DonationResultDto>> GetDonationsAsync(int page = 1)
        {
            AddAuthHeader();
            try
            {
                var endpoint = _configuration["ApiEndpoints:Donations:GetAll"];
                var response = await _httpClient.GetAsync($"{endpoint}?page={page}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<DonationResultDto>>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"API is not accessible. Please ensure the API is running on {_httpClient.BaseAddress}. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while getting donations: {ex.Message}");
            }
        }

        public async Task<IEnumerable<UserDto>> GetDonorsAsync()
        {
            AddAuthHeader();
            var endpoint = _configuration["ApiEndpoints:Users:GetDonors"];
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
        }

        public async Task<IEnumerable<UserDto>> GetAdminsAsync()
        {
            AddAuthHeader();
            var endpoint = _configuration["ApiEndpoints:Users:GetAdmins"];
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
        }

        public async Task<UserDto> CreateAdminAsync(CreateAdminDto dto)
        {
            AddAuthHeader();
            var endpoint = _configuration["ApiEndpoints:Users:CreateAdmin"];
            var response = await _httpClient.PostAsJsonAsync(endpoint, dto);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to create admin: {errorContent}");
            }
            return await response.Content.ReadFromJsonAsync<UserDto>();
        }

        public async Task<UserDto> UpdateAdminAsync(Guid id, UserDto dto)
        {
            AddAuthHeader();
            var endpoint = string.Format(_configuration["ApiEndpoints:Users:UpdateAdmin"], id);
            var response = await _httpClient.PutAsJsonAsync(endpoint, dto);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to update admin: {errorContent}");
            }
            return await response.Content.ReadFromJsonAsync<UserDto>();
        }

        public async Task DeleteAdminAsync(Guid id)
        {
            AddAuthHeader();
            var endpoint = string.Format(_configuration["ApiEndpoints:Users:DeleteAdmin"], id);
            var response = await _httpClient.DeleteAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to delete admin: {errorContent}");
            }
        }

        public async Task<AuthenticationResult> LoginAsync(LoginDto dto)
        {
            try
            {
                var endpoint = _configuration["ApiEndpoints:Auth:Login"];
                var response = await _httpClient.PostAsJsonAsync(endpoint, dto);
                if (!response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadFromJsonAsync<AuthenticationResult>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"API is not accessible. Please ensure the API is running on {_httpClient.BaseAddress}. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred during login: {ex.Message}");
            }
        }

        public async Task CreateCampaignAsync(CreateCampaignDto dto)
        {
            AddAuthHeader();
            var endpoint = _configuration["ApiEndpoints:Campaigns:Create"];
            var response = await _httpClient.PostAsJsonAsync(endpoint, dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<CampaignResultDto> GetCampaignByIdAsync(int id)
        {
            AddAuthHeader();
            var endpoint = string.Format(_configuration["ApiEndpoints:Campaigns:GetById"], id);
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CampaignResultDto>();
        }

        public async Task UpdateCampaignAsync(int id, UpdateCampaignDto dto)
        {
            AddAuthHeader();
            var endpoint = string.Format(_configuration["ApiEndpoints:Campaigns:Update"], id);
            var response = await _httpClient.PutAsJsonAsync(endpoint, dto);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"API Error {(int)response.StatusCode}: {errorContent}");
            }
        }

        public async Task DeleteCampaignAsync(int id)
        {
            AddAuthHeader();
            var endpoint = string.Format(_configuration["ApiEndpoints:Campaigns:Delete"], id);
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
        }

        public async Task<UserDto> GetAdminByIdAsync(Guid id)
        {
            AddAuthHeader();
            var admins = await GetAdminsAsync();
            return admins.FirstOrDefault(a => a.Id == id);
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int page = 1)
        {
            AddAuthHeader();
            var endpoint = _configuration["ApiEndpoints:Categories:GetAll"];
            var response = await _httpClient.GetAsync($"{endpoint}?page={page}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            AddAuthHeader();
            var endpoint = string.Format(_configuration["ApiEndpoints:Categories:GetById"], id);
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            AddAuthHeader();
            var endpoint = _configuration["ApiEndpoints:Categories:Create"];
            var response = await _httpClient.PostAsJsonAsync(endpoint, dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            AddAuthHeader();
            var endpoint = string.Format(_configuration["ApiEndpoints:Categories:Update"], id);
            var response = await _httpClient.PutAsJsonAsync(endpoint, dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            AddAuthHeader();
            var endpoint = string.Format(_configuration["ApiEndpoints:Categories:Delete"], id);
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<CampaignProgressDto>> GetTopCampaignsAsync()
        {
            AddAuthHeader();
            var endpoint = _configuration["ApiEndpoints:Dashboard:TopCampaigns"];
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<CampaignProgressDto>>();
        }

        public async Task<DashboardStatsDto> GetDashboardOverviewAsync()
        {
            AddAuthHeader();
            var endpoint = _configuration["ApiEndpoints:Dashboard:Overview"];
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DashboardStatsDto>();
        }

        public async Task<int> GetCategoriesCountAsync()
        {
            AddAuthHeader();
            var endpoint = _configuration["ApiEndpoints:Categories:Count"];
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<bool> ResetPasswordByEmailAsync(string email, string newPassword)
        {
            var endpoint = _configuration["ApiEndpoints:Users:ResetPasswordByEmail"];
            var payload = new { email, newPassword };
            var response = await _httpClient.PostAsync(endpoint, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
    }
} 