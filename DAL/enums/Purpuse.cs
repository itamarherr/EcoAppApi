using System.ComponentModel.DataAnnotations;

namespace DAL.enums;

public enum Purpose
{
    [Display(Name = "Before Construction")]
    BeforeConstruction,
    [Display(Name = "Dislocations")]
    Dislocations,
    [Display(Name = "Trees Illness")]
    TreesIllness
}
