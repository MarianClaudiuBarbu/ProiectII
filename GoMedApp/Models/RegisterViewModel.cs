namespace GoMedApp.Models
{
   public class RegisterViewModel
{
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string HashedPassword { get; set; } = null!;

}
}