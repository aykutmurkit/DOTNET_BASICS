namespace test.DTOs
{
    public class StationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateStationDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateStationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
    }
} 