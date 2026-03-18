using System;
using System.Collections.Generic;

namespace MyApiProject.Models;

public partial class SysUser
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }
}
