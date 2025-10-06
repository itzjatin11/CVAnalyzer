namespace CVAnalyzer.Models
{
    public class DashboardViewModel
    {
        public List<ClusterGroupViewModel> StudentClusters { get; set; }
    }

    public class ClusterGroupViewModel
    {
        public string ClusterName { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string Skills { get; set; }
    }
}