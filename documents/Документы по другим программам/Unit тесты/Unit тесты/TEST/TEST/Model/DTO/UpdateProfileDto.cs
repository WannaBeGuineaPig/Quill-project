namespace TEST.Model.DTO
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Patronymic { get; set; } = ""; // ← по умолчанию пусто
    }
}
