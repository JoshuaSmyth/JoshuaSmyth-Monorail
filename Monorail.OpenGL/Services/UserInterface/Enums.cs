namespace Monorail.Framework.Services.UserInterface
{
    public enum Anchor
    {
        TopLeft = 0,
        TopMiddle = 1,
        TopRight = 2,
        CentreLeft = 3,
        CentreMiddle = 4,
        CentreRight = 5,
        BottomLeft = 6,
        BottomMiddle = 7,
        BottomRight = 8
    }

    public enum ChildLayoutMode
    {
        FreeForm = 0,
        Horizonal = 1,
        Vertical = 2,
        Grid = 3            // Need to know the width and height of the grid somehow
    }
}
