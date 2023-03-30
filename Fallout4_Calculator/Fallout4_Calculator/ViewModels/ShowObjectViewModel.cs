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
    public class ShowObjectViewModel : BaseViewModel
    {
        string content = "";
        string nextContent = "";
        string name_Object;
        string image_URL;
        bool creation_Object;
        string imageString;
        byte[] ImageBytes;
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

        public AsyncCommand UpdateObjectCommand { get; }
        public AsyncCommand DeleteObjectCommand { get; }
        public AsyncCommand ImageUploadCommand { get; }
        public AsyncCommand ImageURLCommand { get; }
        public Command ShowObjectAppearingCommand { get; }
        public Command ShowObjectDisappearingCommand { get; }
        public Command AmountObjectComponentCommand { get; }
        public AsyncCommand IncludeComponentCommand { get; }



        public ShowObjectViewModel()
        {
            UpdateObjectCommand = new AsyncCommand(UpdateObject);
            DeleteObjectCommand = new AsyncCommand(DeleteObject);
            ImageUploadCommand = new AsyncCommand(ImageUpload);
            ImageURLCommand = new AsyncCommand(ImageURL);
            ShowObjectAppearingCommand = new Command(ShowObjectAppearing);
            ShowObjectDisappearingCommand = new Command(ShowObjectDisappearing);
            AmountObjectComponentCommand = new Command<ComponentWithEntry>(AmountObjectComponent);
            Amount_Component = new ObservableRangeCollection<Components_A>();
            IncludeComponentCommand = new AsyncCommand(IncludeComponent);
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
        async Task UpdateObject()
        {
            if ((Name_Object != string.Empty) && (ImageString != string.Empty))
            {

                await App.Database.UpdateObjectAsync(new Objects
                {
                    ID_0bject = ID,
                    Nam_Obj = Name_Object,
                    Image = ImageString,
                    Creation = Convert.ToInt16(Creation_Object)
                });
                await App.Database.Delete_Object_Components_Object_Async(ID);
                foreach (var t in Amount_Component)
                {
                    if (t.Amount > 0)
                        await App.Database.SaveComponents_Object_Async(t.ID_Component, ID, t.Amount);
                }
            }
        }
        async Task DeleteObject()
        {
            await App.Database.Delete_Object_Components_Object_Async(ID);
            await App.Database.DeleteObjectAsync(new Objects
            {
                ID_0bject = ID,
            });
            await AppShell.Current.GoToAsync("../");
        }
        void ShowObjectAppearing()
        {
            Amount_Component.Clear();
        }
        void ShowObjectDisappearing()
        {
        }
        async Task IncludeComponent()
        {
            //bool include = true;
            int include = 2;
            var jsonStr = JsonConvert.SerializeObject(include);
            await AppShell.Current.GoToAsync($"{nameof(ChangeComponentPage)}?Content={jsonStr}");
        }
        void AmountObjectComponent(ComponentWithEntry componentWithEntry)
        {
            var deleteitem = componentWithEntry as ComponentWithEntry;
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
        private async void PerformOperation(string getcont)
        {
            var content = JsonConvert.DeserializeObject<Objects>(getcont);
            if (content != null)
            {
                ID = content.ID_0bject;
                var com_data = await App.Database.GetOneObjectsAsync(ID);
                Name_Object = com_data.Nam_Obj;
                ImageString = com_data.Image;
                Creation_Object = Convert.ToBoolean(com_data.Creation);
                var t = await App.Database.GetComponents_Components_Object(ID);
                foreach (var item in t)
                {
                    Components_A components_A = new Components_A
                    {
                        ID_Component = item.ID_Component,
                        Nam_Com = item.Nam_Com,
                        Weight = item.Weight,
                        Price = item.Price,
                        Image = item.Image,
                        Amount = await App.Database.GetAmount_Components_Object_Async(item.ID_Component, ID)
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
