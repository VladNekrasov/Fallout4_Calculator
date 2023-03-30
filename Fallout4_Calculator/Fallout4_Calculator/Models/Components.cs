using SQLite;
namespace Fallout4_Calculator.Models
{
    [Table("Component")]
    public class Components
    {
        [PrimaryKey, AutoIncrement]
        public int ID_Component { get; set; } //Первичный ключ
        public string Nam_Com { get; set; } //Название компонента
        public string Image { get; set; } //Изображение компонента
        public int Price { get; set; } //Цена компонента
        public decimal Weight { get; set; } //Вес компонента
    }
}
