namespace NewsApp.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }

    public class CategoryGetDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CategoryAdminDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
