using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBoards.Data.Cors;
public class CorsSettings
{
    public static string SectionName { get; } = "CorsSettings";
    public string? MVC { get; set; }
}
