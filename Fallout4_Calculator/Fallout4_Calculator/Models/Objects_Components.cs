using SQLite;

namespace Fallout4_Calculator.Models
{
    [Table("Objects_Components")]
    public class Objects_Components
    {
        [Indexed]
        public int ID_Object { get; set; } //Вторичный ключ
        [Indexed]
        public int ID_Component { get; set; } //Вторичный ключ
        public int Amount { get; set; } //Количество
    }
}
