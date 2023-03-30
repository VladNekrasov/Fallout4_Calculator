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
    public class CreateJunkViewModel : BaseViewModel
    {
        string content = "";
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
        public AsyncCommand CreateJunkCommand { get; }
        public AsyncCommand ImageUploadCommand { get; }
        public AsyncCommand ImageURLCommand { get; }
        public Command CreateJunkAppearingCommand { get; }
        public Command CreateJunkDisappearingCommand { get; }
        public AsyncCommand IncludeComponentCommand { get; }
        public Command AmountObjectComponentCommand { get; }



        public CreateJunkViewModel()
        {
            CreateJunkCommand = new AsyncCommand(CreateJunk);
            ImageUploadCommand = new AsyncCommand(ImageUpload);
            ImageURLCommand = new AsyncCommand(ImageURL);
            WeightJunkCommand = new Command(WeightJunk);
            PriceJunkCommand = new Command(PriceJunk);
            CreateJunkAppearingCommand = new Command(CreateJunkAppearing);
            CreateJunkDisappearingCommand = new Command(CreateJunkDisappearing);
            Amount_Component = new ObservableRangeCollection<Components_A>();
            IncludeComponentCommand = new AsyncCommand(IncludeComponent);
            AmountObjectComponentCommand = new Command<ComponentWithEntry>(AmountObjectComponent);
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
        async Task CreateJunk()
        {
            if ((Name_Junk != string.Empty) && (ImageString != string.Empty) && (w_Junk != -1) && (p_Junk != -1))
            {

                int id=await App.Database.SaveJunkAsync(new Junk
                {
                    Nam_Jun = Name_Junk,
                    Image = ImageString,
                    Weight = w_Junk,
                    Price = p_Junk

                });
                foreach (var t in Amount_Component)
                {
                    if (t.Amount > 0)
                        await App.Database.SaveComponents_Junk_Async(t.ID_Component, id, t.Amount);
                }
            }
            Name_Junk = string.Empty;
            Image_URL = string.Empty;
            ImageString = string.Empty;
            Weight_Junk = string.Empty;
            Price_Junk = string.Empty;
            w_Junk = -1;
            p_Junk = -1;
            OldEntry = string.Empty;
            OldEntryP = string.Empty;
            Amount_Component.Clear();
        }
        void CreateJunkAppearing()
        {
            double _;
            double.TryParse("1.1", out _);
            if (_ == 0) dot_check = false;
            OldEntry = Weight_Junk;
            OldEntryP = Price_Junk;
            Amount_Component.Clear();


        }
        void CreateJunkDisappearing()
        {
        }
        async Task IncludeComponent()
        {
            int include = 3;
            var jsonStr = JsonConvert.SerializeObject(include);
            await AppShell.Current.GoToAsync($"{nameof(ChangeComponentPage)}?Content={jsonStr}");
        }
        void AmountObjectComponent(ComponentWithEntry componentWithEntry)
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
        async private void PerformOperation(string getcont)
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
