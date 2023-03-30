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
    public class ChangeJunkViewModel : BaseViewModel
    {
        string content = "";
        string te_Search;
        Junk junk;
        string toolbarText = "Добавить хлам";
        int include_junk = 0;
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
        public Junk SelectedJunk
        {
            get => junk;
            set => SetProperty(ref junk, value);
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
        public int IncludeJunk
        {
            get => include_junk;
            set => SetProperty(ref include_junk, value);
        }

        public ObservableRangeCollection<Junk> O_Junk { get; set; }
        public ObservableRangeCollection<Junk> O_Junk_Copy { get; set; }
        public ObservableRangeCollection<object> L_Junk_Selected { get; set; }
        public ObservableRangeCollection<object> L_Junk_Selected_Copy { get; set; }


        public Command SearchCommand { get; }
        public AsyncCommand AddJunkCommand { get; }
        public AsyncCommand JunkAppearingCommand { get; }
        public Command JunkDisappearingCommand { get; }
        public AsyncCommand SelectJunkCommand { get; }
        public AsyncCommand AddAmountJunkCommand { get; }

        public ChangeJunkViewModel()
        {
            Title = "Редактор компонентов";
            O_Junk = new ObservableRangeCollection<Junk>();
            O_Junk_Copy = new ObservableRangeCollection<Junk>();
            SearchCommand = new Command(Search);
            AddJunkCommand = new AsyncCommand(AddJunk);
            JunkAppearingCommand = new AsyncCommand(JunkAppearing);
            JunkDisappearingCommand = new Command(JunkDisappearing);
            SelectJunkCommand = new AsyncCommand(SelectJunk);
            L_Junk_Selected = new ObservableRangeCollection<object>();
            L_Junk_Selected_Copy = new ObservableRangeCollection<object>();
            AddAmountJunkCommand = new AsyncCommand(AddAmountJunk);
        }

        void JunkDisappearing()
        {

        }
        async Task JunkAppearing()
        {
            IsBusy = true;
            await Task.Delay(1000);
            O_Junk.Clear();
            O_Junk_Copy.Clear();
            var new_Junk = await App.Database.GetJunkAsync();
            O_Junk.AddRange(new_Junk);
            O_Junk_Copy.AddRange(new_Junk);
            Search();
            IsBusy = false;
            if (IncludeJunk == 0) Title = Title = "Редактор хлама";
            else
            {
                Title = "Выберите хлам";
                foreach (var x in L_Junk_Selected)
                    L_Junk_Selected_Copy.Add(x);
            }
        }
        async Task AddJunk()
        {
            if (IncludeJunk != 1)
            {
                O_Junk.Clear();
                O_Junk_Copy.Clear();
                L_Junk_Selected.Clear();
                L_Junk_Selected_Copy.Clear();
                TE_Search = String.Empty;
                await AppShell.Current.GoToAsync(nameof(CreateJunkPage));
            }
        }
        async Task SelectJunk()
        {
            if (SelectedJunk != null && IncludeJunk == 0)
            {
                int id = SelectedJunk.ID_Junk;
                var com = new Junk
                {
                    ID_Junk = SelectedJunk.ID_Junk,
                };
                O_Junk.Clear();
                O_Junk_Copy.Clear();
                var jsonStr = JsonConvert.SerializeObject(com);
                await AppShell.Current.GoToAsync($"{nameof(ShowJunkPage)}?Content={jsonStr}");
            }
            if (IncludeJunk != 0 && !IsBusy)
            {
                foreach (var x in O_Junk)
                {
                    if (L_Junk_Selected.Contains(x))
                    {
                        if (!L_Junk_Selected_Copy.Contains(x))
                            L_Junk_Selected_Copy.Add(x);
                    }
                    else L_Junk_Selected_Copy.Remove(x);
                }
            }
        }
        async Task AddAmountJunk()
        {
            var L_index = new List<int>();
            foreach (Junk x in L_Junk_Selected_Copy)
                L_index.Add(x.ID_Junk);
            var jsonStr = JsonConvert.SerializeObject(L_index);
            if (IncludeJunk == 1)
                await AppShell.Current.GoToAsync($"{"//BuildCalculatorPage"}?Content={jsonStr}");
        }
        void Search()
        {
            IsBusy = true;
            var text = TE_Search;
            if (IncludeJunk != 0)
                L_Junk_Selected.Clear();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var filteredO = O_Junk_Copy.Where(x => x.Nam_Jun.ToLower().Contains(text.ToLower())).ToList();
                O_Junk.Clear();
                foreach (var ob in filteredO)
                {
                    O_Junk.Add(ob);
                    if (IncludeJunk != 0 && L_Junk_Selected_Copy.Contains(ob))
                        L_Junk_Selected.Add(ob);
                }
            }
            else
            {
                O_Junk.Clear();
                foreach (var obc in O_Junk_Copy)
                {
                    O_Junk.Add(obc);
                    if (IncludeJunk != 0 && L_Junk_Selected_Copy.Contains(obc))
                        L_Junk_Selected.Add(obc);
                }

            }
            IsBusy = false;
        }
        private void PerformOperation(string getcont)
        {
            var content = JsonConvert.DeserializeObject<int>(getcont);
            IncludeJunk = content;
            if (IncludeJunk == 1)
                ToolbarText = "";
        }
    }
}
