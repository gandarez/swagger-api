namespace Swagger.Api.Interfaces
{
    public interface IJsonSerializer
    {
        string SerializeObject(object objectToBeEncoded);
    }
}
