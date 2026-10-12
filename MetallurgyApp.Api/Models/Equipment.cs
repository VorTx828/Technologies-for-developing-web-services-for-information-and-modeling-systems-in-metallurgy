using System.ComponentModel.DataAnnotations;

namespace MetallurgyApp.Api.Models;

public enum EquipmentState
{
    [Display(Name = "Работает")] Working,
    [Display(Name = "На ремонте")] UnderRepair,
    [Display(Name = "Законсервировано")] Mothballed,
    [Display(Name = "Списано")] Decommissioned
}

public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public int ManufactureYear { get; set; }
    public EquipmentState State { get; set; }
    public int ProductionAreaId { get; set; }
    public ProductionArea? ProductionArea { get; set; }
}

public class EquipmentCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public int ManufactureYear { get; set; }
    public EquipmentState State { get; set; }
    public int ProductionAreaId { get; set; }
}