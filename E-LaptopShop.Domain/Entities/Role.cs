using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = "";

    public bool IsActive { get; set; } = true;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
