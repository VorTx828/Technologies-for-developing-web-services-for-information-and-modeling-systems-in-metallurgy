using MetallurgyApp.Client.Models;
using MetallurgyApp.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MetallurgyApp.Client.Controllers;

[Authorize]
public class EquipmentsController : Controller
{
    private readonly ApiClient _api;
    public EquipmentsController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? search, EquipmentState? state, int? areaId)
    {
        var list = await _api.GetEquipmentsAsync(search, state, areaId);

        ViewBag.Search = search;
        ViewBag.State = state;
        ViewBag.AreaId = areaId;
        await PopulateAreasAsync(areaId);

        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var e = await _api.GetEquipmentAsync(id);
        return e is null ? NotFound() : View(e);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateAreasAsync();
        return View(new EquipmentDto());
    }

    [Authorize(Roles = "Admin,Operator")] 
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EquipmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateAreasAsync(dto.ProductionAreaId);
            return View(dto);
        }
        await _api.CreateEquipmentAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var e = await _api.GetEquipmentAsync(id);
        if (e is null) return NotFound();
        await PopulateAreasAsync(e.ProductionAreaId);
        return View(e);
    }

    [Authorize(Roles = "Admin,Operator")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EquipmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateAreasAsync(dto.ProductionAreaId);
            return View(dto);
        }
        await _api.UpdateEquipmentAsync(id, dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteEquipmentAsync(id);
        return RedirectToAction(nameof(Index));
    }


    [Authorize(Roles = "Admin,Operator")]
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _api.DeleteEquipmentAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateAreasAsync(int? selected = null)
    {
        var areas = await _api.GetAreasAsync();
        ViewBag.Areas = new SelectList(areas, "Id", "Name", selected);
    }
}