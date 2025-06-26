using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DEF.Manager;

public class CfgItem
{
    public string _id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}

public class CfgInfo
{
    public string _id { get; set; }
    public string NameSpace { get; set; }
    public List<CfgItem> ListCfgItem { get; set; } = new();
}