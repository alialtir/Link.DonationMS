using Application.Dtos.UserDTOs;
using DTOs.CampaignDTOs;
using DTOs.CategoryDTOs;
using DTOs.DonationDTOs;
using DTOs.UserDTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Link.DonationMS.AdminPortal.Models
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IEnumerable<CampaignResultDto>> GetCampaignsAsync(int page = 1)
        {
            var response = await _httpClient.GetAsync($"campaigns?page={page}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<CampaignResultDto>>();
        }

        public async Task<IEnumerable<DonationResultDto>> GetDonationsAsync(int page = 1)
        {
            var response = await _httpClient.GetAsync($"donations?page={page}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<DonationResultDto>>();
        }

        public async Task<IEnumerable<UserDto>> GetDonorsAsync()
        {
            var response = await _httpClient.GetAsync("users/donors");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
        }

        public async Task<IEnumerable<UserDto>> GetAdminsAsync()
        {
            var response = await _httpClient.GetAsync("users/admins");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
        }

        public async Task<UserDto> CreateAdminAsync(RegisterUserDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("users/admins", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserDto>();
        }

        public async Task<UserDto> UpdateAdminAsync(Guid id, UserDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"users/admins/{id}", dto);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to update admin: {errorContent}");
            }
            return await response.Content.ReadFromJsonAsync<UserDto>();
        }

        public async Task DeleteAdminAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"users/admins/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to delete admin: {errorContent}");
            }
        }

        public async Task<AuthenticationResult> LoginAsync(LoginDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", dto);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<AuthenticationResult>();
        }

        public async Task CreateCampaignAsync(CreateCampaignDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("campaigns", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<CampaignResultDto> GetCampaignByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"campaigns/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CampaignResultDto>();
        }

        public async Task UpdateCampaignAsync(int id, UpdateCampaignDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"campaigns/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCampaignAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"campaigns/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<UserDto> GetAdminByIdAsync(Guid id)
        {
            var admins = await GetAdminsAsync();
            return admins.FirstOrDefault(a => a.Id == id);
        }


        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("categories");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"categories/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("categories", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"categories/{id}", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"categories/{id}");
            return response.IsSuccessStatusCode;
        }


        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
    }
} 