using System.Net.Http.Json;
using MetallurgyApp.Client.Models;

namespace MetallurgyApp.Client.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    public ApiClient(HttpClient http) => _http = http;

    // Equipments
    public async Task<List<EquipmentDto>> GetEquipmentsAsync(
        string? search = null, EquipmentState? state = null, int? areaId = null)
    {
        var url = "api/equipments?";
        if (!string.IsNullOrWhiteSpace(search)) url += $"search={Uri.EscapeDataString(search)}&";
        if (state.HasValue) url += $"state={(int)state}&";
        if (areaId.HasValue) url += $"areaId={areaId}&";
        return await _http.GetFromJsonAsync<List<EquipmentDto>>(url) ?? new();
    }

    public async Task<EquipmentDto?> GetEquipmentAsync(int id) =>
        await _http.GetFromJsonAsync<EquipmentDto>($"api/equipments/{id}");

    public async Task<int> CreateEquipmentAsync(EquipmentDto dto)
    {
        var r = await _http.PostAsJsonAsync("api/equipments", dto);
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<int>();
    }

    public async Task UpdateEquipmentAsync(int id, EquipmentDto dto)
    {
        var r = await _http.PutAsJsonAsync($"api/equipments/{id}", dto);
        r.EnsureSuccessStatusCode();
    }

    public async Task DeleteEquipmentAsync(int id)
    {
        var r = await _http.DeleteAsync($"api/equipments/{id}");
        r.EnsureSuccessStatusCode();
    }

    // ProductionAreas
    public async Task<List<ProductionAreaDto>> GetAreasAsync(
        string? search = null, string? workshop = null)
    {
        var url = "api/productionareas?";
        if (!string.IsNullOrWhiteSpace(search)) url += $"search={Uri.EscapeDataString(search)}&";
        if (!string.IsNullOrWhiteSpace(workshop)) url += $"workshop={Uri.EscapeDataString(workshop)}&";
        return await _http.GetFromJsonAsync<List<ProductionAreaDto>>(url) ?? new();
    }

    public async Task<ProductionAreaDto?> GetAreaAsync(int id) =>
        await _http.GetFromJsonAsync<ProductionAreaDto>($"api/productionareas/{id}");

    public async Task<int> CreateAreaAsync(ProductionAreaDto dto)
    {
        var r = await _http.PostAsJsonAsync("api/productionareas", dto);
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<int>();
    }

    public async Task UpdateAreaAsync(int id, ProductionAreaDto dto)
    {
        var r = await _http.PutAsJsonAsync($"api/productionareas/{id}", dto);
        r.EnsureSuccessStatusCode();
    }

    public async Task DeleteAreaAsync(int id)
    {
        var r = await _http.DeleteAsync($"api/productionareas/{id}");
        r.EnsureSuccessStatusCode();
    }
}