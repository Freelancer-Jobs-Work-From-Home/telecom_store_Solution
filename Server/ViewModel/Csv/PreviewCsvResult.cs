namespace Server.ViewModel.Csv
{
    public class PreviewCsvResult<T>
    {
        public List<T> PreviewData { get; set; }
        public string CsvFileBase64 { get; set; }
    }

}
