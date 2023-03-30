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
    public class CreateObjectViewModel : BaseViewModel
    {
        string content = "";
        string name_Object=string.Empty;
        string image_URL;
        bool creation_Object;
        string imageString=string.Empty;
        byte[] ImageBytes;

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
        public string Name_Object
        {
            get => name_Object;
            set => SetProperty(ref name_Object, value);
        }
        public bool Creation_Object
        {
            get => creation_Object;
            set => SetProperty(ref creation_Object, value);

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
       



        public AsyncCommand CreateObjectCommand { get; }
        public AsyncCommand ImageUploadCommand { get; }
        public AsyncCommand ImageURLCommand { get; }
        public Command CreateObjectAppearingCommand { get; }
        public Command CreateObjectDisappearingCommand { get; }
        public AsyncCommand IncludeComponentCommand { get; }
        public Command AmountObjectComponentCommand { get; }


        public CreateObjectViewModel()
        {
            CreateObjectCommand = new AsyncCommand(CreateObject);
            ImageUploadCommand = new AsyncCommand(ImageUpload);
            ImageURLCommand = new AsyncCommand(ImageURL);
            CreateObjectAppearingCommand = new Command(CreateObjectAppearing);
            CreateObjectDisappearingCommand = new Command(CreateObjectDisappearing);
            Amount_Component = new ObservableRangeCollection<Components_A>();
            IncludeComponentCommand = new AsyncCommand(IncludeComponent);
            AmountObjectComponentCommand = new Command<ComponentWithEntry>(AmountObjectComponent);
        }


        async Task CreateObject()
        {
            if ((Name_Object != string.Empty) && (ImageString != string.Empty))
            {

                int id=await App.Database.SaveObjectAsync(new Objects
                {   Nam_Obj = Name_Object,
                    Image = ImageString,
                    Creation = Convert.ToInt16(Creation_Object)
                });
                foreach (var t in Amount_Component)
                {
                    if (t.Amount>0)
                    await App.Database.SaveComponents_Object_Async(t.ID_Component,id,t.Amount);
                }
            }
            Name_Object = string.Empty;
            Image_URL = string.Empty;
            ImageString = string.Empty;
            Creation_Object = false;
            Amount_Component.Clear();
        }
        async Task ImageURL()
        {
            ImageString = string.Empty;
            Uri outUri;
            if (Uri.TryCreate(image_URL, UriKind.Absolute, out outUri)
               && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps))
            {
                Task<Xamarin.Forms.ImageSource> result = Task<Xamarin.Forms.ImageSource>.Factory.StartNew(() => Xamarin.Forms.ImageSource.FromUri(outUri));
                var _Object = await result;
                if (_Object != null) ImageString = image_URL;
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
        void CreateObjectAppearing()
        {
            Amount_Component.Clear();
        }
        void CreateObjectDisappearing()
        {
        }
        async Task IncludeComponent()
        {
            //bool include = true;
            int include = 1;
            var jsonStr = JsonConvert.SerializeObject(include);
            await AppShell.Current.GoToAsync($"{nameof(ChangeComponentPage)}?Content={jsonStr}");
        }
        void AmountObjectComponent(ComponentWithEntry componentWithEntry)
        {
            var deleteitem=componentWithEntry as ComponentWithEntry;
            var c = deleteitem.components;
            var e = deleteitem.entry;
            string OldEntry = c.Amount.ToString();
            string NewEntry = e.Text;
            int _;
            if (!int.TryParse(NewEntry, out _))
            {
                if (NewEntry != String.Empty)
                    e.Text = OldEntry;
            }
            else
            {
                if (_ < 0) e.Text = OldEntry;
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
                Amount_Component.Add(new Components_A { ID_Component=t.ID_Component,Nam_Com=t.Nam_Com,Image=t.Image,Weight=t.Weight,Price=t.Price });
            }      
        }
    }
}