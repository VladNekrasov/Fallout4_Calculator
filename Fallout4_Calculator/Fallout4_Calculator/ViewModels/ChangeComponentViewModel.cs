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
    public class ChangeComponentViewModel : BaseViewModel
    {
        string content = "";
        string te_Search;
        Components components;
        int include_component = 0;

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
        public string TE_Search
        {
            get => te_Search;
            set => SetProperty(ref te_Search, value);
        }
        public Components SelectedComponent
        {
            get => components;
            set => SetProperty(ref components, value);
        }
        public int IncludeComponent
        {
            get => include_component;
            set => SetProperty(ref include_component, value);
        }

        public ObservableRangeCollection<Components> O_Component { get; set; }
        public ObservableRangeCollection<Components> O_Component_Copy { get; set; }
        public ObservableRangeCollection<object> L_Components_Selected { get; set; }
        public ObservableRangeCollection<object> L_Components_Selected_Copy { get; set; }


        public Command SearchCommand { get; }
        public AsyncCommand AddComponentCommand { get; }
        public AsyncCommand ComponentAppearingCommand { get; }
        public Command ComponentDisappearingCommand { get; }
        public AsyncCommand SelectComponentCommand { get; }
        public AsyncCommand AddAmountComponentCommand { get; }
    

        public ChangeComponentViewModel()
        {
            O_Component = new ObservableRangeCollection<Components>();
            O_Component_Copy = new ObservableRangeCollection<Components>();
            L_Components_Selected = new ObservableRangeCollection<object>();
            L_Components_Selected_Copy = new ObservableRangeCollection<object>();
            SearchCommand = new Command(Search);
            AddComponentCommand = new AsyncCommand(AddComponent);
            ComponentAppearingCommand = new AsyncCommand(ComponentAppearing);
            ComponentDisappearingCommand = new Command(ComponentDisappearing);
            SelectComponentCommand = new AsyncCommand(SelectComponent);
            AddAmountComponentCommand = new AsyncCommand(AddAmountComponent);
        }

        void ComponentDisappearing()
        {

        }
        async Task ComponentAppearing()
        {
            IsBusy = true;
            await Task.Delay(1000);
            O_Component.Clear();
            O_Component_Copy.Clear();
            var new_Component = await App.Database.GetComponentsAsync();
            O_Component.AddRange(new_Component);
            O_Component_Copy.AddRange(new_Component);
            Search();
            IsBusy = false;
            if (IncludeComponent==0) Title = "Редактор компонентов";
            else
            {
                Title = "Выберите компоненты";
                foreach (var x in L_Components_Selected)
                    L_Components_Selected_Copy.Add(x);
                    
            }
        }
        async Task AddComponent()
        {
            O_Component.Clear();
            O_Component_Copy.Clear();
            L_Components_Selected.Clear();
            L_Components_Selected_Copy.Clear();
            TE_Search = String.Empty;
            await AppShell.Current.GoToAsync(nameof(CreateComponentPage));

        }
        async Task SelectComponent()
        {
            if (SelectedComponent != null && IncludeComponent==0)
            {
                int id = SelectedComponent.ID_Component;
                var com = new Components
                {
                    ID_Component = SelectedComponent.ID_Component,
                };
                O_Component.Clear();
                O_Component_Copy.Clear();
                var jsonStr = JsonConvert.SerializeObject(com);
                await AppShell.Current.GoToAsync($"{nameof(ShowComponentPage)}?Content={jsonStr}");
            }
            if (IncludeComponent!=0 && !IsBusy)
            {
                foreach (var x in O_Component)
                {
                    if (L_Components_Selected.Contains(x))
                    {
                        if (!L_Components_Selected_Copy.Contains(x))
                            L_Components_Selected_Copy.Add(x);
                    }
                    else L_Components_Selected_Copy.Remove(x);
                }
            }
        }
        async Task AddAmountComponent()
        {
            var L_index = new List<int>();
            foreach (Components x in L_Components_Selected_Copy)
                L_index.Add(x.ID_Component);
            var jsonStr = JsonConvert.SerializeObject(L_index);
            if (IncludeComponent==1)
            await AppShell.Current.GoToAsync($"{"//DataBasePage//CreateObjectPage"}?Content={jsonStr}");
            if (IncludeComponent==2)
            await AppShell.Current.GoToAsync($"{"//DataBasePage//ShowObjectPage"}?NextContent={jsonStr}");
            if (IncludeComponent == 3)
            await AppShell.Current.GoToAsync($"{"//DataBasePage//CreateJunkPage"}?Content={jsonStr}");
            if (IncludeComponent == 4)
            await AppShell.Current.GoToAsync($"{"//DataBasePage//ShowJunkPage"}?NextContent={jsonStr}");
        }
        void Search()
        {
            IsBusy = true;
            var text = TE_Search;
            if (IncludeComponent!=0)
                L_Components_Selected.Clear();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var filteredO = O_Component_Copy.Where(x => x.Nam_Com.ToLower().Contains(text.ToLower())).ToList();
                O_Component.Clear();
                foreach (var ob in filteredO)
                {
                   O_Component.Add(ob);
                    if (IncludeComponent!=0 && L_Components_Selected_Copy.Contains(ob))
                       L_Components_Selected.Add(ob);
                }
            }
            else
            {
                O_Component.Clear();
                foreach (var obc in O_Component_Copy)
                {
                    O_Component.Add(obc);
                    if (IncludeComponent!=0 && L_Components_Selected_Copy.Contains(obc))
                        L_Components_Selected.Add(obc);
                }
            }
            IsBusy = false;
        }
        private  void PerformOperation(string getcont)
        {
            var content = JsonConvert.DeserializeObject<int>(getcont);
            IncludeComponent =content;
        }     
    }
}
