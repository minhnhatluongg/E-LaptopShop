using Microsoft.AspNetCore.Authorization;

namespace E_LaptopShop.Helpers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AdminOnlyAttribute : AuthorizeAttribute
{
    public AdminOnlyAttribute() { Roles = ApiRoles.Admin; }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AdminOrManagerAttribute : AuthorizeAttribute
{
    public AdminOrManagerAttribute() { Roles = ApiRoles.AdminOrManager; }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AdminManagerOrWarehouseAttribute : AuthorizeAttribute
{
    public AdminManagerOrWarehouseAttribute() { Roles = ApiRoles.AdminManagerOrWarehouse; }
}
