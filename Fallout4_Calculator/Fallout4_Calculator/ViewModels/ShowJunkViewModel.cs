using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fallout4_Calculator.Models;
using Fallout4_Calculator.Views;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Fallout4_Calculator.ViewModels
{
    [Xamarin.Forms.QueryProperty(nameof(Content), nameof(Content))]
    [Xamarin.Forms.QueryProperty(nameof(NextContent), nameof(NextContent))]
    public class ShowJunkViewModel : BaseViewModel
    {
        string content = "";
        string nextContent = "";
        string name_Junk;
        string weight_Junk;
        string price_Junk;
        string image_URL;
        string imageString;
        byte[] ImageBytes;
        string OldEntry;
        string OldEntryP;
        bool dot_check = true;
        decimal w_Junk = -1;
        int p_Junk = -1;
        int ID;


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
        public string NextContent
        {
            get => nextContent;
            set
            {
                nextContent = Uri.UnescapeDataString(value ?? string.Empty);
                OnPropertyChanged();
                PerformOperationNext(nextContent);
            }
        }
        public string Name_Junk
        {
            get => name_Junk;
            set => SetProperty(ref name_Junk, value);
        }
        public string Weight_Junk
        {
            get => weight_Junk;
            set => SetProperty(ref weight_Junk, value);
        }
        public string Price_Junk
        {
            get => price_Junk;
            set => SetProperty(ref price_Junk, value);
        }
        public string Image_URL
        {
            get => image_URL;
            set => SetProperty(ref image_URL, value);
        }
        public string ImageString
        {
            get => imageString;
            set => SetProperty(ref imageString, value);
        }
        public ObservableRangeCollection<Components_A> Amount_Component { get; set; }



        public Command WeightJunkCommand { get; }
        public Command PriceJunkCommand { get; }
        public AsyncCommand UpdateJunkCommand { get; }
        public AsyncCommand DeleteJunkCommand { get; }
        public AsyncCommand ImageUploadCommand { get; }
        public AsyncCommand ImageURLCommand { get; }
        public Command ShowJunkAppearingCommand { get; }
        public Command ShowJunkDisappearingCommand { get; }
        public Command AmountJunkComponentCommand { get; }
        public AsyncCommand IncludeComponentCommand { get; }


        public ShowJunkViewModel()
        {
            UpdateJunkCommand = new AsyncCommand(UpdateJunk);
            DeleteJunkCommand = new AsyncCommand(DeleteJunk);
            ImageUploadCommand = new AsyncCommand(ImageUpload);
            ImageURLCommand = new AsyncCommand(ImageURL);
            WeightJunkCommand = new Command(WeightJunk);
            PriceJunkCommand = new Command(PriceJunk);
            ShowJunkAppearingCommand = new Command(ShowJunkAppearing);
            ShowJunkDisappearingCommand = new Command(ShowJunkDisappearing);
            AmountJunkComponentCommand = new Command<ComponentWithEntry>(AmountJunkComponent);
            Amount_Component = new ObservableRangeCollection<Components_A>();
            IncludeComponentCommand = new AsyncCommand(IncludeComponent);
        }


        void WeightJunk()
        {
            w_Junk = -1;
            string NewEntry = Weight_Junk;
            decimal _;
            if (!decimal.TryParse(NewEntry, out _))
            {
                if (NewEntry != String.Empty)
                    Weight_Junk = OldEntry;
                else
                    OldEntry = String.Empty;
            }
            else
            {
                if ((NewEntry != string.Empty) && dot_check) NewEntry = NewEntry.Replace(',', '.');
                else if (NewEntry != string.Empty) NewEntry = NewEntry.Replace('.', ',');
                if (_ < 0) Weight_Junk = OldEntry;
                else
                {
                    OldEntry = NewEntry;
                    Weight_Junk = NewEntry;
                    w_Junk = _;
                }
            }

        }
        void PriceJunk()
        {
            p_Junk = -1;
            string NewEntry = Price_Junk;
            int _;
            if (!int.TryParse(NewEntry, out _))
            {
                if (NewEntry != String.Empty)
                    Price_Junk = OldEntryP;
                else
                    OldEntryP = String.Empty;
            }
            else
            {
                if ((NewEntry != string.Empty) && dot_check) NewEntry = NewEntry.Replace(',', '.');
                else if (NewEntry != string.Empty) NewEntry = NewEntry.Replace('.', ',');
                if (_ < 0) Price_Junk = OldEntryP;
                else
                {
                    OldEntryP = NewEntry;
                    Price_Junk = NewEntry;
                    p_Junk = _;
                }
            }

        }
        async Task ImageURL()
        {
            ImageString = string.Empty;
            Uri outUri;
            if (Uri.TryCreate(image_URL, UriKind.Absolute, out outUri)
               && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps))
            {
                Task<Xamarin.Forms.ImageSource> result = Task<Xamarin.Forms.ImageSource>.Factory.StartNew(() => Xamarin.Forms.ImageSource.FromUri(outUri));
                var _Junk = await result;
                if (_Junk != null) ImageString = image_URL;
            }
        }
        async Task ImageUpload()
        {
            ImageString = string.Empty;
            var result = await MediaPicker.PickPhotoAsync();
            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    ImageBytes = memoryStream.ToArray();
                }

                string imageBase64 = Convert.ToBase64String(ImageBytes);
                ImageString = imageBase64;
            }
        }
        async Task UpdateJunk()
        {
            if ((Name_Junk != string.Empty) && (ImageString != string.Empty) && (w_Junk != -1) && (p_Junk != -1))
            {

                await App.Database.UpdateJunkAsync(new Junk
                {
                    ID_Junk = ID,
                    Nam_Jun = Name_Junk,
                    Image = ImageString,
                    Weight = w_Junk,
                    Price = p_Junk

                });
                await App.Database.Delete_Junk_Components_Junk_Async(ID);
                foreach (var t in Amount_Component)
                {
                    if (t.Amount > 0)
                        await App.Database.SaveComponents_Junk_Async(t.ID_Component, ID, t.Amount);
                }
            }
        }
        async Task DeleteJunk()
        {
            await App.Database.Delete_Junk_Components_Junk_Async(ID);
            await App.Database.DeleteJunkAsync(new Junk
            {
                ID_Junk = ID,
            });
            await AppShell.Current.GoToAsync("../");
        }
        void ShowJunkAppearing()
        {
            double _;
            double.TryParse("1.1", out _);
            if (_ == 0) dot_check = false;
            Amount_Component.Clear();

        }
        void ShowJunkDisappearing()
        {
        }
        async Task IncludeComponent()
        {
            int include = 4;
            var jsonStr = JsonConvert.SerializeObject(include);
            await AppShell.Current.GoToAsync($"{nameof(ChangeComponentPage)}?Content={jsonStr}");
        }
        void AmountJunkComponent(ComponentWithEntry componentWithEntry)
        {
            var deleteitem = componentWithEntry as ComponentWithEntry;
            var c = deleteitem.components;
            var e = deleteitem.entry;
            string OldEntryA = c.Amount.ToString();
            string NewEntry = e.Text;
            int _;
            if (!int.TryParse(NewEntry, out _))
            {
                if (NewEntry != String.Empty)
                    e.Text = OldEntryA;
            }
            else
            {
                if (_ < 0) e.Text = OldEntryA;
                else
                {
                    c.Amount = _;
                }
            }
        }
        private async void PerformOperation(string getcont)
        {
            var content = JsonConvert.DeserializeObject<Junk>(getcont);
            if (content != null)
            {
                ID = content.ID_Junk;
                var com_data = await App.Database.GetOneJunkAsync(ID);
                Name_Junk = com_data.Nam_Jun;
                ImageString = com_data.Image;
                Weight_Junk = com_data.Weight.ToString();
                Price_Junk = com_data.Price.ToString();
                OldEntry = Weight_Junk;
                OldEntryP = Price_Junk;
                var t = await App.Database.GetComponents_Components_Junk(ID);
                foreach (var item in t)
                {
                    Components_A components_A = new Components_A
                    {
                        ID_Component = item.ID_Component,
                        Nam_Com = item.Nam_Com,
                        Weight = item.Weight,
                        Price = item.Price,
                        Image = item.Image,
                        Amount = await App.Database.GetAmount_Components_Junk_Async(item.ID_Component, ID)
                    };
                    Amount_Component.Add(components_A);
                }
            }
        }
        private async void PerformOperationNext(string getcont)
        {
            var content = JsonConvert.DeserializeObject<List<int>>(getcont);
            foreach (int item in content)
            {
                var t = await App.Database.GetOneComponentsAsync(item);
                Amount_Component.Add(new Components_A { ID_Component = t.ID_Component, Nam_Com = t.Nam_Com, Image = t.Image, Weight = t.Weight, Price = t.Price });
            }
        }
    }
}
