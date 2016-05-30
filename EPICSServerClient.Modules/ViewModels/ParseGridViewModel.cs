using EPICSServerClient.Helpers.Data;
using EPICSServerClient.Helpers.Models;
using EPICSServerClient.Helpers.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Regions;
using System.Diagnostics;

namespace EPICSServerClient.Modules.ViewModels
{
    public class ParseGridViewModel : ViewModelBase
    {
        private ObservableCollection<ParseObject> parseObjects = new ObservableCollection<ParseObject>();
        private IParseService ParseService { get; set; }

        public ParseGridViewModel(IParseService ParseService)
        {
            this.ParseService = ParseService;
            PopulateParseData();
        }

        public ObservableCollection<ParseObject> ParseObjects
        {
            get { return parseObjects; }
            set { parseObjects = value; }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            ParseObjects.Clear();
            PopulateParseData();
            ParseData.PopulateDataGrid(null);
        }

        private void PopulateParseData()
        {
            foreach (ParseObject obj in ParseService.GetClassObjects(ParseData.CurrentClass))
            {
                ParseObjects.Add(obj);
            }
        }
    }
}
