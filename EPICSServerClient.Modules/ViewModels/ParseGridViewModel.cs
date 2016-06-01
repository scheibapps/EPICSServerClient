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
using EPICSServerClient.Modules.Views;
using Prism.Commands;

namespace EPICSServerClient.Modules.ViewModels
{
    public class ParseGridViewModel : ViewModelBase
    {
        private ObservableCollection<PFObject> parseObjects = new ObservableCollection<PFObject>();
        private IParseService ParseService { get; set; }
        private Boolean isLoading = false;
        private DelegateCommand addRowCommand { get; set; }
        private DelegateCommand<PFObject> dirtyRowCommand { get; set; }
        private DelegateCommand<object> saveRowCommand { get; set; }
        private DelegateCommand<object> deleteRowCommand { get; set; }
        private PFObject CurrentObject = null;
        private List<PFObject> DirtyRows = new List<PFObject>();
        private bool IsDirty = false;

        public ParseGridViewModel(IParseService ParseService)
        {
            this.ParseService = ParseService;
            AddRowCommand = new DelegateCommand(AddRow);
            DirtyRowCommand = new DelegateCommand<PFObject>(SelectDirtyRow);
            SaveRowCommand = new DelegateCommand<object>(SaveDirtyRow, CanSaveRow);
            DeleteRowCommand = new DelegateCommand<object>(DeleteRow, CanDeleteRow);
        }

        public ObservableCollection<PFObject> ParseObjects
        {
            get { return parseObjects; }
            set { parseObjects = value; }
        }

        public DelegateCommand AddRowCommand
        {
            get { return addRowCommand; }
            set { addRowCommand = value; }
        }

        public DelegateCommand<PFObject> DirtyRowCommand
        {
            get { return dirtyRowCommand; }
            set { dirtyRowCommand = value; }
        }

        public DelegateCommand<object> SaveRowCommand
        {
            get { return saveRowCommand; }
            set { saveRowCommand = value; }
        }

        public DelegateCommand<object> DeleteRowCommand
        {
            get { return deleteRowCommand; }
            set { deleteRowCommand = value; }
        }

        public Boolean IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        private void AddRow()
        {
            PFObject obj = new PFObject();
            foreach(var column in ParseData.dataGrid.Columns)
            {
                obj.AddProperty(column.Header.ToString(), "");
            }
            ParseObjects.Add(obj);
        }

        private void SelectDirtyRow(PFObject obj)
        {
            CurrentObject = obj;
            if(CurrentObject != null && !DirtyRows.Contains(CurrentObject))
                DirtyRows.Add(CurrentObject);
            IsDirty = true;
            SaveRowCommand.RaiseCanExecuteChanged();
            DeleteRowCommand.RaiseCanExecuteChanged();
        }

        private void SaveDirtyRow(object parm)
        {
            IsLoading = true;
            foreach (var obj in DirtyRows)
            {
                var objectId = ParseService.PostClassObject(ParseData.CurrentClass, obj);
                if (objectId != String.Empty)
                    obj.SetPropertyValue("objectId", objectId);
            }
            IsLoading = false;
            DirtyRows.Clear();
            IsDirty = false;
            SelectDirtyRow(null);
        }

        private void DeleteRow(object parm)
        {
            ParseService.DeleteClassObject(ParseData.CurrentClass, CurrentObject);
            ParseObjects.Remove(CurrentObject);
            DirtyRows.Remove(CurrentObject);
            SelectDirtyRow(null);
        }

        private bool CanSaveRow(object parm)
        {
            return true;
        }

        private bool CanDeleteRow(object parm)
        {
            return (CurrentObject != null);
        }

        private void PopulateParseData(List<PFObject> objs)
        {
            foreach (PFObject obj in objs)
            {
                ParseObjects.Add(obj);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (ParseData.AppId == String.Empty && ParseData.Url == String.Empty)
                return;
            else
            {
                ParseObjects.Clear();
                PopulateParseData(navigationContext.Parameters["Class"] as List<PFObject>);
                if (ParseData.dataGrid != null)
                    ParseData.PopulateDataGrid(null);
            }
            CurrentObject = null;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            SaveDirtyRow(null);
        }
    }
}
