using MetallurgyApp.Client.Models;
using MetallurgyApp.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace MetallurgyApp.Client.Controllers;

public class ProductionAreasController : Controller
{
    private readonly ApiClient _api;
    public ProductionAreasController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? search, string? workshop)
    {
        var list = await _api.GetAreasAsync(search, workshop);
        var all = await _api.GetAreasAsync();

        ViewBag.Search = search;
        ViewBag.Workshop = workshop;
        ViewBag.Workshops = all.Select(a => a.Workshop).Distinct().OrderBy(x => x).ToList();

        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var a = await _api.GetAreaAsync(id);
        return a is null ? NotFound() : View(a);
    }

    public IActionResult Create() => View(new ProductionAreaDto());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductionAreaDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        await _api.CreateAreaAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var a = await _api.GetAreaAsync(id);
        return a is null ? NotFound() : View(a);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductionAreaDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid) return View(dto);
        await _api.UpdateAreaAsync(id, dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var a = await _api.GetAreaAsync(id);
        return a is null ? NotFound() : View(a);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _api.DeleteAreaAsync(id);
        return RedirectToAction(nameof(Index));
    }
}