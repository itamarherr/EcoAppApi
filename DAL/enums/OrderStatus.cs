using System.ComponentModel.DataAnnotations;

namespace DAL.enums;

public enum OrderStatus
{

    [Display (Name = "pending")]
    pending = 1,
    [Display (Name = "approved")]
    approved = 2,
    [Display (Name = "rejected")]
    rejected = 3,
    [Display (Name = "inProgress")]
    inProgress = 4,
    [Display (Name = "completed")]
    completed = 5,
}
