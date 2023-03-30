using System;
using System.Threading.Tasks;
using Fallout4_Calculator.Models;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Fallout4_Calculator.ViewModels
{
    [Xamarin.Forms.QueryProperty(nameof(Content), nameof(Content))]
    public class ShowComponentViewModel : BaseViewModel
    {
        string content="";
        string name_Component;
        string weight_Component;
        string price_Component;
        string image_URL;
        string imageString;
        byte[] ImageBytes;
        string OldEntry;
        string OldEntryP;
        bool dot_check = true;
        decimal w_Component = -1;
        int p_Component = -1;
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
        public string Name_Component
        {
            get => name_Component;
            set => SetProperty(ref name_Component, value);
        }
        public string Weight_Component
        {
            get => weight_Component;
            set => SetProperty(ref weight_Component, value);
        }
        public string Price_Component
        {
            get => price_Component;
            set => SetProperty(ref price_Component, value);
        }
        public string Image_URL
        {
            get => image_URL;
            set => SetProperty(ref image_URL, value);
        }
        public string ImageString
        {
            get => imageString;
            set => SetProperty (ref imageString, value);
        }



        public Command WeightComponentCommand { get; }
        public Command PriceComponentCommand { get; }
        public AsyncCommand UpdateComponentCommand { get; }
        public AsyncCommand DeleteComponentCommand { get; }
        public AsyncCommand ImageUploadCommand { get; }
        public AsyncCommand ImageURLCommand { get; }
        public AsyncCommand ShowComponentAppearingCommand { get; }
        public Command ShowComponentDisappearingCommand { get; }
        

        public ShowComponentViewModel()
        {
            UpdateComponentCommand = new AsyncCommand(UpdateComponent);
            DeleteComponentCommand = new AsyncCommand(DeleteComponent);
            ImageUploadCommand = new AsyncCommand(ImageUpload);
            ImageURLCommand = new AsyncCommand(ImageURL);
            WeightComponentCommand = new Command(WeightComponent);
            PriceComponentCommand = new Command(PriceComponent);
            ShowComponentAppearingCommand = new AsyncCommand(ShowComponentAppearing);
            ShowComponentDisappearingCommand = new Command(ShowComponentDisappearing);
        }


        void WeightComponent()
        {
            w_Component = -1;
            string NewEntry = Weight_Component;
            decimal _;
            if (!decimal.TryParse(NewEntry, out _))
            {
                if (NewEntry != String.Empty)
                    Weight_Component = OldEntry;
                else
                    OldEntry = String.Empty;
            }
            else
            {
                if ((NewEntry != string.Empty) && dot_check) NewEntry = NewEntry.Replace(',', '.');
                else if (NewEntry != string.Empty) NewEntry = NewEntry.Replace('.', ',');
                if (_ < 0) Weight_Component = OldEntry;
                else
                {
                    OldEntry = NewEntry;
                    Weight_Component = NewEntry;
                    w_Component = _;
                }
            }

        }
        void PriceComponent()
        {
            p_Component = -1;
            string NewEntry = Price_Component;
            int _;
            if (!int.TryParse(NewEntry, out _))
            {
                if (NewEntry != String.Empty)
                    Price_Component = OldEntryP;
                else
                    OldEntryP = String.Empty;
            }
            else
            {
                if ((NewEntry != string.Empty) && dot_check) NewEntry = NewEntry.Replace(',', '.');
                else if (NewEntry != string.Empty) NewEntry = NewEntry.Replace('.', ',');
                if (_ < 0) Price_Component = OldEntryP;
                else
                {
                    OldEntryP = NewEntry;
                    Price_Component = NewEntry;
                    p_Component = _;
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
                var _Component = await result;
                if (_Component != null) ImageString = image_URL;
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
                //Image_Component = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(ImageBytes));
                ImageString = imageBase64;
            }
        }
        async Task UpdateComponent()
        {
            if ((Name_Component != string.Empty) && (ImageString != string.Empty) && (w_Component != -1) && (p_Component != -1))
            {

                await App.Database.UpdateComponentAsync(new Components
                {
                    ID_Component=ID,
                    Nam_Com = Name_Component,
                    Image = ImageString,
                    Weight = w_Component,
                    Price = p_Component

                });
            }
        }
        async Task DeleteComponent()
        {
            await App.Database.Delete_Components_Components_Object_Async(ID);
            await App.Database.Delete_Components_Components_Junk_Async(ID);
            await App.Database.DeleteComponentAsync(new Components
            {
                ID_Component=ID,
            });
            await AppShell.Current.GoToAsync("../");
        }
        async Task ShowComponentAppearing()
        {
            double _;
            double.TryParse("1.1", out _);
            if (_ == 0) dot_check = false;
            var com_data = await  App.Database.GetOneComponentsAsync(ID);
            Name_Component = com_data.Nam_Com;
            Weight_Component = com_data.Weight.ToString();
            Price_Component = com_data.Price.ToString();
            ImageString = com_data.Image;
            OldEntry = Weight_Component;
            OldEntryP = Price_Component;

        }
        void ShowComponentDisappearing()
        {
        }
        private void PerformOperation(string getcont)
        {
            var content = JsonConvert.DeserializeObject<Components>(getcont);
            ID = content.ID_Component;
        }
    }
}