namespace FilunK.backenddotnet_trial.Models.Authentication
{
    public class RefreshRequestModel
    {
        public string AccessToken { get; set; }

        public string refreshToken { get; set; }
    }
}