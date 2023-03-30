using SQLite;

namespace Fallout4_Calculator.Models
{
    [Table("Object")]
    public class Objects
    {
        [PrimaryKey, AutoIncrement]
        public int ID_0bject { get; set; } //Первичный ключ
        public string Nam_Obj { get; set; } //Название объекта
        public string Image { get; set; } //Изображение объекта
        public int Creation { get; set; } //Показатель строительства
        
    }
}
