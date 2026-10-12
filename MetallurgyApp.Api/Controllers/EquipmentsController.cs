using MetallurgyApp.Api.Data;
using MetallurgyApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MetallurgyApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public EquipmentsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquipmentDto>>> GetAll(
        string? search, EquipmentState? state, int? areaId)
    {
        var q = _db.Equipments.Include(e => e.ProductionArea).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(e => e.Name.Contains(search) || e.Model.Contains(search));
        if (state.HasValue) q = q.Where(e => e.State == state);
        if (areaId.HasValue) q = q.Where(e => e.ProductionAreaId == areaId);

        var list = await q.AsNoTracking().Select(e => new EquipmentDto
        {
            Id = e.Id, Name = e.Name, Model = e.Model, Manufacturer = e.Manufacturer,
            ManufactureYear = e.ManufactureYear, State = e.State,
            ProductionAreaId = e.ProductionAreaId,
            ProductionAreaName = e.ProductionArea != null ? e.ProductionArea.Name : null
        }).ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EquipmentDto>> Get(int id)
    {
        var e = await _db.Equipments.Include(x => x.ProductionArea)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return NotFound();

        return Ok(new EquipmentDto
        {
            Id = e.Id, Name = e.Name, Model = e.Model, Manufacturer = e.Manufacturer,
            ManufactureYear = e.ManufactureYear, State = e.State,
            ProductionAreaId = e.ProductionAreaId,
            ProductionAreaName = e.ProductionArea?.Name
        });
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(EquipmentCreateDto dto)
    {
        var e = new Equipment
        {
            Name = dto.Name, Model = dto.Model, Manufacturer = dto.Manufacturer,
            ManufactureYear = dto.ManufactureYear, State = dto.State,
            ProductionAreaId = dto.ProductionAreaId
        };
        _db.Equipments.Add(e);
        await _db.SaveChangesAsync();
        return Ok(e.Id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EquipmentCreateDto dto)
    {
        var e = await _db.Equipments.FindAsync(id);
        if (e is null) return NotFound();

        e.Name = dto.Name;
        e.Model = dto.Model;
        e.Manufacturer = dto.Manufacturer;
        e.ManufactureYear = dto.ManufactureYear;
        e.State = dto.State;
        e.ProductionAreaId = dto.ProductionAreaId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Equipments.FindAsync(id);
        if (e is null) return NotFound();
        _db.Equipments.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}