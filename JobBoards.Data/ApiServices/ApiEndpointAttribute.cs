namespace JobBoards.Data.ApiServices;

[AttributeUsage(AttributeTargets.Field)]
public class EndpointAttribute : Attribute
{
    public EndpointAttribute(string template)
    {
        Template = template;
    }

    public string Template { get; }
}