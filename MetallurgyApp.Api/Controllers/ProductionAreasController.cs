using MetallurgyApp.Api.Data;
using MetallurgyApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MetallurgyApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionAreasController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProductionAreasController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductionAreaDto>>> GetAll(
        string? search, string? workshop)
    {
        var q = _db.ProductionAreas.Include(a => a.Equipments).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(a => a.Name.Contains(search) || a.Head.Contains(search));
        if (!string.IsNullOrWhiteSpace(workshop))
            q = q.Where(a => a.Workshop == workshop);

        var list = await q.AsNoTracking().Select(a => new ProductionAreaDto
        {
            Id = a.Id, Name = a.Name, Workshop = a.Workshop, Head = a.Head,
            CommissionedAt = a.CommissionedAt,
            EquipmentsCount = a.Equipments.Count
        }).ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductionAreaDto>> Get(int id)
    {
        var a = await _db.ProductionAreas.Include(x => x.Equipments)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (a is null) return NotFound();

        return Ok(new ProductionAreaDto
        {
            Id = a.Id, Name = a.Name, Workshop = a.Workshop, Head = a.Head,
            CommissionedAt = a.CommissionedAt,
            EquipmentsCount = a.Equipments.Count,
            Equipments = a.Equipments.Select(e => new EquipmentDto
            {
                Id = e.Id, Name = e.Name, Model = e.Model, Manufacturer = e.Manufacturer,
                ManufactureYear = e.ManufactureYear, State = e.State,
                ProductionAreaId = e.ProductionAreaId, ProductionAreaName = a.Name
            }).ToList()
        });
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(ProductionAreaDto dto)
    {
        var a = new ProductionArea
        {
            Name = dto.Name, Workshop = dto.Workshop, Head = dto.Head,
            CommissionedAt = dto.CommissionedAt
        };
        _db.ProductionAreas.Add(a);
        await _db.SaveChangesAsync();
        return Ok(a.Id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductionAreaDto dto)
    {
        var a = await _db.ProductionAreas.FindAsync(id);
        if (a is null) return NotFound();

        a.Name = dto.Name;
        a.Workshop = dto.Workshop;
        a.Head = dto.Head;
        a.CommissionedAt = dto.CommissionedAt;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var a = await _db.ProductionAreas.FindAsync(id);
        if (a is null) return NotFound();
        _db.ProductionAreas.Remove(a);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}