using System;
using System.Collections.Generic;

namespace FinalProject_3K1D.Models;

public partial class TheLoai
{
    public string IdTheLoai { get; set; } = null!;

    public string TenTheLoai { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<Phim> IdPhims { get; set; } = new List<Phim>();
}
