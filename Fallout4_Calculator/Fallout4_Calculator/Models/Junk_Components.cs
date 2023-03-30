using SQLite;

namespace Fallout4_Calculator.Models
{

    [Table("Junk_Components")]
    public class Junk_Components
    {
        [Indexed]
        public int ID_Junk { get; set; } //Вторичный ключ
        [Indexed]
        public int ID_Component { get; set; } //Вторичный ключ
        public int Amount { get; set; } //Количество
    }

} 
