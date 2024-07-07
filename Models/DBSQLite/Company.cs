using System;
using System.Collections.Generic;

namespace Extractor.Models.DBSQLite;

public partial class Company
{
    public string CIK { get; set; } = null!;
    public string entityName { get; set; } = null!;
}
