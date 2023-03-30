using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fallout4_Calculator.Models;
using Fallout4_Calculator.Views;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Newtonsoft.Json;


namespace Fallout4_Calculator.ViewModels
{
    [Xamarin.Forms.QueryProperty(nameof(Content), nameof(Content))]
    public class ChangeObjectViewModel : BaseViewModel
    {
        string content = "";
        string te_Search;
        Objects objects;
        string toolbarText = "Добавить объект";
        int include_object = 0;


        public string ToolbarText
        {
            get => toolbarText;
            set => SetProperty(ref toolbarText, value);
        }
        public string TE_Search
        {
            get => te_Search;
            set => SetProperty(ref te_Search, value);
        }
        public Objects SelectedObject
        {
            get => objects;
            set => SetProperty(ref objects, value);
        }
        public string Content
        {
            get => content;
            set
            {
                content = Uri.UnescapeDataString(value ?? string.Empty);
                OnPropertyChanged();
                PerformOperation(content);
            }
        }
        public int IncludeObject
        {
            get => include_object;
            set => SetProperty(ref include_object, value);
        }



        public ObservableRangeCollection<Objects> O_Object { get; set; }
        public ObservableRangeCollection<Objects> O_Object_Copy { get; set; }
        public ObservableRangeCollection<object> L_Objects_Selected { get; set; }
        public ObservableRangeCollection<object> L_Objects_Selected_Copy { get; set; }



        public Command SearchCommand { get; }
        public AsyncCommand AddObjectCommand { get; }
        public AsyncCommand ObjectAppearingCommand { get; }
        public Command ObjectDisappearingCommand { get; }
        public AsyncCommand SelectObjectCommand { get; }
        public AsyncCommand AddAmountObjectCommand { get; }

        public ChangeObjectViewModel()
        {
            O_Object = new ObservableRangeCollection<Objects>();
            O_Object_Copy = new ObservableRangeCollection<Objects>();
            SearchCommand = new Command(Search);
            AddObjectCommand = new AsyncCommand(AddObject);
            ObjectAppearingCommand = new AsyncCommand(ObjectAppearing);
            ObjectDisappearingCommand = new Command(ObjectDisappearing);
            SelectObjectCommand = new AsyncCommand(SelectObject);
            L_Objects_Selected = new ObservableRangeCollection<object>();
            L_Objects_Selected_Copy = new ObservableRangeCollection<object>();
            AddAmountObjectCommand = new AsyncCommand(AddAmountObject);
        }

        void ObjectDisappearing()
        {
          
        }
        async Task ObjectAppearing()
        {
            IsBusy = true;
            await Task.Delay(1000);
            O_Object.Clear();
            O_Object_Copy.Clear();
            var new_Object = await App.Database.GetObjectsAsync();
            O_Object.AddRange(new_Object);
            O_Object_Copy.AddRange(new_Object);
            Search();
            IsBusy = false;
            if (IncludeObject==0) Title = "Редактор объектов";
            else
            {
                Title = "Выберите объекты";
                foreach (var x in L_Objects_Selected)
                    L_Objects_Selected_Copy.Add(x);
            }
        }
        async Task AddObject()
        {
            if (IncludeObject != 1)
            {
                O_Object.Clear();
                O_Object_Copy.Clear();
                L_Objects_Selected.Clear();
                L_Objects_Selected_Copy.Clear();
                TE_Search = String.Empty;
                await AppShell.Current.GoToAsync(nameof(CreateObjectPage));
            }
        }
        async Task SelectObject()
        {
            if (SelectedObject != null && IncludeObject==0)
            {
                int id = SelectedObject.ID_0bject;
                var com = new Objects
                {
                    ID_0bject = SelectedObject.ID_0bject,
                };
                O_Object.Clear();
                O_Object_Copy.Clear();
                var jsonStr = JsonConvert.SerializeObject(com);
                await AppShell.Current.GoToAsync($"{nameof(ShowObjectPage)}?Content={jsonStr}");
            }
            if (IncludeObject != 0 && !IsBusy)
            {
                foreach (var x in O_Object)
                {
                    if (L_Objects_Selected.Contains(x))
                    {
                        if (!L_Objects_Selected_Copy.Contains(x))
                            L_Objects_Selected_Copy.Add(x);
                    }
                    else L_Objects_Selected_Copy.Remove(x);
                }
            }
        }
        async Task AddAmountObject()
        {
            var L_index = new List<int>();
            foreach (Objects x in L_Objects_Selected_Copy)
                L_index.Add(x.ID_0bject);
            var jsonStr = JsonConvert.SerializeObject(L_index);
            if (IncludeObject == 1)
                await AppShell.Current.GoToAsync($"{"//BuildCalculatorPage"}?Content={jsonStr}");
        }
        void Search()
        {
            IsBusy = true;
            var text = TE_Search;
            if (IncludeObject != 0)
                L_Objects_Selected.Clear();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var filteredO = O_Object_Copy.Where(x => x.Nam_Obj.ToLower().Contains(text.ToLower())).ToList();
                O_Object.Clear();
                foreach (var ob in filteredO)
                {
                    O_Object.Add(ob);
                    if (IncludeObject != 0 && L_Objects_Selected_Copy.Contains(ob))
                        L_Objects_Selected.Add(ob);
                }
            }
            else
            {
                O_Object.Clear();
                foreach (var obc in O_Object_Copy)
                {
                    O_Object.Add(obc);
                    if (IncludeObject != 0 && L_Objects_Selected_Copy.Contains(obc))
                        L_Objects_Selected.Add(obc);
                }

            }
            IsBusy = false;
        }
        private void PerformOperation(string getcont)
        {
            var content = JsonConvert.DeserializeObject<int>(getcont);
            IncludeObject = content;
            if (IncludeObject == 1)
                ToolbarText = "";
        }
    }
}
