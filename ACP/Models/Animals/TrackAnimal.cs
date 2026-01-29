

namespace ACP.Models.Animals
{
    public class TrackAnimal
    {
        public DateTime MustUploadBefore { get; set; }
        public bool IsCommitted { get; set; }
        public int AdoptionDetailsId { get; set; }
        public string ImageFileName { get; set; }

    }
}
