using PhotoOrganizer.Common;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public class CommentsConverter : ConverterBase
    {
        public CommentsConverter()
        {
            MetaType = MetaProperty.Comments;
        }
    }
}