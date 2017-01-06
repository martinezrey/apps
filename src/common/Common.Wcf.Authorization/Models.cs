using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Wcf.Authorization
{
    public class Org
    {
        public string Name { get; set; }
        public List<App> Apps { get; set; }
        public List<string> GlobalRoles { get; set; }
    }

    public class App
    {
        public string Name { get; set; }
        public List<string> Roles { get; set; }
    }
}
