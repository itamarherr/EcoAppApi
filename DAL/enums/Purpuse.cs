using System.ComponentModel.DataAnnotations;

namespace DAL.enums;

public enum Purpose
{
    [Display(Name = "Before Construction")]
    BeforeConstruction = 1 ,
    [Display(Name = "Dislocations")]
    Dislocations = 2,
    [Display(Name = "Trees Illness")]
    TreesIllness = 3
}
