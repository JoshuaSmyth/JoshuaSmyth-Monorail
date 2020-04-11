namespace Monorail.Framework.Services.UserInterface
{
    public interface IElement
    {
        string Id { get; set; }

        Anchor Anchor { get; set;}

        float Width { get; set; }

        float Height { get; set; }

        float OffsetX { get; set; }

        float OffsetY { get; set; }
 
        //PlatformTexture PlatformTexture { get; }

        //ElementList Children { get; }

        IElement FindElementById(string id);
    }
}
