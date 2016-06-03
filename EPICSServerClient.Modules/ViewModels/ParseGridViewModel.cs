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
using MahApps.Metro.Controls;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using System.Text.RegularExpressions;
using EPICSServerClient.Helpers.Constants;

namespace EPICSServerClient.Modules.ViewModels
{
    public class ParseGridViewModel : ViewModelBase
    {
        private ObservableCollection<PFObject> parseObjects = new ObservableCollection<PFObject>();
        private IParseService ParseService { get; set; }
        private IRegionManager RegionManager { get; set; }
        private Boolean isLoading = false;
        private DelegateCommand addRowCommand { get; set; }
        private DelegateCommand<object> dirtyRowCommand { get; set; }
        private DelegateCommand saveRowCommand { get; set; }
        private DelegateCommand<object> deleteRowCommand { get; set; }
        private DelegateCommand deleteColumnCommand { get; set; }
        private DelegateCommand dropClassCommand { get; set; }
        private DelegateCommand addColumnCommand { get; set; }
        private PFObject CurrentObject = null;
        private List<PFObject> DirtyRows = new List<PFObject>();

        public ParseGridViewModel(IRegionManager RegionManager, IParseService ParseService)
        {
            this.RegionManager = RegionManager;
            this.ParseService = ParseService;
            AddRowCommand = new DelegateCommand(AddRow);
            DirtyRowCommand = new DelegateCommand<object>(SelectDirtyRow);
            SaveRowCommand = new DelegateCommand(SaveDirtyRow);
            AddColumnCommand = new DelegateCommand(AddColumn);
            DeleteRowCommand = new DelegateCommand<object>(DeleteRow, CanDeleteRow);
            DeleteColumnCommand = new DelegateCommand(DeleteColumn);
            DropClassCommand = new DelegateCommand(DropClass);
        }

        public ObservableCollection<PFObject> ParseObjects
        {
            get { return parseObjects; }
            set { parseObjects = value; }
        }

        public DelegateCommand DropClassCommand
        {
            get { return dropClassCommand; }
            set { dropClassCommand = value; }
        }

        public DelegateCommand AddRowCommand
        {
            get { return addRowCommand; }
            set { addRowCommand = value; }
        }

        public DelegateCommand<object> DirtyRowCommand
        {
            get { return dirtyRowCommand; }
            set { dirtyRowCommand = value; }
        }

        public DelegateCommand SaveRowCommand
        {
            get { return saveRowCommand; }
            set { saveRowCommand = value; }
        }

        public DelegateCommand<object> DeleteRowCommand
        {
            get { return deleteRowCommand; }
            set { deleteRowCommand = value; }
        }

        public DelegateCommand DeleteColumnCommand
        {
            get { return deleteColumnCommand; }
            set { deleteColumnCommand = value; }
        }

        public DelegateCommand AddColumnCommand
        {
            get { return addColumnCommand; }
            set { addColumnCommand = value; }
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

        private async void AddColumn()
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            if (ParseData.CurrentClass == "Class")
            {
                await metroWindow.ShowMessageAsync("Uh oh.", "You cannot add columns to 'Class' because you don't need to! Everything you need is already here!");
                return;
            }
            var columnName = await metroWindow.ShowInputAsync("So you want to add a column?", "Well, give it a name! Remember, no spaces!");
            if (columnName == null || columnName == String.Empty)
            {
                return;
            }
            Regex regex = new Regex("^[a-zA-Z0-9_]*$");
            if (!regex.IsMatch(columnName))
            {
                await metroWindow.ShowMessageAsync("Uh oh.", "It looks like your column name was invalid. Try again! Remember, only alphanumeric characters are allowed with underscores! No spaces!");
                return;
            }
            foreach (var obj in ParseObjects)
                obj.AddProperty(columnName, String.Empty);
            ParseData.AddDataGridColumn(columnName);
        }

