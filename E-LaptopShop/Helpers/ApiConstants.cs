namespace E_LaptopShop.Helpers;

public static class ApiRoles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Warehouse = "Warehouse";
    public const string AdminOrManager = Admin + "," + Manager;
    public const string AdminManagerOrWarehouse = Admin + "," + Manager + "," + Warehouse;
}

public static class ApiTags
{
    public const string Authentication = "🔐 Authentication";
    public const string Customer = "👤 Customer";
    public const string Public = "🔓 Public";
    public const string Admin = "👑 Admin";
    public const string InventoryHistory = "📊 Inventory History";
}
