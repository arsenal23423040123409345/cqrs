using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Person
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string CreatedDate { get; set; }

        public int Status { get; set; }

        public int Age { get; set; }
    }
}
