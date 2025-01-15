using DAL.enums;

namespace EcoAppApi.Utils;

public class PricingService
{
    public decimal CalculatePrice(Purpose consutancyType, int numberOfTrees, bool isPrivate)
    {
        decimal basePrice = consutancyType switch
        {
            Purpose.BeforeConstruction => 1000m,
            Purpose.Dislocations => 1500m,
            Purpose.TreesIllness => 2000m,
            _ => 1000m
        };
        decimal treeMultiplier = numberOfTrees switch
        {
            1 => 1.0m,
            > 1 and <= 5 => 1.2m,
            > 5 and <= 10 => 1.3m,
            > 10 => 1.5m
        };
        decimal privateAreaMultiplier = isPrivate ? 1.0m : 1.2m;

        return basePrice * treeMultiplier * privateAreaMultiplier;
    }
}
