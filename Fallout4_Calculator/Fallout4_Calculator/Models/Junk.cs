using SQLite;

namespace Fallout4_Calculator.Models
{
    [Table("Junk")]
    public class Junk
    {
        [PrimaryKey, AutoIncrement]
        public int ID_Junk { get; set; } //Первичный ключ
        public string Nam_Jun { get; set; } //Название хлама
        public string Image { get; set; } //Изображение хлама
        public int Price { get; set; } //Цена хлама
        public decimal Weight { get; set; } //Вес хлама
    }
}