        private bool CanAddColumn(object parm)
        {
            return true;
        }

        private void AddRow()
        {
            PFObject obj = new PFObject();
            foreach (var column in ParseData.dataGrid.Columns)
            {
                obj.AddProperty(column.Header.ToString(), "");
            }
            ParseObjects.Add(obj);
        }

        private void SelectDirtyRow(object obj)
        {
            try
            {
                CurrentObject = obj as PFObject;
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message + " : Could not cast. Auto assigning to null.");
                CurrentObject = null;
            }
            if (CurrentObject != null && !DirtyRows.Contains(CurrentObject))
                DirtyRows.Add(CurrentObject);
            SaveRowCommand.RaiseCanExecuteChanged();
            DeleteRowCommand.RaiseCanExecuteChanged();
        }

        private void SaveDirtyRow()
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
            SelectDirtyRow(null);
            if(ParseData.CurrentClass == "Class")
                ReloadMenu();
        }

        private bool CanSaveRow()
        {
            return true;
        }

        private void DeleteRow(object parm)
        {
            ParseService.DeleteClassObject(ParseData.CurrentClass, CurrentObject);
            ParseObjects.Remove(CurrentObject);
            DirtyRows.Remove(CurrentObject);
            SelectDirtyRow(null);
        }

        private bool CanDeleteRow(object parm)
        {
            return (CurrentObject != null);
        }

        private async void DeleteColumn()
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            if (ParseData.CurrentClass == "Class")
            {
                await metroWindow.ShowMessageAsync("Uh oh.", "You cannot delete columns from 'Class' because you don't need to! Everything you need is already here!");
                return;
            }
            var columnName = await metroWindow.ShowInputAsync("So you want to delete a column?", "Well, what's its name? Capitalization counts!");
            if (columnName == null || columnName == String.Empty)
            {
                return;
            }
            if (!ParseData.DeleteDataGridColumn(columnName))
            {
                await metroWindow.ShowMessageAsync("Uh oh.", "It looks like your column doesn't exist! Check the spelling and capitalization!");
                return;
            }
            foreach (var obj in ParseObjects)
            {
                var prop = obj.Properties.FirstOrDefault(p => p.Name == columnName);
                obj.Properties.Remove(prop);
                var objectId = ParseService.UpdateOnDelete(ParseData.CurrentClass, obj);
                if (objectId != String.Empty)
                    obj.SetPropertyValue("objectId", objectId);
            }
        }

        private async void DropClass()
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            if (ParseData.CurrentClass == "Class")
            {
                await metroWindow.ShowMessageAsync("Uh oh.", "You cannot drop this class because you don't need to! This application is dependent on this, so you don't want to either!");
                return;
            }
            var className = await metroWindow.ShowInputAsync("So you want to delete a class?", "Well, what's its name? Capitalization counts! Remember, once the data is gone, it's gone forever!");
            if (className == null || className == String.Empty)
            {
                return;
            }
            if (ParseData.CurrentClass != className)
            {
                await metroWindow.ShowMessageAsync("Uh oh.", "It looks like your class doesn't exist! Check the spelling and capitalization!");
                return;
            }
            foreach (var obj in ParseObjects)
            {
                ParseService.DeleteClassObject(ParseData.CurrentClass, obj);
            }
            List<PFObject> objs = ParseService.GetClassObjects("Class");
            var classObj = objs.FirstOrDefault(o => o.Properties.FirstOrDefault(p => p.Name == "name").Value.ToString() == className);
            ParseService.DeleteClassObject("Class", classObj);
            ReloadMenu();
        }

        private void PopulateParseData(List<PFObject> objs)
        {
            if (objs == null)
                return;
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

        private void ReloadMenu()
        {
            var uri = new Uri(typeof(MenuView).FullName, UriKind.Relative);
            RegionManager.RequestNavigate(RegionConstants.MenuRegion, uri);
        }
    }
}
