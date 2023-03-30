using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Fallout4_Calculator.Views;
using Fallout4_Calculator.Models;
using Newtonsoft.Json;

namespace Fallout4_Calculator.ViewModels
{
    [Xamarin.Forms.QueryProperty(nameof(Content), nameof(Content))]
    public class BuildCalculatorViewModel:BaseViewModel
    {
        string content = "";
        byte is_object_content = 0;
        bool isLoaded = false;
        int ib=0;
        int ib_j = 0;
        int id_bad = 0;
        int id_bad_j = 0;

        public bool IsLoaded
        {
            get => isLoaded;
            set => SetProperty(ref isLoaded, value);
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

        public ObservableRangeCollection<Objects_A> Amount_Object { get; set; }
        public ObservableRangeCollection<Objects_A> Amount_Object_Old { get; set; }
        public ObservableRangeCollection<Junk_A> Amount_Junk { get; set; }
        public ObservableRangeCollection<Junk_A> Amount_Junk_Old { get; set; }
        public ObservableRangeCollection<Components_Calculator> Calculator_Components { get;set; }
        public ObservableRangeCollection<Components_Calculator> Calculator_Components_Copy { get; set; }


        public Command BuildCalculatorAppearingCommand { get; }
        public Command BuildCalculatorDisappearingCommand { get; }
        public AsyncCommand AddObjectCommand { get; }
        public AsyncCommand<ObjectWithEntry> AmountObjectComponentCommand { get; }
        public Command<ObjectWithEntry> AmountObjectComponentBidingCommand { get; }
        public AsyncCommand<ObjectWithEntry> AmountObjectComponentDeleteCommand { get; }
        public AsyncCommand AddJunkCommand { get; }
        public AsyncCommand<JunkWithEntry> AmountJunkComponentCommand { get; }
        public Command<JunkWithEntry> AmountJunkComponentBidingCommand { get; }
        public AsyncCommand<JunkWithEntry> AmountJunkComponentDeleteCommand { get; }

        public BuildCalculatorViewModel()
        {
            Title = "Калькулятор ресурсов";
            Amount_Object = new ObservableRangeCollection<Objects_A>();
            Amount_Object_Old = new ObservableRangeCollection<Objects_A>();
            Amount_Junk = new ObservableRangeCollection<Junk_A>();
            Amount_Junk_Old = new ObservableRangeCollection<Junk_A>();
            Calculator_Components = new ObservableRangeCollection<Components_Calculator>();
            Calculator_Components_Copy = new ObservableRangeCollection<Components_Calculator>();
            BuildCalculatorAppearingCommand = new Command(BuildCalculatorAppearing);
            BuildCalculatorDisappearingCommand = new Command(BuildCalculatorDisappearing);
            AddObjectCommand = new AsyncCommand(AddObject);
            AddJunkCommand = new AsyncCommand(AddJunk);
            AmountObjectComponentCommand = new AsyncCommand<ObjectWithEntry>(AmountObjectComponent);
            AmountObjectComponentBidingCommand = new Command<ObjectWithEntry>(AmountObjectComponentBiding);
            AmountObjectComponentDeleteCommand = new AsyncCommand<ObjectWithEntry>(AmountObjectComponentDelete);
            AmountJunkComponentCommand = new AsyncCommand<JunkWithEntry>(AmountJunkComponent);
            AmountJunkComponentBidingCommand = new Command<JunkWithEntry>(AmountJunkComponentBiding);
            AmountJunkComponentDeleteCommand = new AsyncCommand<JunkWithEntry>(AmountJunkComponentDelete);
        }

        void BuildCalculatorAppearing()
        {
            
            Amount_Object.Clear();
            Amount_Junk.Clear();
        }
        void BuildCalculatorDisappearing()
        {
            if (IsLoaded != false)
            {
                Amount_Object.Clear();
                Amount_Junk.Clear();
            }
        }
        async Task AmountObjectComponent(ObjectWithEntry objectWithEntry)
        {
            var deleteitem = objectWithEntry as ObjectWithEntry;
            //текущий объект и поле для ввода
            Objects_A curent_object = deleteitem.objects_A;
            var curent_entry = deleteitem.entry;
            if (curent_object.ID_0bject != id_bad)
               ib = 0;

            //старое и новое значения поля для ввода
            string OldEntry = curent_object.Amount.ToString();
            string NewEntry = curent_entry.Text;

            //ввод количества
            int _;
            bool t = int.TryParse(NewEntry, out _);
            if (_>1000)
            {
                _=0;
                NewEntry = "error";
                t = false;
            }
            if (!t)
            {
                if (NewEntry != String.Empty)
                {
                    curent_entry.Text = OldEntry;
                    ib++;
                    id_bad=curent_object.ID_0bject;
                }
                else
                    curent_object.Amount = 0;
            }
            else
            {
                if (_ < 0)
                {
                    curent_entry.Text = OldEntry;
                    ib++;
                    id_bad = curent_object.ID_0bject;
                }
                else
                {
                    curent_object.Amount = _;
                }
            }

            //компоненты текущего объекта
            List<Components> curent_components= await App.Database.GetComponents_Components_Object(curent_object.ID_0bject);

            //подсчет количества компонентов
            foreach (var curent_components_item in curent_components)
            {
                //присутвует ли компонет в списке компонентов
                bool contain_component = false;

                int it = -1;

                //поиск компонента в списке компонентов
                foreach (var components_Calculator_Copy in Calculator_Components_Copy)
                {
                    if (components_Calculator_Copy.ID_Component == curent_components_item.ID_Component)
                    {
                        contain_component = true;
                        it = curent_components_item.ID_Component;
                        break;
                    }
                }         
                //вставка компонента в список компонентов
                Components_Calculator new_components_Calculator = new Components_Calculator
                    {
                        ID_Component = curent_components_item.ID_Component,
                        Nam_Com = curent_components_item.Nam_Com,
                        Image = curent_components_item.Image,
                        Price = curent_components_item.Price,
                        Weight = curent_components_item.Weight,
                        Amount = await App.Database.GetAmount_Components_Object_Async(curent_components_item.ID_Component, curent_object.ID_0bject)*_,
                        H_Amount=0
                    };
                int add_item = await App.Database.GetAmount_Components_Object_Async(curent_components_item.ID_Component, curent_object.ID_0bject);
                contain_component = false;
                Components_Calculator contain_Components_Calculator=null;
                foreach (var components_Calculator_Copy in Calculator_Components_Copy)
                    {
                        if (components_Calculator_Copy.ID_Component == curent_components_item.ID_Component)
                        {
                            contain_component = true;
                            it = curent_components_item.ID_Component;
                            contain_Components_Calculator=components_Calculator_Copy;
                            break;
                        }
                    }
                if (contain_component == false && _>=0)
                    {
                        new_components_Calculator.Price=new_components_Calculator.Amount*curent_components_item.Price;
                        new_components_Calculator.Weight = new_components_Calculator.Amount * curent_components_item.Weight;
                        Calculator_Components.Add(new_components_Calculator);
                        Calculator_Components_Copy.Add(new_components_Calculator);
                    }
                else
                    {
                        if (NewEntry == String.Empty || NewEntry == "0")
                            contain_Components_Calculator.Amount -= (await App.Database.GetAmount_Components_Object_Async(curent_components_item.ID_Component, curent_object.ID_0bject) * int.Parse(OldEntry));
                        else
                        {
                            if (OldEntry != NewEntry && _>0)
                                contain_Components_Calculator.Amount += add_item * (_ - int.Parse(OldEntry));
                            else if (OldEntry == NewEntry && _ != 0)
                            {
                                if (_ != int.Parse(OldEntry))
                                contain_Components_Calculator.Amount += (add_item * _);
                                if (_ == int.Parse(OldEntry) && ib==0)
                                {
                                    contain_Components_Calculator.Amount += (add_item * _);
                                }
                            }
                           
                        }
                        contain_Components_Calculator.Price = contain_Components_Calculator.Amount * curent_components_item.Price;
                        contain_Components_Calculator.Weight = contain_Components_Calculator.Amount * curent_components_item.Weight;
                    }
            }
            IsBusy = false;
            Calculator_Components.Clear();
            foreach (var components_Calculator_Copy in Calculator_Components_Copy)
                Calculator_Components.Add(components_Calculator_Copy);
            IsBusy=true;
        }
        void AmountObjectComponentBiding(ObjectWithEntry objectWithEntry)
        {
            var deleteitem = objectWithEntry as ObjectWithEntry;
            var c = deleteitem.objects_A;
            var e = deleteitem.entry;
            string OldEntry = c.Amount.ToString();
            string NewEntry = e.Text;
            if (NewEntry != String.Empty)
                if (OldEntry == "0")
                    e.Text = "1";
                else
                e.Text = OldEntry;
        }
        async Task AmountObjectComponentDelete(ObjectWithEntry objectWithEntry)
        {
            IsBusy = false;
            var deleteitem = objectWithEntry as ObjectWithEntry;
            var c = deleteitem.objects_A;
            var e = deleteitem.entry;
            int _;
            int.TryParse(e.Text, out _);
            List<Components> curent_components = await App.Database.GetComponents_Components_Object(c.ID_0bject);
            foreach (var curent_components_item in curent_components)
            {
                foreach (var calculator_Components in Calculator_Components)
                {
                    if (calculator_Components.ID_Component == curent_components_item.ID_Component)
                    {
                        calculator_Components.Amount -= await App.Database.GetAmount_Components_Object_Async(calculator_Components.ID_Component,c.ID_0bject)*_;
                        if (calculator_Components.Amount == 0)
                        {
                            if(await App.Database.SearchComponents_Objects_Async(calculator_Components.ID_Component, c.ID_0bject)==1 && calculator_Components.H_Amount==0)
                            Calculator_Components.Remove(calculator_Components);
                        }
                        break;
                    }
                }
            }
            Calculator_Components_Copy.Clear();
            IsBusy = true;
            Amount_Object.Remove(c);
        }
        async Task AddObject()
        {
            isLoaded = false;
            int include = 1;
            var jsonStr = JsonConvert.SerializeObject(include);
            Amount_Object_Old.Clear();
            is_object_content = 1;
            await AppShell.Current.GoToAsync($"{nameof(ChangeObjectPage)}?Content={jsonStr}");
        }
        async private void PerformOperation(string getcont)
        {
            isLoaded = true;
            Amount_Object_Old.Clear();
            Amount_Junk_Old.Clear();
            foreach (var i in Amount_Object)
                Amount_Object_Old.Add(i);
            foreach (var i in Amount_Junk)
                Amount_Junk_Old.Add(i);
            var content = JsonConvert.DeserializeObject<List<int>>(getcont);
            if (is_object_content==1 && content!=null)
                foreach (int item in content)
                {
                    bool b = true;
                    var t = await App.Database.GetOneObjectsAsync(item);
                    Objects_A p = new Objects_A { ID_0bject = t.ID_0bject, Nam_Obj = t.Nam_Obj, Image = t.Image, Creation = t.Creation };
                    if (Amount_Object_Old.Count != 0)
                    {
                        foreach (var m in Amount_Object_Old)
                            if (m.ID_0bject == p.ID_0bject)
                            {
                                b = false;
                                break;
                            }
                        if (b) Amount_Object_Old.Add(p);
                    }
                    else Amount_Object_Old.Add(p);
                }
            else if (is_object_content == 2 && content!=null)    
                foreach (int item in content)
                {
                    bool b = true;
                    var t = await App.Database.GetOneJunkAsync(item);
                    Junk_A p = new Junk_A { ID_Junk = t.ID_Junk, Nam_Jun = t.Nam_Jun, Image = t.Image,Price=t.Price,Weight=t.Weight};
                    if (Amount_Junk_Old.Count != 0)
                    {
                        foreach (var m in Amount_Junk_Old)
                            if (m.ID_Junk == p.ID_Junk)
                            {
                                b = false;
                                break;
                            }
                        if (b) Amount_Junk_Old.Add(p);
                    }
                    else Amount_Junk_Old.Add(p);
                }

            foreach (var j in Amount_Object_Old)
                Amount_Object.Add(j);
            foreach (var j in Amount_Junk_Old)
                Amount_Junk.Add(j);
            Calculator_Components.Clear();
            Calculator_Components_Copy.Clear();
        }
        async Task AddJunk()
        {
            isLoaded = false;
            int include = 1;
            var jsonStr = JsonConvert.SerializeObject(include);
            Amount_Junk_Old.Clear();
            is_object_content = 2;
            await AppShell.Current.GoToAsync($"{nameof(ChangeJunkPage)}?Content={jsonStr}");
        }
        async Task AmountJunkComponent(JunkWithEntry JunkWithEntry)
        {
            var deleteitem = JunkWithEntry as JunkWithEntry;
            //текущий хлам и поле для ввода
            Junk_A curent_Junk = deleteitem.junk_A;
            var curent_entry = deleteitem.entry;
            if (curent_Junk.ID_Junk != id_bad_j)
                ib_j = 0;

            //старое и новое значения поля для ввода
            string OldEntry = curent_Junk.Amount.ToString();
            string NewEntry = curent_entry.Text;

            //ввод количества
            int _;
            bool t = int.TryParse(NewEntry, out _);
            if (_ > 1000)
            {
                _ = 0;
                NewEntry = "error";
                t = false;
            }
            if (!t)
            {
                if (NewEntry != String.Empty)
                {
                    curent_entry.Text = OldEntry;
                    ib_j++;
                    id_bad_j = curent_Junk.ID_Junk;
                }
                else
                    curent_Junk.Amount = 0;
            }
            else
            {
                if (_ < 0)
                {
                    curent_entry.Text = OldEntry;
                    ib_j++;
                    id_bad_j = curent_Junk.ID_Junk;
                }
                else
                {
                    curent_Junk.Amount = _;
                }
            }

            //компоненты текущего хлама
            List<Components> curent_components = await App.Database.GetComponents_Components_Junk(curent_Junk.ID_Junk);
            //подсчет количества компонентов
            foreach (var curent_components_item in curent_components)
            {
                //присутвует ли компонет в списке компонентов
                bool contain_component = false;

                int it = -1;

                //поиск компонента в списке компонентов
                foreach (var components_Calculator_Copy in Calculator_Components_Copy)
                {
                    if (components_Calculator_Copy.ID_Component == curent_components_item.ID_Component)
                    {
                        contain_component = true;
                        it = curent_components_item.ID_Component;
                        break;
                    }
                }

                //вставка компонента в список компонентов
                Components_Calculator new_components_Calculator = new Components_Calculator
                {
                    ID_Component = curent_components_item.ID_Component,
                    Nam_Com = curent_components_item.Nam_Com,
                    Image = curent_components_item.Image,
                    H_Amount = await App.Database.GetAmount_Components_Junk_Async(curent_components_item.ID_Component, curent_Junk.ID_Junk) * _
                };
                int add_item = await App.Database.GetAmount_Components_Junk_Async(curent_components_item.ID_Component, curent_Junk.ID_Junk);
                contain_component = false;
                Components_Calculator contain_Components_Calculator = null;
                foreach (var components_Calculator_Copy in Calculator_Components_Copy)
                {
                    if (components_Calculator_Copy.ID_Component == curent_components_item.ID_Component)
                    {
                        contain_component = true;
                        it = curent_components_item.ID_Component;
                        contain_Components_Calculator = components_Calculator_Copy;
                        break;
                    }
                }
                if (contain_component == false && _ >= 0)
                {
                    Calculator_Components.Add(new_components_Calculator);
                    Calculator_Components_Copy.Add(new_components_Calculator);
                }
                else
                {
                    if (NewEntry == String.Empty || NewEntry == "0")
                        contain_Components_Calculator.H_Amount -= (add_item * int.Parse(OldEntry));
                    else
                    {
                        if (OldEntry != NewEntry && _ > 0)
                            contain_Components_Calculator.H_Amount += add_item * (_ - int.Parse(OldEntry));
                        else if (OldEntry == NewEntry && _ != 0)
                        {
                            if (_ != int.Parse(OldEntry))
                                contain_Components_Calculator.H_Amount += add_item * _;
                            if (_ == int.Parse(OldEntry) && ib_j == 0)
                            {
                                contain_Components_Calculator.H_Amount += add_item * _;
                            }
                        }

                    }
                }
            }
            IsBusy = false;
            Calculator_Components.Clear();
            foreach (var components_Calculator_Copy in Calculator_Components_Copy)
                Calculator_Components.Add(components_Calculator_Copy);
            IsBusy = true;
        }
        void AmountJunkComponentBiding(JunkWithEntry JunkWithEntry)
        {
            var deleteitem = JunkWithEntry as JunkWithEntry;
            var c = deleteitem.junk_A;
            var e = deleteitem.entry;
            string OldEntry = c.Amount.ToString();
            string NewEntry = e.Text;
            if (NewEntry != String.Empty)
                if (OldEntry == "0")
                    e.Text = "1";
                else
                    e.Text = OldEntry;
        }
        async Task AmountJunkComponentDelete(JunkWithEntry JunkWithEntry)
        {
            IsBusy = false;
            var deleteitem = JunkWithEntry as JunkWithEntry;
            var c = deleteitem.junk_A;
            var e = deleteitem.entry;
            int _;
            int.TryParse(e.Text, out _);
            List<Components> curent_components = await App.Database.GetComponents_Components_Junk(c.ID_Junk);
            foreach (var curent_components_item in curent_components)
            {
                foreach (var calculator_Components in Calculator_Components)
                {
                    if (calculator_Components.ID_Component == curent_components_item.ID_Component)
                    {
                        calculator_Components.H_Amount -= await App.Database.GetAmount_Components_Junk_Async(calculator_Components.ID_Component, c.ID_Junk) * _;
                        if (calculator_Components.H_Amount == 0)
                        {
                            if (await App.Database.SearchComponents_Junk_Async(calculator_Components.ID_Component, c.ID_Junk) == 1 || calculator_Components.Amount==0)
                                Calculator_Components.Remove(calculator_Components);
                        }
                        break;
                    }
                }
            }
            Calculator_Components_Copy.Clear();
            IsBusy = true;
            Amount_Junk.Remove(c);
        }
    }
}
