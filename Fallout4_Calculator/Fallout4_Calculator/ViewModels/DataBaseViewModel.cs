using System.Threading.Tasks;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Fallout4_Calculator.Views;

namespace Fallout4_Calculator.ViewModels
{
    
    public class DataBaseViewModel:BaseViewModel
    {

        string text_page1= "В редакторе компонентов можно добавлять, удалять и изменять ранее созданные компоненты. Для перехода в окно добавления нажмите в верхнем правом углу кнопку Добавить компонент. Для перехода в режим редактирования нажмите на окно компонента (аналогично для объектов и хлама)";
        public string Text_page1
        {
            get => text_page1;
            set => SetProperty(ref text_page1, value);
        }
        string text_page2 = "В редакторе компонентов можно добавлять, удалять и изменять ранее созданные компоненты. Для перехода в окно добавления нажмите в верхнем правом углу кнопку Добавить компонент. Для перехода в режим редактирования нажмите на окно компонента (аналогично для объектов и хлама)";
        public string Text_page2
        {
            get => text_page2;
            set => SetProperty(ref text_page2, value);
        }
        public AsyncCommand ChangeObjectCommand { get; }
        public AsyncCommand ChangeComponentsCommand { get; }
        public AsyncCommand ChangeJunkCommand { get; }
        public AsyncCommand DataBaseAppearingCommand { get; }
        public Command DataBaseDisappearingCommand { get; }
        public DataBaseViewModel()
        {
            Title = "Редактор базы данных";
            ChangeObjectCommand = new AsyncCommand(ChangeObject);
            ChangeComponentsCommand = new AsyncCommand(ChangeComponents);
            ChangeJunkCommand = new AsyncCommand(ChangeJunk);
            DataBaseAppearingCommand = new AsyncCommand(DataBaseAppearing);
            DataBaseDisappearingCommand = new Command(DataBaseDisappearing);
        }



        async Task DataBaseAppearing()
        {
            await App.Database.CreateTables();          
        }
        void DataBaseDisappearing()
        {
        }
        async Task ChangeObject()
        { 
            await AppShell.Current.GoToAsync(nameof(ChangeObjectPage));

        }
        async Task ChangeComponents()
        {
            await AppShell.Current.GoToAsync(nameof(ChangeComponentPage));
        }
        async Task ChangeJunk()
        {
            await AppShell.Current.GoToAsync(nameof(ChangeJunkPage));
        }

    }
}
