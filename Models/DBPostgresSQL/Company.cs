using System;
using System.Collections.Generic;

namespace Extractor.Models.DBPostgresSQL;

public partial class Company
{
    public string Cik { get; set; } = null!;

    public string Entityname { get; set; } = null!;
}
