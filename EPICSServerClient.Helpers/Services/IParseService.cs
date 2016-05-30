using EPICSServerClient.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EPICSServerClient.Helpers.Services
{
    public interface IParseService
    {
        List<TabItem> GetClasses();
        List<ParseObject> GetClassObjects(string Class);
    }
}
