using System.ComponentModel.DataAnnotations;

namespace MetallurgyApp.Api.Models;

public class ProductionArea
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Workshop { get; set; } = string.Empty;
    public string Head { get; set; } = string.Empty;
    public DateTime CommissionedAt { get; set; }
    public List<Equipment> Equipments { get; set; } = new();
}

public class ProductionAreaDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Workshop { get; set; } = string.Empty;
    public string Head { get; set; } = string.Empty;
    public DateTime CommissionedAt { get; set; }
    public int EquipmentsCount { get; set; }
    public List<EquipmentDto> Equipments { get; set; } = new();
}

public class EquipmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public int ManufactureYear { get; set; }
    public EquipmentState State { get; set; }
    public int ProductionAreaId { get; set; }
    public string? ProductionAreaName { get; set; }
}